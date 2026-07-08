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


    [Header("Controles Móviles")]
    public float aceleracionMovil = 0f;
    public float direccionMovil = 0f;
    public bool frenoMovil = false;
    // --------------------------------

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
            // Si el botón táctil se está usando, toma ese valor. Si no, usa el teclado.
            forwardInput = aceleracionMovil != 0f ? aceleracionMovil : Input.GetAxisRaw("Vertical");
            turnInput = direccionMovil != 0f ? direccionMovil : Input.GetAxis("Horizontal");
            brakeInput = frenoMovil || Input.GetKey(KeyCode.Space);
            // ---------------------------------------------
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

        // Chequeamos si el auto se está moviendo
        if (rb.velocity.magnitude > 1f || !RaceMonitor.racing)
        {
            lastTimeMoving = Time.time;
        }

        // Si pasaron 4 segundos estancado
        if (Time.time > lastTimeMoving + 4f)
        {
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
            
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

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