using UnityEngine;

public class Astrodyne : MonoBehaviour
{
    #region Serialized Fields
    [Header("Cached References")]
    Player player;

    [Header("Point to rotate around."), Space(15)]
    [SerializeField] Transform point;

    [Header("Configurable Variables"), Tooltip("The speed at which the weapon orbits around the player. \n (Degrees/Second)")]
    [SerializeField] float orbitSpeed;
    #endregion

    void Start() => player = FindObjectOfType<Player>();

    void Update() => Orbit(orbitSpeed);

    void Orbit(float orbitSpeed) =>
        // Rotate the weapon around the player.
        transform.RotateAround(point.position, Vector3.forward, orbitSpeed * Time.deltaTime);
}