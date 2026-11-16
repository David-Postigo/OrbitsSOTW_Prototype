using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public float sideSpeed = 0.75f;
    public float backSpeed = 0.5f;
    public float airSpeed = 0.5f;

    [Header("Sprint")]
    public float sprintSpeed = 2f;

    InputSystem_Actions inputs;

    void Start()
    {
        inputs = new InputSystem_Actions();
        inputs.Enable();
    }

    void FixedUpdate()
    {
        Vector2 input = inputs.Player.Move.ReadValue<Vector2>();

        Vector3 dir = new Vector3(input.x, 0f, input.y);

        // Convertir la dirección local a dirección del jugador
        dir = transform.TransformDirection(dir);

        // Velocidad según la dirección
        float currentSpeed = speed;

        if (input.y < 0)
        {
            currentSpeed = speed * backSpeed;
        }
        else if (input.x != 0 && input.y <= 0)
        {
            currentSpeed = speed * sideSpeed;
        }
        else if (input.y == 0 && input.x != 0)
        {
            currentSpeed = speed * sideSpeed;
        }

        // Comprobar si está en el suelo
        bool isgrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);

        // Movimiento en el aire al 50%
        if (!isgrounded)
        {
            currentSpeed *= airSpeed;
        }
        else
        {
            // Sprint solo hacia adelante o diagonal hacia adelante
            if (inputs.Player.Sprint.IsPressed() && input.y > 0)
            {
                currentSpeed = speed * sprintSpeed;
            }
        }

        // Evitar que el movimiento diagonal sea más rápido
        if (dir.magnitude > 1f)
        {
            dir.Normalize();
        }

        // Mover al jugador
        transform.position += dir * currentSpeed * Time.deltaTime;
    }
}