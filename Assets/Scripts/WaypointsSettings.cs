using UnityEngine;

public class WaypointsSettings : MonoBehaviour
{
    [Header("Instrucciones para la IA")]
    [Tooltip("Velocidada/aceleracion a la que deberia ir hacia el siguiente waypoint")]
    public float targetAcceleration = 0.5f;

    [Tooltip("Es una curva la requiera frenar?")]
    public bool requiresBraking = false;

    [Tooltip("Si tiene que frenar, a cuanto metros debe arrancar a frenar")]
    public float brakingDistance = 5f;
}
