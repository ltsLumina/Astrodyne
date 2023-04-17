using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerAnimator : MonoBehaviour
{
    Player player;
    SpriteRenderer playerSprite;
    Dash dash;
    SpriteRenderer sprite;
    GameObject afterImage;
    ParticleSystem.MainModule parSys;
    Weapon weapon;

    public Animator Anim { get; private set; }

    [Header("Cached Hashes")]
    readonly static int IsMoving = Animator.StringToHash("isMoving");
    readonly static int InCombat = Animator.StringToHash("inCombat");

    void Start()
    {
        player     = GetComponentInParent<Player>();
        dash       = GetComponentInParent<Dash>();
        Anim       = GetComponent<Animator>();
        sprite     = GetComponent<SpriteRenderer>();
        afterImage = transform.GetChild(1).gameObject;
        parSys     = afterImage.GetComponentInChildren<ParticleSystem>().main;
        weapon     = transform.parent.GetComponentInChildren<Weapon>();

        // Alternatively:
        // player.onMoveInputChanged += OnMoveInputChanged;
        // weapon.onShoot            += OnShoot;
        // dash.onDash               += OnDash;

        player.onMoveInputChanged += _ => Anim.SetBool(IsMoving, player.MoveInput != Vector2.zero);
        weapon.onShoot            += () => Anim.SetBool(InCombat, weapon.IsInCombat());
        dash.onDash               += () =>
        {
            if (!dash.IsDashing) return;
            CameraShake.Instance.ShakeCamera(3f, 0.2f);
            StartCoroutine(AfterImageRoutine());
        };
    }

    #region Event Handlers
    void OnMoveInputChanged(Vector2 moveInput) =>
        Anim.SetBool(IsMoving, moveInput != Vector2.zero);

    void OnShoot() =>
        Anim.SetBool(InCombat, weapon.IsInCombat());

    void OnDash()
    {
        if (!dash.IsDashing) return;
        CameraShake.Instance.ShakeCamera(3f, 0.2f);
        StartCoroutine(AfterImageRoutine());
    }
    #endregion

    #region Coroutines
    IEnumerator AfterImageRoutine()
    {
        afterImage.SetActive(true);

        float totalDuration = parSys.startLifetime.constant;

        yield return new WaitForSeconds(totalDuration);

        afterImage.SetActive(false);
    }

    public IEnumerator SpriteRoutine(float duration)
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