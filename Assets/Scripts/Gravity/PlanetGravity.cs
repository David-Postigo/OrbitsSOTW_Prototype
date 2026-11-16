
using UnityEngine;

public class PlanetGravity : MonoBehaviour
{
    public Transform planet;

    public float gravity = 9.81f;
    public float rotationSpeed = 5f;

    private Vector3 gravityDirection;

    void Update()
    {
        if (planet == null)
            return;

        // Calculamos la direccion hacia el centro del planeta.
        gravityDirection = (planet.position - transform.position).normalized;

        // Aplicamos gravedad hacia el centro del planeta.
        GetComponent<Rigidbody>().AddForce(
            gravityDirection * gravity,
            ForceMode.Acceleration
        );

        // Calculamos la orientacion del personaje respecto al planeta.
        Quaternion targetRotation = Quaternion.FromToRotation(
            transform.up,
            -gravityDirection
        ) * transform.rotation;

        // Giramos el personaje para adaptarlo a la superficie.
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }
}
