using System.Collections;
using UnityEngine;

public class RaceMonitor : MonoBehaviour
{
    public GameObject[] countDownItems;
    
    // Esta variable sigue siendo static, lo cual es útil, pero la reiniciaremos manualmente
    public static bool racing = false;
    public static bool primeraIATermino = false;
    
    private void Awake()
    {
        // Al cargar la escena, forzamos a que la carrera empiece pausada.
        // Esto garantiza que la IA y el Jugador esperen la cuenta regresiva.
        racing = false; 
        primeraIATermino = false;
    }
    
    public static void AvisarIAFinalizo()
        {
            primeraIATermino = true;
        }
    void Start()
    {
        foreach (GameObject g in countDownItems)
        {
            g.SetActive(false);
        }
        StartCoroutine(PlayCountDown());
    }

    IEnumerator PlayCountDown()
    {
        yield return new WaitForSeconds(5);
        foreach (GameObject g in countDownItems)
        {
            g.SetActive(true);
            yield return new WaitForSeconds(1);
            g.SetActive(false);
        }
        // La carrera comienza ahora para todos al mismo tiempo
        racing = true;
    }
}
