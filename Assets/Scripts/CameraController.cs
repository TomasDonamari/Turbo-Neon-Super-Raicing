using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Objetos")]
    [Tooltip("Transform del vehículo")]
    public Transform target;
    [Tooltip("El Rigidbody del vehículo para calcular la velocidad")]
    public Rigidbody carRigidbody;

    [Header("Posicionamiento de la cámara")]
    public float distance = 6.0f;       // Distancia detrás del auto
    public float height = 2.5f;         // Altura de la cámara respecto al auto
    public float followSpeed = 10.0f;   // Qué tan rápido la cámara alcanza al auto (suavidad)
    public float rotationSpeed = 10.0f; // Qué tan rápido rota la cámara para mirar al auto

    [Header("(FOV)")]
    public Camera cam;
    public float minFOV = 60f;          // FOV cuando el auto está quieto
    public float maxFOV = 85f;          // FOV a máxima velocidad
    public float maxSpeed = 50f;        // Velocidad a la que se alcanza el maxFOV

    void Start()
    {
        
        if (cam == null)
        {
            cam = GetComponent<Camera>();
        }
    }

    
    void FixedUpdate()
    {
        if (target == null) return;

        //Calcular la posición (detrás y arriba del auto)
        Vector3 targetPosition = target.position - (target.forward * distance) + (Vector3.up * height);

        //Mover la cámara suavemente hacia esa posición (Lerp)
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.fixedDeltaTime);

        //Calcular y aplicar la rotación para mirar al auto
        Vector3 directionToLook = target.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(directionToLook);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);

        //Efecto de sensación de velocidad (Ajuste del FOV)
        if (carRigidbody != null && cam != null)
        {
            // Obtenemos la velocidad actual del auto
            float currentSpeed = carRigidbody.velocity.magnitude;
            
            // Calculamos un porcentaje de 0 a 1 basado en la velocidad máxima
            float speedRatio = Mathf.Clamp01(currentSpeed / maxSpeed);
            
            // Ajustamos el FOV suavemente
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, Mathf.Lerp(minFOV, maxFOV, speedRatio), Time.fixedDeltaTime * 2f);
        }
    }
}