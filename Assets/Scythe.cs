using UnityEngine;

/// <summary>
/// Possibly a temporary script, unsure if this will be used or simply replaced by the MeleeSystem script.
/// </summary>
public class Scythe : MonoBehaviour
{
    Camera cam;
    Transform playerPos;

    void Start()
    {
        cam       = Camera.main;
        playerPos = GetComponentInParent<Player>().transform;
    }

    void Update() => HandleRotation();

    void HandleRotation()
    {
        // Gets the mouse position in the world.
        Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

        // Rotates the object in relation to the mouse position.
        Vector2 direction = mousePos - (Vector2)playerPos.position;
        transform.up = direction;

        // Sets the rotation of the weapon.
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        WeaponAnimatorManager.WeaponAnim.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward); //TODO: fix so the scythe weapon art doesn't rotate.
    }
}