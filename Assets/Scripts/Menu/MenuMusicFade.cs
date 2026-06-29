using UnityEngine;
using System.Collections;

public class MenuMusicFade : MonoBehaviour
{
    public AudioSource audioSource; 
    public float fadeDuration = 2.0f; 

    void Start()
    {
    
        audioSource.volume = 0;
        
        
        audioSource.Play();
        
       
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        float currentTime = 0;

        
        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            
            
            audioSource.volume = Mathf.Lerp(0, 1, currentTime / fadeDuration);
            
            yield return null; 
        }

        
        audioSource.volume = 1;
    }
}