using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(CapsuleCollider2D))]
public class Player : MonoBehaviour
{
    #region SerializedFields
    [Header("Cached References")]
    // ReSharper disable once InconsistentNaming
    Rigidbody2D RB;
    new CapsuleCollider2D collider;
    Camera cam;
    [SerializeField]
    SpriteRenderer sr;

    [Header("Movement")]
    [SerializeField] float moveSpeed = 5f;

    [SerializeField] float dashForce = 5f;
    [SerializeField] float dashDuration = 0.5f;
    [SerializeField] float dashCooldown = 1f;

    [Header("Attacking")]
    [SerializeField] float attackRange = 1f;
    [SerializeField] float attackDamage = 1f;
    [SerializeField] float attackCooldown = 1f;

    [Header("Abilities")]
    [SerializeField] float abilityRange = 1f;
    [SerializeField] float abilityDamage = 1f;
    [SerializeField] float abilityCooldown = 1f;
    [SerializeField] float drawCooldown;

    [Header("Health")]
    [SerializeField] float maxHealth = 100f;
    [SerializeField] float currentHealth = 100f;
    #endregion

    #region Properties
    [field: Header("Configurable Variables")]
    public bool IsFacingRight { get; set; } = true;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        cam      = Camera.main;
        RB       = GetComponent<Rigidbody2D>();
        collider = GetComponent<CapsuleCollider2D>();
        //sr       = GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Boundaries();
        Movement();
        FaceMouse();
    }

    void Boundaries()
    {
        // Keeps the player within the camera's view.
        Vector3 viewPos = cam.WorldToViewportPoint(transform.position);
        viewPos.x       = Mathf.Clamp01(viewPos.x);
        viewPos.y       = Mathf.Clamp01(viewPos.y);
        transform.position = cam.ViewportToWorldPoint(viewPos);
    }

    void Movement()
    { // Movement: WASD or Arrow Keys
        // Moves the player by getting their input and multiplying it by the move speed.
        // Gives the movement an acceleration/deceleration feel.
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        RB.velocity = new Vector2(x, y) * moveSpeed;
    }

    void FaceMouse()
    {
        // Smoothly rotates player to face mouse.
        Vector3 mousePos  = cam.ScreenToWorldPoint(Input.mousePosition);

        // flip the sprite if the mouse is on the left side of the player
        if (mousePos.x < transform.position.x)
        {
            sr.flipY = true;
            IsFacingRight = false;
            Debug.Log("Facing Left");
        }
        else
        {
            sr.flipY = false;
            IsFacingRight = true;
            Debug.Log("Facing Right");
        }
    }
}