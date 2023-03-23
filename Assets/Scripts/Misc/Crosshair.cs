using System;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    // Get the mouse position.
    Camera cam;

    void Start()
    {
        cam = Camera.main;
        Cursor.visible = false;
    }

    void Update()
    {
        FollowCursor();
    }

    void FollowCursor()
    {
        Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        transform.position = mousePos;
    }
}