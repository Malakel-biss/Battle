using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance = null;
    private AudioSource backgroundMusic;

    void Awake()
    {
       
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

      
        instance = this;
        DontDestroyOnLoad(gameObject);

        
        Transform backgroundTransform = transform.Find("Background");

        if (backgroundTransform == null)
        {
            Debug.LogError("L'objet Background n'a pas été trouvé sous AudioManager.");
            return;
        }

        backgroundMusic = backgroundTransform.GetComponent<AudioSource>();

        if (backgroundMusic == null)
        {
            Debug.LogError("Aucune AudioSource trouvée sur l'objet Background.");
            return;
        }

        
        if (!backgroundMusic.isPlaying)
        {
            backgroundMusic.Play();
        }
    }
}
