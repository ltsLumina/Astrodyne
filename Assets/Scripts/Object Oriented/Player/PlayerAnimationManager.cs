using System;
using System.Collections;
using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour
{
    Player player;
    Dash dash;
    GameObject afterImage;
    ParticleSystem.MainModule parSys;
    Camera cam;
    SpriteRenderer sprite;

    public static Animator PlayerAnim { get; private set; }

    [Header("Cached Hashes")]
    static readonly int IsMoving = Animator.StringToHash("isMoving");

    void Start()
    {
        PlayerAnim = GetComponent<Animator>();

        player     = GetComponentInParent<Player>();
        dash       = GetComponentInParent<Dash>();
        afterImage = transform.GetChild(1).gameObject; // TODO: Make this more robust, rather than accessing by index. //P.S. I tried. Not working regardless.
        parSys     = afterImage.GetComponentInChildren<ParticleSystem>().main;
        cam        = Camera.main;
        sprite     = GetComponentInChildren<SpriteRenderer>();

        player.onMoveInputChanged += _ => PlayerAnim.SetBool(IsMoving, player.MoveInput != Vector2.zero);
        dash.onDash += () =>
        {
            if (!dash.IsDashing) return;
            CameraShake.Instance.ShakeCamera(3f, 0.2f);
            StartCoroutine(AfterImageRoutine());
        };
    }

    void Update() => FacingDirection();

    void FacingDirection()
    { // Smoothly rotates player to face mouse.
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

        // Flip the sprite if the mouse is on the left side of the player
        if (mousePos.x < transform.position.x)
        {
            sprite.flipX  = true;
            player.IsFacingRight = false;
            Debug.Assert(!player.IsFacingRight, "Facing Left!");
        }
        else
        {
            sprite.flipX  = false;
            player.IsFacingRight = true;
            Debug.Assert(player.IsFacingRight, "Facing Right!");
        }
    }

    #region Coroutines
    IEnumerator AfterImageRoutine()
    {
        afterImage.SetActive(true);

        float totalDuration = parSys.startLifetime.constant;

        yield return new WaitForSeconds(totalDuration);

        afterImage.SetActive(false);
    }

    public static IEnumerator SpriteRoutine(float duration, SpriteRenderer sprite)
    {
        float timeElapsed = 0f;

        // Flash the player red.
        sprite.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sprite.color = Color.white;
        yield return new WaitForSeconds(0.1f);

        while (timeElapsed < duration)
        {
            // Blink the player to indicate invincibility frames.
            const float blinkTime = 0.1f;

            for (int i = 0; i < duration; i++)
            {
                sprite.enabled = false;
                yield return new WaitForSeconds(blinkTime);
                sprite.enabled = true;
                yield return new WaitForSeconds(blinkTime);
            }
            timeElapsed += 0.2f;
        }
    }
    #endregion
}