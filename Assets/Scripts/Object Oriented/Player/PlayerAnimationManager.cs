using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerAnimationManager : MonoBehaviour
{
    Player player;
    SpriteRenderer playerSprite;
    Dash dash;
    GameObject afterImage;
    ParticleSystem.MainModule parSys;
    Weapon weapon;
    SpriteRenderer weaponSprite;
    MeleeComboSystem meleeSys;

    public Animator PlayerAnim { get; private set; }

    [Header("Cached Hashes")]
    readonly static int IsMoving = Animator.StringToHash("isMoving");
    readonly static int InCombat = Animator.StringToHash("inCombat");

    void Start()
    {
        player       = GetComponentInParent<Player>();
        playerSprite = GetComponent<SpriteRenderer>();
        dash         = GetComponentInParent<Dash>();
        afterImage   = transform.GetChild(1).gameObject; // TODO: Make this more robust, rather than accessing by index.
        parSys       = afterImage.GetComponentInChildren<ParticleSystem>().main;
        PlayerAnim   = GetComponent<Animator>();

        // All weapon related components are children of the player.
        var parent = transform.parent;
        weapon       = parent.GetComponentInChildren<Weapon>();
        meleeSys     = parent.GetComponentInChildren<MeleeComboSystem>();
        weaponSprite = weapon.GetComponent<SpriteRenderer>();

        player.onMoveInputChanged += _ => PlayerAnim.SetBool(IsMoving, player.MoveInput != Vector2.zero);
        weapon.onShoot            += EnterCombat;
        meleeSys.onMeleeAttack += () =>
        {
            RandomizeSlashDirection();
            EnterCombat();
        };
        dash.onDash += () =>
        {
            if (!dash.IsDashing) return;
            CameraShake.Instance.ShakeCamera(3f, 0.2f);
            StartCoroutine(AfterImageRoutine());
        };
    }

    #region Event Handlers
    void EnterCombat() =>
        PlayerAnim.SetBool(InCombat, weapon.IsInCombat());
        #endregion

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

    //TODO: Move this to a more appropriate class, e.g WeaponAnimationManager.
    void RandomizeSlashDirection()
    {
        int rand = UnityEngine.Random.Range(0, 2);
        weaponSprite.flipY = rand == 1;
    }
}