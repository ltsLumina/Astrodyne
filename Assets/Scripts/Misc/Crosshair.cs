using System;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    // Get the mouse position.
    Vector2 mousePos;
    Camera cam;
    Player boundaries;

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
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        transform.position = mousePos;
    }
}