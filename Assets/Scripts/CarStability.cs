using UnityEngine;

public class CarStability : MonoBehaviour
{
    [Header("Componentes")]
    [Tooltip("El Rigidbody de tu auto")]
    public Rigidbody carRigidbody;
    
    [Tooltip("GameObject CentroDeMasa del vehículo")]
    public Transform customCenterOfMass;

    [Header("Estabilidad")]
    [Tooltip("Fuerza invisible que empuja el auto hacia abajo. Auméntar en caso de que el auto resbale mucho.")]
    public float downforceMultiplier = 50f;

    void Start()
    {
        if (carRigidbody == null) carRigidbody = GetComponent<Rigidbody>();

        if (customCenterOfMass != null)
        {
            carRigidbody.centerOfMass = customCenterOfMass.localPosition;
        }
    }

    void FixedUpdate()
    {
        // Aplicar Downforce basado en la velocidad
        // A mayor velocidad (magnitude), más fuerte lo empujamos contra el suelo (-transform.up)
        float currentSpeed = carRigidbody.linearVelocity.magnitude;
        
        // Usamos -transform.up para que empuje "hacia el piso del auto" incluso si está en una rampa
        carRigidbody.AddForce(-transform.up * downforceMultiplier * currentSpeed);
    }
}
