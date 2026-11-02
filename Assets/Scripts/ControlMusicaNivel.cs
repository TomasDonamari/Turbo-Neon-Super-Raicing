using UnityEngine;

public class ControlMusicaNivel : MonoBehaviour
{
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
        bool musicaActiva = PlayerPrefs.GetInt("MusicaCarrera", 1) == 1;
        Debug.Log("Carrera cargada. ¿Música activa?: " + musicaActiva);

        if (!musicaActiva)
        {
            audioSource.mute = true;
            audioSource.Stop(); // Lo frena por completo además de mutearlo
        }
        else
        {
            audioSource.mute = false;
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
    }
}