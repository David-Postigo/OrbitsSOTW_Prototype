
using UnityEngine;


// en este script para el sistema de raycast y acercamiento de la camara he utlizado lo que aprendimos en clase, el sistema de pivot me lo ha explicado la ia con el lookat.
public class CameraMovement : MonoBehaviour
{
    public Transform pivot;
    public float distance = 3f;
    public float collisionOffset = 0.05f;
    public LayerMask obstacleMask = ~0;

    private Vector3 hitPosition;


    //sistema de rotacion y raycast para orbitar la camara y fijarse en el pivot, con un sistema de colision para que la camara no atraviese paredes.
    void LateUpdate()
    {
        if (pivot == null)
            return;

        Vector3 direction = -(pivot.rotation * Vector3.forward);

        float currentDistance = distance;

        RaycastHit hit;

        Debug.DrawRay(pivot.position, direction * distance, Color.red);

        if (Physics.Raycast(pivot.position, direction, out hit, distance, obstacleMask, QueryTriggerInteraction.Ignore))
        {
            currentDistance = Mathf.Max(0.1f, hit.distance - collisionOffset);
            Debug.DrawRay(pivot.position, direction * hit.distance, Color.green);
        }


        hitPosition = pivot.position + direction * currentDistance;

        transform.position = hitPosition;

        transform.LookAt(pivot.position);
    }

    //Sistema de comprovacion visual para comprobar que el raycast funciona correctamente, dibujando una esfera roja en el pivot y una esfera verde en la posicion del hit. como en clase.
    private void OnDrawGizmosSelected()
    {
        if (pivot == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(pivot.position, 0.1f);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(hitPosition, 0.1f);
    }
}