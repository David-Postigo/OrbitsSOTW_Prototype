
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    [Header("Punto de referencia")]
    public Transform puntoCamara;

    [Header("Configuracion de camara")]
    public float separacionCamara = 3f;
    public float margenColision = 0.05f;
    public LayerMask capasObstaculos = ~0;

    [Header("Zoom")]
    public float sensibilidadZoom = 0.01f;
    public float distanciaMinima = 1f;
    public float distanciaMaxima = 8f;

    [Header("Movimiento del raton")]
    public float sensibilidadRaton = 0.15f;
    public float anguloMinimo = -80f;
    public float anguloMaximo = 80f;

    private InputSystem_Actions acciones;

    private float rotacionHorizontal;
    private float rotacionVertical;

    private float distanciaActual;
    private Vector3 posicionCamara;

    private bool cursorBloqueado = true;

    void Awake()
    {
        acciones = new InputSystem_Actions();
        acciones.Enable();

        BloquearCursor();
    }

    void Start()
    {
        if (puntoCamara == null)
        {
            Debug.LogError(
                "No has asignado el punto de referencia de la camara."
            );

            enabled = false;
            return;
        }

        Vector3 angulos = puntoCamara.eulerAngles;

        rotacionHorizontal = angulos.y;
        rotacionVertical = angulos.x;

        if (rotacionVertical > 180f)
            rotacionVertical -= 360f;

        distanciaActual = Mathf.Clamp(
            separacionCamara,
            distanciaMinima,
            distanciaMaxima
        );
    }

    void Update()
    {
        // Alternar bloqueo del cursor con ESC.
        if (Keyboard.current != null &&
            Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (cursorBloqueado)
                DesbloquearCursor();
            else
                BloquearCursor();
        }

        // Zoom con la rueda del raton.
        if (cursorBloqueado && Mouse.current != null)
        {
            float scroll = Mouse.current.scroll.ReadValue().y;

            if (Mathf.Abs(scroll) > 0.01f)
            {
                distanciaActual -= scroll * sensibilidadZoom;

                distanciaActual = Mathf.Clamp(
                    distanciaActual,
                    distanciaMinima,
                    distanciaMaxima
                );
            }
        }
    }

    void LateUpdate()
    {
        if (puntoCamara == null)
            return;

        // Solo mover la camara cuando el cursor esta bloqueado.
        if (cursorBloqueado)
        {
            Vector2 movimientoRaton =
                acciones.Player.Look.ReadValue<Vector2>();

            rotacionHorizontal +=
                movimientoRaton.x * sensibilidadRaton;

            rotacionVertical -=
                movimientoRaton.y * sensibilidadRaton;

            rotacionVertical = Mathf.Clamp(
                rotacionVertical,
                anguloMinimo,
                anguloMaximo
            );
        }

        Quaternion rotacion = Quaternion.Euler(
            rotacionVertical,
            rotacionHorizontal,
            0f
        );

        Vector3 direccionCamara =
            -(rotacion * Vector3.forward);

        float distanciaFinal = distanciaActual;

        RaycastHit impacto;

        if (Physics.Raycast(
            puntoCamara.position,
            direccionCamara,
            out impacto,
            distanciaActual,
            capasObstaculos,
            QueryTriggerInteraction.Ignore))
        {
            distanciaFinal = Mathf.Max(
                0.1f,
                impacto.distance - margenColision
            );
        }

        posicionCamara = puntoCamara.position +
                         direccionCamara * distanciaFinal;

        transform.position = posicionCamara;
        transform.LookAt(puntoCamara.position);
    }

    void BloquearCursor()
    {
        cursorBloqueado = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void DesbloquearCursor()
    {
        cursorBloqueado = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void OnDisable()
    {
        DesbloquearCursor();
    }

    void OnDestroy()
    {
        if (acciones != null)
        {
            acciones.Disable();
            acciones.Dispose();
        }

        DesbloquearCursor();
    }

    void OnDrawGizmosSelected()
    {
        if (puntoCamara == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(puntoCamara.position, 0.1f);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(posicionCamara, 0.1f);
    }
}
