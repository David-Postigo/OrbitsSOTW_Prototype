
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Planeta de este checkpoint")]
    public PlanetSurface planet;

    [Header("Punto de respawn")]
    public Transform respawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        PlayerCheckpoint playerCheckpoint =
            other.GetComponentInParent<PlayerCheckpoint>();

        if (playerCheckpoint == null)
            return;

        if (planet == null)
        {
            Debug.LogWarning("Checkpoint sin planeta asignado: " + name);
            return;
        }

        Transform point = respawnPoint != null
            ? respawnPoint
            : transform;

        playerCheckpoint.SetCheckpoint(
            point.position,
            point.rotation,
            planet
        );
    }
}
