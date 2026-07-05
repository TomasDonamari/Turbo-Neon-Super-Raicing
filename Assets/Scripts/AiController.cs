using UnityEngine;

[RequireComponent(typeof(Car))]
[RequireComponent(typeof(CheckpointManager))]
public class AiController : MonoBehaviour
{
    public Circuit circuit;
    Car car;
    
    private CheckpointManager checkpointTracker; 

    public float steerSensitivity = 0.1f;
    Vector3 target;
    int currentWayPoint = 0;

    //  VARIABLES PARA EL RESPAWN 
    private float lastTimeMoving = 0f;
    private Vector3 posicionInicial;
    private Quaternion rotacionInicial;
    
    void Start()
    {
        car = this.GetComponent<Car>();
        checkpointTracker = GetComponent<CheckpointManager>(); 
        
        // Guardamos posición inicial por si falla todo
        posicionInicial = transform.position;
        rotacionInicial = transform.rotation;
        
        target = circuit.wayPoints[currentWayPoint].transform.position;
    }

    void Update()
    {
        if (RaceMonitor.racing == false) return;
        
        if (checkpointTracker.lap >= 2)
        {
            car.SetInputs(0f, 0f, true);
            return; 
        }

        Vector3 localTarget = car.transform.InverseTransformPoint(target);
        float distanceToTarget = Vector3.Distance(target, car.transform.position);

        float targetAngle = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;
        float localForwardVelocity = car.transform.InverseTransformDirection(car.carRB.linearVelocity).z;
        float steer = Mathf.Clamp(targetAngle * steerSensitivity, -1f, 1f) * Mathf.Sign(localForwardVelocity);
        
        Transform currentWP = circuit.wayPoints[currentWayPoint].transform;
        WaypointsSettings wpSettings = currentWP.GetComponent<WaypointsSettings>();

        float acceleration = 0.5f;
        bool isBraking = false;

        if (wpSettings != null)
        {
            acceleration = wpSettings.targetAcceleration;

            if (wpSettings.requiresBraking && distanceToTarget < wpSettings.brakingDistance)
            {
                isBraking = true;
                acceleration = 0.1f; 
            }
            
            if (checkpointTracker.lap == 2)
            {
                acceleration += 0.2f; 
            }
        }
        else 
        {
            if (distanceToTarget < 10f) acceleration = 0.2f;
        }

        car.SetInputs(acceleration, steer, isBraking);

        if (distanceToTarget < 6f) 
        {
            currentWayPoint++;
            if (currentWayPoint >= circuit.wayPoints.Length)
            {
                currentWayPoint = 0;
            }
            target = circuit.wayPoints[currentWayPoint].transform.position;
        }

        
        // --- LÓGICA DE RESPAWN DE LA IA ---
        
        // Chequeamos si el auto se está moviendo usando linearVelocity 
        if (car.carRB.linearVelocity.magnitude > 1f)
        {
            lastTimeMoving = Time.time;
        }

        // Si pasaron 4 segundos estancado
        if (Time.time > lastTimeMoving + 4f)
        {
            // Teletransportamos al último checkpoint (o al inicio)
            if (checkpointTracker.ultimoWaypoint != null)
            {
                transform.position = checkpointTracker.ultimoWaypoint.position;
                transform.rotation = checkpointTracker.ultimoWaypoint.rotation;
            }
            else
            {
                transform.position = posicionInicial;
                transform.rotation = rotacionInicial;
            }
            
            // Frenamos el auto para limpiarle la inercia
            car.carRB.linearVelocity = Vector3.zero;
            car.carRB.angularVelocity = Vector3.zero;

            // Recalculamos el target para que no intente volver atrás
            ActualizarTargetMasCercano();

            lastTimeMoving = Time.time; 
        }
    }

    // FUNCIÓN PARA REUBICAR A LA IA EN SU RUTA 
    private void ActualizarTargetMasCercano()
    {
        float minimaDistancia = Mathf.Infinity;
        int indiceMasCercano = 0;

        // Buscamos cuál es el waypoint del circuito que está más cerca de donde reaparecimos
        for (int i = 0; i < circuit.wayPoints.Length; i++)
        {
            float distancia = Vector3.Distance(transform.position, circuit.wayPoints[i].transform.position);
            if (distancia < minimaDistancia)
            {
                minimaDistancia = distancia;
                indiceMasCercano = i;
            }
        }

        // Asignamos ese waypoint como el nuevo objetivo inmediato
        currentWayPoint = indiceMasCercano;
        target = circuit.wayPoints[currentWayPoint].transform.position;
    }
}