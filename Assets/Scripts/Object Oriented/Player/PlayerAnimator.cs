using System.Collections;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    Player player;
    Dash dash;
    Animator anim;
    SpriteRenderer sprite;
    GameObject afterImage;
    ParticleSystem.MainModule parSys;

    [Header("Cached Hashes")]
    readonly static int IsMoving = Animator.StringToHash("isMoving");

    void Awake()
    {
        player     = GetComponentInParent<Player>();
        dash       = GetComponentInParent<Dash>();
        anim       = GetComponent<Animator>();
        sprite     = GetComponent<SpriteRenderer>();
        afterImage = transform.GetChild(1).gameObject;
        parSys     = transform.GetChild(1).GetComponent<ParticleSystem>().main;

    }

    void Update()
    {
        HandleAnimationChange();
    }

    void HandleAnimationChange()
    {
        // If the player is moving, set the isMoving parameter to true.
        anim.SetBool(IsMoving, player.moveInput != Vector2.zero);

        // Attack animation.

        // Dash animation.
        // Shake screen, character trail, etc.
        if (dash.IsDashing) CameraShake.Instance.ShakeCamera(1.5f, 0.2f);

        if (dash.IsDashing) StartCoroutine(AfterImageRoutine());

        // Death animation.
    }

    IEnumerator AfterImageRoutine()
    {
        afterImage.SetActive(true);

        float totalDuration = parSys.startLifetime.constant;

        yield return new WaitForSeconds(totalDuration);

        afterImage.SetActive(false);
    }
}