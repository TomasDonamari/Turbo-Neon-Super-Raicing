using UnityEngine;

public class CheckpointManager : MonoBehaviour 
{
    public int lap = 0; 
    public int nextCheckpoint = 0;
    private int maxCheckpointNumber = 0;
    public int vueltasParaTerminar = 2;
    public bool esJugador = false;

    private float tiempoUltimoCheckpoint = 0f;

    void Start()
    {
        GameObject[] checkpoints = GameObject.FindGameObjectsWithTag("Checkpoint");
        foreach (GameObject cp in checkpoints)
        {
            if (int.TryParse(cp.name, out int number))
            {
                if (number > maxCheckpointNumber)
                {
                    maxCheckpointNumber = number;
                }
            }
        }
        
        // MENSAJE INICIAL
        Debug.Log($"<color=cyan>[INICIO]</color> Encontré {checkpoints.Length} checkpoints. El número máximo en la pista es el {maxCheckpointNumber}");
    }

    private void OnTriggerEnter(Collider other) 
    {
        if (other.CompareTag("Checkpoint"))
        {

            if (int.TryParse(other.gameObject.name, out int thisCPNumber))
            {
                if (thisCPNumber == nextCheckpoint)
                {
                    tiempoUltimoCheckpoint = Time.time;
                    
                    // MENSAJE DE ÉXITO
                    Debug.Log($"<color=green>[BIEN]</color> Cruzaste el Checkpoint {thisCPNumber}");

                    if (thisCPNumber == 0) 
                    {
                        lap++; 
                        Debug.Log($"<color=yellow>[VUELTA]</color> Tienes {lap} vueltas registradas.");
                        if (lap >= vueltasParaTerminar && !esJugador)
                        {
                        RaceMonitor.AvisarIAFinalizo();
                        }
                    }

                    nextCheckpoint++;
                    if (nextCheckpoint > maxCheckpointNumber)
                    {
                        nextCheckpoint = 0; 
                        Debug.Log("<color=orange>[INFO]</color> Circuito completado, esperando cruzar la meta (0) otra vez.");
                    }
                }
                else 
                {
                    // MENSAJE DE ERROR (Aquí estará el problema)
                    Debug.LogWarning($"<color=red>[ERROR]</color> Tocaste el Checkpoint {thisCPNumber}, pero el sistema estaba esperando que toques el {nextCheckpoint}");
                }
            }
        }
    }
}