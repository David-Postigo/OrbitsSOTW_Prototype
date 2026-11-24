
using UnityEngine;

public class PlayerCheckpoint : MonoBehaviour
{
    private bool hasCheckpoint;

    private Vector3 checkpointPosition;
    private Quaternion checkpointRotation;
    private PlanetSurface checkpointPlanet;

    public bool HasCheckpoint()
    {
        return hasCheckpoint;
    }

    public void SetCheckpoint(
        Vector3 position,
        Quaternion rotation,
        PlanetSurface planet)
    {
        if (planet == null)
            return;

        checkpointPosition = position;
        checkpointRotation = rotation;
        checkpointPlanet = planet;

        hasCheckpoint = true;

        Debug.Log("Checkpoint actualizado: " + planet.name);
    }

    public void Respawn(PlanetGravity player)
    {
        if (!hasCheckpoint || player == null)
            return;

        player.TeleportToCheckpoint(
            checkpointPosition,
            checkpointRotation,
            checkpointPlanet
        );

        Debug.Log("Jugador respawneado en: " + checkpointPlanet.name);
    }
}
