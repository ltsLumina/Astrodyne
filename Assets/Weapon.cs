using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

//TODO: Doesn't work at all.

public class Weapon : MonoBehaviour
{
    Rigidbody2D RB;
    new CapsuleCollider2D collider;
    Transform player;

    void Start()
    {
        // ReSharper disable once InconsistentNaming
        RB       = GetComponent<Rigidbody2D>();
        collider = GetComponent<CapsuleCollider2D>();
        player   = FindObjectOfType<Player>().transform;
    }

    void Update()
    {
        Orbit();
    }

    void Orbit()
    {
        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;

        difference.Normalize();

        float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, rotationZ);

        if (rotationZ is < -90 or > 90)
        {
            if (player.transform.localEulerAngles.y == 0)
            {
                transform.localRotation = Quaternion.Euler(180, 0, -rotationZ);
            } else if (player.transform.eulerAngles.y == 180)
            {
                transform.localRotation = Quaternion.Euler(180, 180, -rotationZ);
            }
        }

    }
}