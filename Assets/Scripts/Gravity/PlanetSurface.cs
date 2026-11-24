
using UnityEngine;

public class PlanetSurface : MonoBehaviour
{
    [Header("Configuracion del planeta")]
    [Min(0f)]
    public float planetGravity = 9.81f;

    [Min(0f)]
    public float gravityRadius = 100f;

    [Min(0f)]
    public float maxDistance = 150f;
}
