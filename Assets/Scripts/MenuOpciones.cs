using UnityEngine;
using UnityEngine.UI;

public class MenuOpciones : MonoBehaviour
{
    public Toggle toggleMusica;

    void Start()
    {
        // 1. Leemos el valor guardado (1 = activada, 0 = apagada)
        bool musicaActiva = PlayerPrefs.GetInt("MusicaCarrera", 1) == 1;

        // 2. SetIsOnWithoutNotify cambia el tilde visual sin disparar el evento por accidente
        if (toggleMusica != null)
        {
            toggleMusica.SetIsOnWithoutNotify(musicaActiva);
        }
        
        Debug.Log("Menú iniciado. Estado de música cargado: " + musicaActiva);
    }

    public void CambiarEstadoMusica(bool activa)
    {
        PlayerPrefs.SetInt("MusicaCarrera", activa ? 1 : 0);
        PlayerPrefs.Save();
        
        Debug.Log("Se cambió el Toggle. Música guardada como: " + activa);
    }
}