#region
using System.Collections.Generic;
using UnityEngine;
#endregion

//TODO: REDO SCRIPT
public class Afterimage : MonoBehaviour
{
    [SerializeField] int numImages = 5;
    [SerializeField] float interval = 0.05f;
    [SerializeField] float imageSpacing = 0.1f;
    [SerializeField] float imageOpacity = 0.5f;

    SpriteRenderer spriteRenderer;
    readonly List<GameObject> images = new();

    // The timer for creating afterimages
    float timer;

    void Start() =>
        // Get the sprite renderer component of the object
        spriteRenderer = GetComponent<SpriteRenderer>();

    void Update()
    {
        // Create a new afterimage if the timer has elapsed
        timer += Time.deltaTime;

        if (timer >= interval)
        {
            timer -= interval;
            CreateAfterImage();
        }

        // Update the position and opacity of each afterimage
        for (int i = 0; i < images.Count; i++)
        {
            // Calculate the position and rotation of the current afterimage
            float      alpha    = i / (float) numImages;
            Vector3    position = transform.position - i * imageSpacing * transform.up;
            Quaternion rotation = transform.rotation;

            // Set the position, rotation, and opacity of the current afterimage
            images[i].transform.position = position;
            images[i].transform.rotation = rotation;
            Color color = spriteRenderer.color;
            color.a                                        = imageOpacity * alpha;
            images[i].GetComponent<SpriteRenderer>().color = color;
        }
    }

    // Creates a new afterimage object and adds it to the list
    void CreateAfterImage()
    {
        // Create a new game object and set its properties
        var image = new GameObject("AfterImage");
        image.transform.parent                      = transform.parent;
        image.transform.position                    = transform.position;
        image.transform.rotation                    = transform.rotation;
        image.transform.localScale                  = transform.localScale;
        image.AddComponent<SpriteRenderer>().sprite = spriteRenderer.sprite;

        // Add the game object to the list and set its opacity
        images.Insert(0, image);
        Color color = spriteRenderer.color;
        color.a                                    = imageOpacity;
        image.GetComponent<SpriteRenderer>().color = color;

        // Destroy the oldest afterimage if we've exceeded the maximum limit
        if (images.Count > numImages)
        {
            Destroy(images[numImages]);
            images.RemoveAt(numImages);
        }
    }
}