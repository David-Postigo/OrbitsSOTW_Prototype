
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlanetGravity : MonoBehaviour
{
    public Transform planet;

    [Header("Gravedad")]
    public float gravity = 9.81f;
    public float rotationSpeed = 5f;

    [Header("Limites")]
    public float gravityRadius = 100f;
    public float maxDistance = 150f;

    private InputSystem_Actions inputActions;

    private Rigidbody rb;
    private bool invertedGravity = false;
    private bool insideGravityRadius;

    private Vector3 gravityDirection;

    // Posicion inicial del jugador.
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        inputActions = new InputSystem_Actions();
        inputActions.Enable();

        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    void Update()
    {
        if (planet == null)
            return;

        float distance = Vector3.Distance(
            transform.position,
            planet.position
        );

        // Reiniciar si supera la distancia maxima.
        if (distance > maxDistance)
        {
            ResetPlayer();
            return;
        }

        insideGravityRadius = distance <= gravityRadius;

        // Invertir gravedad.
        if (inputActions.Player.Gravity.WasPressedThisFrame())
        {
            invertedGravity = !invertedGravity;
        }
    }

    void FixedUpdate()
    {
        if (planet == null || !insideGravityRadius)
            return;

        gravityDirection = (
            planet.position - transform.position
        ).normalized;

        if (invertedGravity)
        {
            gravityDirection = -gravityDirection;
        }

        rb.AddForce(
            gravityDirection * gravity,
            ForceMode.Acceleration
        );

        Quaternion targetRotation = Quaternion.FromToRotation(
            transform.up,
            -gravityDirection
        ) * transform.rotation;

        rb.MoveRotation(
            Quaternion.Slerp(
                rb.rotation,
                targetRotation,
                rotationSpeed * Time.fixedDeltaTime
            )
        );
    }

    void ResetPlayer()
    {
        // Reiniciar posicion y rotacion.
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.position = initialPosition;
        rb.rotation = initialRotation;

        // Recuperar gravedad normal.
        invertedGravity = false;
    }

    void OnDestroy()
    {
        inputActions.Disable();
        inputActions.Dispose();
    }
}
