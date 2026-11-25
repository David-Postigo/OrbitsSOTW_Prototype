
using UnityEngine;

public class JumpDetected : MonoBehaviour
{
    [Header("Planet")]
    public Transform planet;

    [Header("Ground Detection")]
    public bool isgrounded;
    public float radius = 0.3f;
    public float groundDistance = 1.1f;
    public LayerMask layerMask;

    [Header("Jump")]
    public float jumpForce = 7f;
    public float jumpCooldown = 1f;

    private float nextJumpTime;

    private InputSystem_Actions inputActions;
    private Rigidbody rb;

    private Vector3 gravityDirection;
    private Vector3 groundPosition;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        inputActions = new InputSystem_Actions();
        inputActions.Enable();

        nextJumpTime = 0f;
    }

    private void Update()
    {
        if (planet == null)
            return;

        if (inputActions.Player.Jump.WasPressedThisFrame() &&
            isgrounded &&
            Time.time >= nextJumpTime)
        {
            // Direccion opuesta a la gravedad.
            Vector3 jumpDirection = (
                transform.position - planet.position
            ).normalized;

            // Eliminar velocidad hacia el suelo.
            float velocityAlongJump = Vector3.Dot(
                rb.linearVelocity,
                jumpDirection
            );

            if (velocityAlongJump < 0f)
            {
                rb.linearVelocity -= jumpDirection * velocityAlongJump;
            }

            // Saltar.
            rb.AddForce(
                jumpDirection * jumpForce,
                ForceMode.Impulse
            );

            isgrounded = false;

            // Activar cooldown.
            nextJumpTime = Time.time + jumpCooldown;
        }
    }

    private void FixedUpdate()
    {
        if (planet == null)
            return;

        // Direccion desde el centro hacia el jugador.
        gravityDirection = (
            transform.position - planet.position
        ).normalized;

        // Posicion debajo de los pies.
        groundPosition = transform.position -
                         gravityDirection * groundDistance;

        // Comprobar si hay suelo.
        isgrounded = Physics.CheckSphere(
            groundPosition,
            radius,
            layerMask,
            QueryTriggerInteraction.Ignore
        );
    }

    private void OnDrawGizmosSelected()
    {
        if (planet == null)
            return;

        Vector3 direction = (
            transform.position - planet.position
        ).normalized;

        Vector3 checkPosition = transform.position -
                               direction * groundDistance;

        Gizmos.color = isgrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(checkPosition, radius);
    }

    private void OnDestroy()
    {
        if (inputActions != null)
        {
            inputActions.Disable();
            inputActions.Dispose();
        }
    }
}
