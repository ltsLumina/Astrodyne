using System;
using System.Collections;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    Player player;
    Dash dash;
    SpriteRenderer sprite;
    GameObject afterImage;
    ParticleSystem.MainModule parSys;
    Weapon weapon;

    public Animator Anim { get; private set; }
    public EventHandler OnAnimationChange;

    [Header("Cached Hashes")]
    readonly static int IsMoving = Animator.StringToHash("isMoving");
    readonly static int InCombat = Animator.StringToHash("inCombat");

    void Awake()
    {
        player     = GetComponentInParent<Player>();
        dash       = GetComponentInParent<Dash>();
        Anim       = GetComponent<Animator>();
        sprite     = GetComponent<SpriteRenderer>();
        afterImage = transform.GetChild(1).gameObject;
        parSys     = afterImage.GetComponentInChildren<ParticleSystem>().main;
        weapon     = transform.parent.GetComponentInChildren<Weapon>();

    }

    void Update()
    {
        HandleAnimationChange();
    }

    void HandleAnimationChange()
    {
        OnAnimationChange?.Invoke(this, EventArgs.Empty); //TODO: this

        // If the player is moving, set the isMoving parameter to true.
        Anim.SetBool(IsMoving, player.moveInput != Vector2.zero);

        // In combat
        Anim.SetBool(InCombat, weapon.IsInCombat());

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