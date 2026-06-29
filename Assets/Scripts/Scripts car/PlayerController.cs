using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; 

[RequireComponent(typeof(Car))]
[RequireComponent(typeof(CheckpointManager))] 
public class PlayerController : MonoBehaviour
{
    private Car carScript;
    private CheckpointManager checkpointTracker; 
    
    public TextMeshProUGUI textoVueltas; 

    // VARIABLES PARA EL FIN DE CARRERA
    [Header("Configuración de Carrera")]
    public int vueltasParaTerminar = 2; // Cantidad de vueltas totales
    public GameObject panelFinCarrera;  // El panel UI que aparecerá al final
    
    private bool carreraTerminada = false;

    private void Awake()
    {
        carScript = GetComponent<Car>();
        checkpointTracker = GetComponent<CheckpointManager>(); 
    }

    private void Start()
    {
        // Nos aseguramos de que la pantalla final esté oculta al empezar a jugar
        if (panelFinCarrera != null)
        {
            panelFinCarrera.SetActive(false);
        }
    }

    private void Update()
    {
        // Como 'lap' suma 1 en la largada, para hacer 2 vueltas físicas 'lap' debe llegar a 3.
        // Por lo tanto, si pedimos 2 vueltas, terminamos cuando lap > 2.
        if (checkpointTracker.lap > vueltasParaTerminar && !carreraTerminada)
        {
            TerminarCarrera();
        }

        // LÓGICA DE MOVIMIENTO
        float forwardInput = 0f;
        float turnInput = 0f;
        bool brakeInput = false;

        // Solo se puede manejar si la carrera empezó Y no ha terminado
        if (RaceMonitor.racing && !carreraTerminada) 
        {
            forwardInput = Input.GetAxisRaw("Vertical");
            turnInput = Input.GetAxis("Horizontal");
            brakeInput = Input.GetKey(KeyCode.Space);
        }
        else if (carreraTerminada)
        {
            // Si la carrera terminó, clavamos los frenos para que el auto no siga de largo
            brakeInput = true; 
        }

        carScript.SetInputs(forwardInput, turnInput, brakeInput);

        if (textoVueltas != null && checkpointTracker != null && !carreraTerminada)
        {
            // Mathf.Clamp evita que en la pantalla diga "Vuelta 3 / 2" justo antes de cambiar de escena
            int vueltaMostrar = Mathf.Clamp(checkpointTracker.lap, 1, vueltasParaTerminar);
            textoVueltas.text = "Vuelta: " + vueltaMostrar.ToString() + " / " + vueltasParaTerminar; 
        }
    }

    // Función que se ejecuta una sola vez al terminar
    private void TerminarCarrera()
    {
        carreraTerminada = true;
        
        // Activamos el panel gráfico
        if (panelFinCarrera != null)
        {
            panelFinCarrera.SetActive(true);
        }
    }

    // FUNCIÓN PÚBLICA PARA EL BOTÓN
    public void VolverAlMenu()
    {
        SceneManager.LoadScene("MenuPrincipal"); 
    }
}