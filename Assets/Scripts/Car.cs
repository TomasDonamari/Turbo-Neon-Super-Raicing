using UnityEngine;

public class Car : MonoBehaviour
{
   [Header("Físicas y Gravedad")]
    public Rigidbody carRB;
   

   [Header("Wheel Colliders")]
   public WheelCollider wheelFL;
   public WheelCollider wheelFR;
   public WheelCollider wheelRL;
   public WheelCollider wheelRR;


    [Header("Wheel Transforms")]
    public Transform wheelFLTransform;
    public Transform wheelFRTransform;
    public Transform wheelRLTransform;
    public Transform wheelRRTransform;

    [Header("Car Settings")]
    public float motorForce = 1500f;
    public float brakeForce = 4000f;
    public float maxSteerAngle = 40;

    private float downforce = 50f;
    private float currentSteerAngle;
    private float currentBrakeForce;
    private bool isBraking;
    private float verticalInput;
    private float horizontalInput;

    private void FixedUpdate()
    {
        HandleMotor();
        HandleSteering(); 
        UpdateWheels();
        AddDownforce();
    }


    private void HandleMotor()
    {
        float verticalInput = Input.GetAxisRaw("Vertical");
        isBraking = Input.GetKey(KeyCode.Space);

        
        if (isBraking)
        {
            // fuerza del motor a CERO
            wheelRL.motorTorque = 0f;
            wheelRR.motorTorque = 0f;
            // Aplicamos toda la fuerza de los frenos
            currentBrakeForce = brakeForce;
        }
        // Si no aceleramos ni frenamos (Freno de motor suave)
        else if (verticalInput == 0)
        {
            wheelRL.motorTorque = 0f;
            wheelRR.motorTorque = 0f;
            currentBrakeForce = 300f; // Freno suave para que no ruede infinitamente
        }
        // Si estamos usando W o S (Acelerando o en Reversa)
        else
        {
            // Le damos potencia al motor y quitamos los frenos
            wheelRL.motorTorque = verticalInput * motorForce;
            wheelRR.motorTorque = verticalInput * motorForce;
            currentBrakeForce = 0f;
        }

        ApplyBraking();
    }
    
        private void ApplyBraking()
    {
        wheelFL.brakeTorque = currentBrakeForce;
        wheelFR.brakeTorque = currentBrakeForce;
        wheelRL.brakeTorque = currentBrakeForce;
        wheelRR.brakeTorque = currentBrakeForce;
}

    private void HandleSteering()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        
        // Calculamos la velocidad actual del auto
        float speed = carRB.velocity.magnitude;

        // Creamos un reductor. Mientras más alta es la velocidad, mayor es el reductor.
        float steeringReduction = 1f + (speed / 20f); 

        // 3. Dividimos el ángulo máximo por el reductor. 
        // Si vas lento, gira los 40 grados. Si vas rapidísimo, quizás solo gire 15 grados para no resbalar.
        currentSteerAngle = (maxSteerAngle / steeringReduction) * horizontalInput;

        wheelFL.steerAngle = currentSteerAngle;
        wheelFR.steerAngle = currentSteerAngle;
    }

    private void UpdateWheels(WheelCollider collider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        collider.GetWorldPose(out pos, out rot);
        wheelTransform.position = pos;
        wheelTransform.rotation = rot;
    }
    
 
    private void UpdateWheels()
    {
        UpdateWheels(wheelFL, wheelFLTransform);
        UpdateWheels(wheelFR, wheelFRTransform);
        UpdateWheels(wheelRL, wheelRLTransform);
        UpdateWheels(wheelRR, wheelRRTransform);
    }

    private void AddDownforce()
    {
        // Aplasta el auto contra el piso multiplicando la fuerza por la velocidad actual
        carRB.AddForce(-transform.up * downforce * carRB.velocity.magnitude);
    }

}
