
using UnityEngine;

public class TeleportPoint : MonoBehaviour
{
    [Header("Destino")]
    public Transform destination;

    [Header("Planeta de destino")]
    public PlanetSurface destinationPlanet;

    [Header("Configuracion")]
    public bool updateCheckpoint = true;

    private void OnTriggerEnter(Collider other)
    {
        PlanetGravity player =
            other.GetComponentInParent<PlanetGravity>();

        if (player == null)
            return;

        if (destination == null || destinationPlanet == null)
        {
            Debug.LogWarning(
                "TeleportPoint sin destino o planeta asignado: " + name
            );
            return;
        }

        player.TeleportTo(destination, destinationPlanet);

        if (updateCheckpoint)
        {
            PlayerCheckpoint checkpoint =
                player.GetComponent<PlayerCheckpoint>();

            if (checkpoint != null)
            {
                checkpoint.SetCheckpoint(
                    destination.position,
                    destination.rotation,
                    destinationPlanet
                );
            }
        }
    }
}
