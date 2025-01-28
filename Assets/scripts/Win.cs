using UnityEngine;
using UnityEngine.SceneManagement;

public class Win : MonoBehaviour
{
    public static Win instance;

    private AudioSource winAudioSource;       
    private AudioSource gameOverAudioSource;  

    private bool hasPlayedWinSound = false;    
    private bool hasPlayedGameOverSound = false; 

    void Awake()
    {
        
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Garde l'AudioManager entre les scènes
        }
        else
        {
            Destroy(gameObject);
            return;
        }

       
        Transform winAudio = transform.Find("WinAudio");
        if (winAudio != null)
        {
            winAudioSource = winAudio.GetComponent<AudioSource>();
            if (winAudioSource == null)
            {
                Debug.LogError("Aucun composant AudioSource trouvé sur WinAudio.");
            }
            else
            {
                winAudioSource.playOnAwake = false; 
            }
        }
        else
        {
            Debug.LogError("WinAudio non trouvé comme enfant de " + gameObject.name);
        }

        
        Transform gameOverAudio = transform.Find("GameOverAudio");
        if (gameOverAudio != null)
        {
            gameOverAudioSource = gameOverAudio.GetComponent<AudioSource>();
            if (gameOverAudioSource == null)
            {
                Debug.LogError("Aucun composant AudioSource trouvé sur GameOverAudio.");
            }
            else
            {
                gameOverAudioSource.playOnAwake = false; // Désactiver Play On Awake
            }
        }
        else
        {
            Debug.LogError("GameOverAudio non trouvé comme enfant de " + gameObject.name);
        }

        // S'abonner à l'événement de changement de scène pour réinitialiser les booléens
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        // Se désabonner de l'événement pour éviter les fuites de mémoire
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Réinitialiser les booléens lors du chargement d'une nouvelle scène
        hasPlayedWinSound = false;
        hasPlayedGameOverSound = false;
    }

    void Update()
    {
        // Vérifie si le GameManager est disponible
        if (GameManager.instance == null)
        {
            Debug.LogWarning(" ");
            return;
        }

       
        if (GameManager.instance.victoryPanel.activeSelf && !hasPlayedWinSound)
        {
            PlayWinSound();
            hasPlayedWinSound = true; // Marquer que le son de victoire a été joué
        }

        // Vérifie si le jeu est en phase de game over
        if (GameManager.instance.gameOverPanel.activeSelf && !hasPlayedGameOverSound)
        {
            PlayGameOverSound();
            hasPlayedGameOverSound = true; // Marquer que le son de game over a été joué
        }
    }

    // Méthode pour jouer le son de victoire
    void PlayWinSound()
    {
        // Vérifie que le son est assigné
        if (winAudioSource != null)
        {
            winAudioSource.Play(); // Joue le son de victoire
        }
        else
        {
            Debug.LogError("Win sound n'est pas assigné !");
        }
    }

   
    void PlayGameOverSound()
    {
        
        if (gameOverAudioSource != null)
        {
            gameOverAudioSource.Play(); // Joue le son de game over
        }
        else
        {
            Debug.LogError("GameOver sound n'est pas assigné !");
        }
    }
}