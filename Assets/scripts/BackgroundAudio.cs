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
            Debug.LogError("L'objet Background n'a pas �t� trouv� sous AudioManager.");
            return;
        }

        backgroundMusic = backgroundTransform.GetComponent<AudioSource>();

        if (backgroundMusic == null)
        {
            Debug.LogError("Aucune AudioSource trouv�e sur l'objet Background.");
            return;
        }

        
        if (!backgroundMusic.isPlaying)
        {
            backgroundMusic.Play();
        }
    }
}
