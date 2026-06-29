using UnityEngine;

[RequireComponent(typeof(Car))]
[RequireComponent(typeof(CheckpointManager))]
public class AiController : MonoBehaviour
{
    public Circuit circuit;
    Car car;
    
    // Variable para guardar la referencia al manager de checkpoints
    private CheckpointManager checkpointTracker; 

    public float steerSensitivity = 0.1f;
    Vector3 target;
    int currentWayPoint = 0;
    
    void Start()
    {
        car = this.GetComponent<Car>();
        
        // Al iniciar, buscamos el CheckpointManager adjunto a este mismo auto
        checkpointTracker = GetComponent<CheckpointManager>(); 
        
        target = circuit.wayPoints[currentWayPoint].transform.position;
    }

    void Update()
    {
        if (RaceMonitor.racing == false) return;
        
        // EJEMPLO DE USO: Si la IA ya completó 3 vueltas (la carrera terminó)
        if (checkpointTracker.lap >= 2)
        {
            // Le decimos al auto que deje de acelerar, no doble y pise el freno a fondo
            car.SetInputs(0f, 0f, true);
            return; // Cortamos la ejecución del Update para que no siga buscando waypoints
        }

        Vector3 localTarget = car.transform.InverseTransformPoint(target);
        float distanceToTarget = Vector3.Distance(target, car.transform.position);

        float targetAngle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;
        float localForwardVelocity = car.transform.InverseTransformDirection(car.carRB.linearVelocity).z;
        float steer = Mathf.Clamp(targetAngle * steerSensitivity, -1f, 1f) * Mathf.Sign(localForwardVelocity);
        
        // Waypoint actual al que nos dirigimos
        Transform currentWP = circuit.wayPoints[currentWayPoint].transform;
        
        // Buscamos el script "WaypointSettings" en el waypoint actual
        WaypointsSettings wpSettings = currentWP.GetComponent<WaypointsSettings>();

        // Valores por defecto
        float acceleration = 0.5f;
        bool isBraking = false;

        // Si el waypoint tiene el script, usamos sus valores personalizados
        if (wpSettings != null)
        {
            // Tomamos la aceleración ideal para este tramo
            acceleration = wpSettings.targetAcceleration;

            // Revisamos si esta curva en particular necesita que toquemos el freno
            if (wpSettings.requiresBraking && distanceToTarget < wpSettings.brakingDistance)
            {
                isBraking = true;
                acceleration = 0.1f; 
            }
            
            // 4. OTRO EJEMPLO: Si es la última vuelta (vuelta 2, ya que empieza en 0), la IA acelera más
            if (checkpointTracker.lap == 2)
            {
                acceleration += 0.2f; // Le damos un "boost" de desesperación
            }
        }
        else 
        {
            // Comportamiento viejo de respaldo por si no hay script
            if (distanceToTarget < 10f) acceleration = 0.2f;
        }

        // ---------------------------------------------

        // Enviamos las instrucciones al motor del auto
        car.SetInputs(acceleration, steer, isBraking);

        // Cambio al siguiente waypoint
        if (distanceToTarget < 6f) 
        {
            currentWayPoint++;
            if (currentWayPoint >= circuit.wayPoints.Length)
            {
                currentWayPoint = 0;
            }
            target = circuit.wayPoints[currentWayPoint].transform.position;
        }
    }
}