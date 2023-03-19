using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    #region Serialized Fields
    [Header("Cached References")]
    Transform playerPos;
    Player player;
    Camera cam;

    [Header("Shooting Parameters"), Tooltip("Parameters that govern the shooting of the weapon.")]
    float timeSinceLastShot;
    [SerializeField, Tooltip("The rate at which the weapon can shoot. \n RPM (Rounds Per Minute)")]
    float fireRate;

    [Header("Bullet Fields")]
    [SerializeField] GameObject bullet; Rigidbody2D bulletRB;
    [SerializeField] float bulletForce;
    #endregion

    void Start()
    {
        playerPos = FindObjectOfType<Player>().transform;
        player    = FindObjectOfType<Player>();
        cam       = Camera.main;
    }

    void Update()
    {
        WeaponLogic();
    }

    /// <summary>
    /// Handles all the logic for the weapon such as the rotation, position, and shooting.
    /// </summary>
    void WeaponLogic()
    {
        HandleRotation();

        void HandleRotation()
        {
            // Converts the mousePos from Screen values to World values.
            Vector2 mousePos   = cam.ScreenToWorldPoint(Input.mousePosition);

            // Rotates the object in relation to the mouse position.
            Vector2 direction  = mousePos - (Vector2)playerPos.position;
            float   angle      = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle + 270, Vector3.forward);
        }

        HandleWeaponPosition();

        void HandleWeaponPosition()
        {
            // Moves the weapon slightly to the left or right, depending on if the player isFacingRight.
            transform.position = player.IsFacingRight
                ? (Vector2)playerPos.position + new Vector2(-0.3f, 0.5f)
                : (Vector2)playerPos.position + new Vector2(0.3f, 0.5f);
        }

        HandleShooting();

        void HandleShooting()
        {
            // Increments the time since the last shot, and while it is larger than the fireRate, the player can shoot.
            timeSinceLastShot += Time.deltaTime;

            if (!Input.GetMouseButton(0) || !(timeSinceLastShot > fireRate)) return;
            // Instantiates an object and adds force to it.
            bullet   = Instantiate(bullet, transform.position, transform.rotation);
            bulletRB = bullet.GetComponent<Rigidbody2D>();
            bulletRB.AddForce(transform.up * bulletForce, ForceMode2D.Impulse);

            // Reset the time since the last shot.
            timeSinceLastShot = 0;
        }
    }
}