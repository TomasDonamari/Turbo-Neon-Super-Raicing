using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPausa : MonoBehaviour
{
    [Header("UI Referencias")]
    public GameObject panelPausa;

    private bool juegoPausado = false;

    void Update()
    {
        // Detectamos si se presiona la tecla Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (juegoPausado)
            {
                Reanudar();
            }
            else
            {
                Pausar();
            }
        }
    }

    public void Pausar()
    {
        // Activamos el panel gráfico
        panelPausa.SetActive(true);
        
        // Congelamos el tiempo
        Time.timeScale = 0f;
        
        juegoPausado = true;
    }

    public void Reanudar()
    {
        // Desactivamos el panel gráfico
        panelPausa.SetActive(false);
        
        // Devolvemos el tiempo a la normalidad
        Time.timeScale = 1f;
        
        juegoPausado = false;
    }

    public void VolverAlMenu()
    {
        // MUY IMPORTANTE: Antes de cambiar de escena, hay que devolver el tiempo a 1.
        // Si no lo hacés, tu Menú Principal va a cargar congelado.
        Time.timeScale = 1f;
        
        // Usamos el nombre de tu escena de menú (que vi en tu script anterior)
        SceneManager.LoadScene("MenuPrincipal"); 
    }
}