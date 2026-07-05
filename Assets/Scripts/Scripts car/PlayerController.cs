using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; 

[RequireComponent(typeof(Car))]
[RequireComponent(typeof(CheckpointManager))]
[RequireComponent(typeof(Rigidbody))] 
public class PlayerController : MonoBehaviour
{
    private Car carScript;
    private CheckpointManager checkpointTracker;
    private Rigidbody rb; 

    float lastTimeMoving = 0; 
    public TextMeshProUGUI textoVueltas; 

    [Header("Configuración de Carrera")]
    public int vueltasParaTerminar = 2; 
    public GameObject panelFinCarrera;  
    
    private bool carreraTerminada = false;
    public GameObject panelPerdiste;

    // Guardamos una posición inicial de respaldo por si acaso
    private Vector3 posicionInicial;
    private Quaternion rotacionInicial;

    private void Awake()
    {
        carScript = GetComponent<Car>();
        checkpointTracker = GetComponent<CheckpointManager>(); 
        rb = GetComponent<Rigidbody>(); 
    }

    private void Start()
    {
        if (panelFinCarrera != null)
        {
            panelFinCarrera.SetActive(false);
        }

        posicionInicial = transform.position;
        rotacionInicial = transform.rotation;
    }

    private void Update()
    {
        if (checkpointTracker.lap > vueltasParaTerminar && !carreraTerminada)
        {
            TerminarCarrera();
        }

        // LÓGICA DE MOVIMIENTO
        float forwardInput = 0f;
        float turnInput = 0f;
        bool brakeInput = false;

        if (RaceMonitor.racing && !carreraTerminada) 
        {
            forwardInput = Input.GetAxisRaw("Vertical");
            turnInput = Input.GetAxis("Horizontal");
            brakeInput = Input.GetKey(KeyCode.Space);
        }
        else if (carreraTerminada)
        {
            brakeInput = true; 
        }

        carScript.SetInputs(forwardInput, turnInput, brakeInput);

        if (textoVueltas != null && checkpointTracker != null && !carreraTerminada)
        {
            int vueltaMostrar = Mathf.Clamp(checkpointTracker.lap, 1, vueltasParaTerminar);
            textoVueltas.text = "Vuelta: " + vueltaMostrar.ToString() + " / " + vueltasParaTerminar; 
        }

        if (RaceMonitor.primeraIATermino && !carreraTerminada)
        {
            PerderCarrera();
        }

        // --- LÓGICA DE RESPAWN POR WAYPOINT ---

        // 1. Chequeamos si el auto se está moviendo
        if (rb.velocity.magnitude > 1f || !RaceMonitor.racing)
        {
            lastTimeMoving = Time.time;
        }

        // 2. Si pasaron 4 segundos estancado
        if (Time.time > lastTimeMoving + 4f)
        {
            // Le pedimos la posición al último waypoint cruzado
            // NOTA: Acá asumo que tu CheckpointManager tiene una variable llamada "ultimoWaypoint"
            if (checkpointTracker.ultimoWaypoint != null)
            {
                transform.position = checkpointTracker.ultimoWaypoint.position;
                transform.rotation = checkpointTracker.ultimoWaypoint.rotation;
            }
            else
            {
                // Si por alguna razón todavía no cruzó ningún waypoint, lo mandamos al inicio
                transform.position = posicionInicial;
                transform.rotation = rotacionInicial;
            }
            
            // Frenamos el auto para que no salga volando por la inercia anterior
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // Reseteamos el contador
            lastTimeMoving = Time.time; 
        }
    }

    private void TerminarCarrera()
    {
        carreraTerminada = true;
        if (panelFinCarrera != null) panelFinCarrera.SetActive(true);
    }

    public void VolverAlMenu()
    {
        SceneManager.LoadScene("MenuPrincipal"); 
    }

    private void PerderCarrera()
    {
        carreraTerminada = true;
        if (panelPerdiste != null) panelPerdiste.SetActive(true);
        carScript.SetInputs(0, 0, true); 
    }

    public void ReiniciarCarrera()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
    }
}