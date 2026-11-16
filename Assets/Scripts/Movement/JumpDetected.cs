
using UnityEngine;

public class JumpDetected : MonoBehaviour
{
    [Header("Ground Detection")]
    public bool isgrounded;
    public float distance = 0.2f;
    public LayerMask layerMask;
    public Vector3 offset = new Vector3(0f, -0.9f, 0f);
    public float radius = 0.25f;

    [Header("Jump height")]
    public float jumpForce = 7f;

    InputSystem_Actions inputActions;
    Rigidbody rb;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (inputActions.Player.Jump.WasPressedThisFrame() && isgrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            isgrounded = false;
        }
    }

    private void FixedUpdate()
    {
        Vector3 checkPosition = transform.position + offset;

        isgrounded = Physics.CheckSphere(checkPosition, radius, layerMask, QueryTriggerInteraction.Ignore);
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 checkPosition = transform.position + offset;

        Gizmos.color = isgrounded ? Color.green : Color.red;

        Gizmos.DrawWireSphere(checkPosition, radius);
    }
}