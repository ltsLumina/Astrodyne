using UnityEngine;

public class Astrodyne : MonoBehaviour
{
    [Header("Cached References")]
    Player player;
    [SerializeField] Transform point;

    public Transform Point
    {
        get => point;
        set => point = value;
    }

    [Header("Configurable Variables"), Tooltip("The speed at which the weapon orbits around the player. \n (Degrees/Second)")]
    [SerializeField] float orbitSpeed;

    void Start()
    {
        player = FindObjectOfType<Player>();
    }

    void Update()
    {
        Orbit(orbitSpeed);
    }

    void Orbit(float orbitSpeed)
    {
        point.position = player.transform.position;

        // Rotate the weapon around the player.
        transform.RotateAround(point.position, Vector3.forward, orbitSpeed * Time.deltaTime);
    }
}