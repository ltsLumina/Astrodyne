using System;
using System.Collections;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    Player player;
    Dash dash;
    Animator anim;
    SpriteRenderer sprite;

    [Header("Cached Hashes")]
    readonly static int IsMoving = Animator.StringToHash("isMoving");

    void Awake()
    {
        player = GetComponentInParent<Player>();
        anim   = GetComponentInChildren<Animator>();
        dash   = GetComponentInParent<Dash>();
        sprite = GetComponent<SpriteRenderer>();
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
        if (dash.IsDashing)
            CameraShake.Instance.ShakeCamera(1.5f, 0.2f);

        // Death animation.
    }

    public IEnumerator DamageRoutine() //TODO: add invincibility frames and flash the player invisible.
    {
        sprite.color = new (255, 0, 0);
        yield return new WaitForSeconds(0.1f);
        sprite.color = new (255, 255, 255);
    }
}