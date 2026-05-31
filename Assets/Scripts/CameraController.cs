using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Objetivos")]
    public Transform target;
    public Rigidbody carRigidbody;

    [Header("Posicionamiento")]
    public float distance = 6.0f;
    public float height = 2.5f;
    public float followSpeed = 10.0f;
    public float rotationSpeed = 10.0f;

    
    [Header("Vista Lateral (Look Around)")]
    [Tooltip("Ángulo máximo que la cámara rotará hacia los lados")]
    public float lookAngleAmount = 70.0f;
    [Tooltip("Velocidad con la que la cámara gira hacia el lado")]
    public float lookSmoothness = 5.0f;
    
    
    private float currentLookAngle = 0f;
    

    [Header("Efectos de Velocidad (FOV)")]
    public Camera cam;
    public float minFOV = 60f;
    public float maxFOV = 85f;
    public float maxSpeed = 50f;

    [Header("Vibración de Cámara (Shake)")]
    public float shakeSpeedThreshold = 30f;
    public float maxShakeAmount = 0.1f;

    void Start()
    {
        if (cam == null) cam = GetComponent<Camera>();
    }

    void Update()
    {
        
        float targetLookAngle = 0f;
        
        float inputX = Input.GetAxis("Horizontal");
        
        targetLookAngle = inputX * lookAngleAmount;

        currentLookAngle = Mathf.Lerp(currentLookAngle, targetLookAngle, Time.deltaTime * lookSmoothness);
        
    }

    void FixedUpdate()
    {
        if (target == null) return;

        
        float forwardSpeed = 0f;
        float currentSpeed = 0f;

        if (carRigidbody != null)
        {
            forwardSpeed = Vector3.Dot(carRigidbody.velocity, target.forward);
            currentSpeed = carRigidbody.velocity.magnitude;
        }

      
        
       
        Vector3 direction = (forwardSpeed < -1.0f) ? target.forward : -target.forward;

        // --- ROTACIÓN LATERAL ---
        // Creamos una rotación alrededor del eje vertical (Y) del auto
        Quaternion lookRotation = Quaternion.AngleAxis(currentLookAngle, target.up);
        
        // Rotamos el vector de dirección básica usando la rotación lateral
        Vector3 rotatedDirection = lookRotation * direction;

        // Calculamos la posición final deseada
        Vector3 targetPosition = target.position + (rotatedDirection * distance) + (Vector3.up * height);
        // ----------------------------------------------------------------------

        // Movimiento y Rotación Suave
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.fixedDeltaTime);

        // La rotación siempre mira hacia el auto, pero ahora desde un lado
        Vector3 directionToLook = target.position - transform.position;
        if (directionToLook != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToLook);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }

        // Efectos Visuales (FOV y Shake)
        if (carRigidbody != null && cam != null)
        {
            // FOV
            float speedRatio = Mathf.Clamp01(currentSpeed / maxSpeed);
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, Mathf.Lerp(minFOV, maxFOV, speedRatio), Time.fixedDeltaTime * 2f);

            // Shake
            if (currentSpeed > shakeSpeedThreshold)
            {
                float shakeRatio = Mathf.Clamp01((currentSpeed - shakeSpeedThreshold) / (maxSpeed - shakeSpeedThreshold));
                float currentShakeAmount = maxShakeAmount * shakeRatio;
                transform.position += Random.insideUnitSphere * currentShakeAmount;
            }
        }
    }
}