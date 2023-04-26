using UnityEngine;

/// <summary>
/// Possibly a temporary script, unsure if this will be used or simply replaced by the MeleeSystem script.
/// </summary>
public class Scythe : MonoBehaviour
{
    Camera cam;
    Transform playerPos;
    WeaponAnimatorManager weaponManager;
    SlashEffect slashEffect;

    void Start()
    {
        cam       = Camera.main;
        playerPos = GetComponentInParent<Player>().transform;
        weaponManager = GetComponentInChildren<WeaponAnimatorManager>();
        slashEffect = GetComponentInChildren<SlashEffect>();
    }

    void Update() => HandleRotation();

    void HandleRotation()
    {
        Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

        // Rotates the object in relation to the mouse position.
        Vector2 direction = mousePos - (Vector2)playerPos.position;

        transform.up = direction;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        weaponManager.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}