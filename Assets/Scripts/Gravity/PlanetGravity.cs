
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlanetGravity : MonoBehaviour
{
    [Header("Planeta inicial")]
    public PlanetSurface currentPlanet;

    [Header("Movimiento")]
    public float rotationSpeed = 5f;

    [Header("Deteccion de planetas")]
    public float detectionRadius = 250f;
    public float searchInterval = 0.2f;
    public LayerMask planetLayer;

    private Rigidbody rb;
    private InputSystem_Actions inputActions;
    private PlayerCheckpoint playerCheckpoint;

    private bool invertedGravity;
    private bool resetting;
    private bool waitingForDestinationGravity;

    private float searchTimer;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private PlanetSurface initialPlanet;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        inputActions = new InputSystem_Actions();
        inputActions.Enable();

        playerCheckpoint = GetComponent<PlayerCheckpoint>();

        initialPosition = rb.position;
        initialRotation = rb.rotation;
        initialPlanet = currentPlanet;

        if (currentPlanet == null)
        {
            Debug.LogWarning("No hay planeta inicial asignado.");
        }
    }

    void Update()
    {
        if (inputActions.Player.Gravity.WasPressedThisFrame())
        {
            invertedGravity = !invertedGravity;
        }
    }

    void FixedUpdate()
    {
        if (resetting || currentPlanet == null)
            return;

        float distance = Vector3.Distance(
            rb.position,
            currentPlanet.transform.position
        );

        // Comprobar limite del planeta actual.
        if (distance > currentPlanet.maxDistance)
        {
            ResetPlayer();
            return;
        }

        // Esperar a entrar en el radio de gravedad del destino.
        if (waitingForDestinationGravity)
        {
            if (distance <= currentPlanet.gravityRadius)
            {
                waitingForDestinationGravity = false;

                Debug.Log("Gravedad del planeta activada.");
            }
            else
            {
                return;
            }
        }

        // Buscar otros planetas solamente cuando
        // el jugador ya esta dentro de la gravedad actual.
        searchTimer += Time.fixedDeltaTime;

        if (searchTimer >= searchInterval)
        {
            searchTimer = 0f;
            DetectPlanet();
        }

        if (currentPlanet == null)
            return;

        distance = Vector3.Distance(
            rb.position,
            currentPlanet.transform.position
        );

        if (distance > currentPlanet.maxDistance)
        {
            ResetPlayer();
            return;
        }

        if (distance > currentPlanet.gravityRadius)
            return;

        Vector3 gravityDirection =
            (currentPlanet.transform.position - rb.position).normalized;

        if (invertedGravity)
            gravityDirection = -gravityDirection;

        rb.AddForce(
            gravityDirection * currentPlanet.planetGravity,
            ForceMode.Acceleration
        );

        Quaternion targetRotation = Quaternion.FromToRotation(
            rb.rotation * Vector3.up,
            -gravityDirection
        ) * rb.rotation;

        rb.MoveRotation(
            Quaternion.Slerp(
                rb.rotation,
                targetRotation,
                rotationSpeed * Time.fixedDeltaTime
            )
        );
    }

    void DetectPlanet()
    {
        Collider[] colliders = Physics.OverlapSphere(
            rb.position,
            detectionRadius,
            planetLayer,
            QueryTriggerInteraction.Ignore
        );

        PlanetSurface closestPlanet = null;
        float closestDistance = float.MaxValue;

        foreach (Collider col in colliders)
        {
            PlanetSurface surface =
                col.GetComponentInParent<PlanetSurface>();

            if (surface == null || surface == currentPlanet)
                continue;

            float distance = Vector3.Distance(
                rb.position,
                surface.transform.position
            );

            if (distance <= surface.gravityRadius &&
                distance < closestDistance)
            {
                closestDistance = distance;
                closestPlanet = surface;
            }
        }

        if (closestPlanet != null)
        {
            ChangePlanet(closestPlanet);
        }
    }

    public void ChangePlanet(PlanetSurface newPlanet)
    {
        if (newPlanet == null)
            return;

        currentPlanet = newPlanet;
        invertedGravity = false;
        waitingForDestinationGravity = false;

        Debug.Log("Planeta actual: " + newPlanet.name);
    }

    public void TeleportTo(
        Transform destination,
        PlanetSurface newPlanet)
    {
        if (destination == null || newPlanet == null)
            return;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.position = destination.position;
        rb.rotation = destination.rotation;

        currentPlanet = newPlanet;
        invertedGravity = false;

        waitingForDestinationGravity = true;
        searchTimer = 0f;

        Physics.SyncTransforms();

        Debug.Log("Teletransporte realizado: " + newPlanet.name);
    }

    public void TeleportToCheckpoint(
        Vector3 position,
        Quaternion rotation,
        PlanetSurface newPlanet)
    {
        if (newPlanet == null)
            return;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.position = position;
        rb.rotation = rotation;

        currentPlanet = newPlanet;
        invertedGravity = false;

        waitingForDestinationGravity = true;
        searchTimer = 0f;

        Physics.SyncTransforms();

        Debug.Log("Respawn en checkpoint: " + newPlanet.name);
    }

    public void ResetPlayer()
    {
        if (resetting)
            return;

        resetting = true;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        if (playerCheckpoint != null &&
            playerCheckpoint.HasCheckpoint())
        {
            playerCheckpoint.Respawn(this);
        }
        else
        {
            ResetToInitialPosition();
        }

        invertedGravity = false;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        resetting = false;
    }

    public void ResetToInitialPosition()
    {
        rb.position = initialPosition;
        rb.rotation = initialRotation;

        currentPlanet = initialPlanet;
        invertedGravity = false;

        waitingForDestinationGravity = true;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        searchTimer = 0f;

        Physics.SyncTransforms();
    }

    void OnDestroy()
    {
        if (inputActions != null)
        {
            inputActions.Disable();
            inputActions.Dispose();
        }
    }
}
