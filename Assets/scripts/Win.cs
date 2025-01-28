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
            DontDestroyOnLoad(gameObject); // Garde l'AudioManager entre les sc�nes
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
                Debug.LogError("Aucun composant AudioSource trouv� sur WinAudio.");
            }
            else
            {
                winAudioSource.playOnAwake = false; 
            }
        }
        else
        {
            Debug.LogError("WinAudio non trouv� comme enfant de " + gameObject.name);
        }

        
        Transform gameOverAudio = transform.Find("GameOverAudio");
        if (gameOverAudio != null)
        {
            gameOverAudioSource = gameOverAudio.GetComponent<AudioSource>();
            if (gameOverAudioSource == null)
            {
                Debug.LogError("Aucun composant AudioSource trouv� sur GameOverAudio.");
            }
            else
            {
                gameOverAudioSource.playOnAwake = false; // D�sactiver Play On Awake
            }
        }
        else
        {
            Debug.LogError("GameOverAudio non trouv� comme enfant de " + gameObject.name);
        }

        // S'abonner � l'�v�nement de changement de sc�ne pour r�initialiser les bool�ens
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        // Se d�sabonner de l'�v�nement pour �viter les fuites de m�moire
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // R�initialiser les bool�ens lors du chargement d'une nouvelle sc�ne
        hasPlayedWinSound = false;
        hasPlayedGameOverSound = false;
    }

    void Update()
    {
        // V�rifie si le GameManager est disponible
        if (GameManager.instance == null)
        {
            Debug.LogWarning(" ");
            return;
        }

       
        if (GameManager.instance.victoryPanel.activeSelf && !hasPlayedWinSound)
        {
            PlayWinSound();
            hasPlayedWinSound = true; // Marquer que le son de victoire a �t� jou�
        }

        // V�rifie si le jeu est en phase de game over
        if (GameManager.instance.gameOverPanel.activeSelf && !hasPlayedGameOverSound)
        {
            PlayGameOverSound();
            hasPlayedGameOverSound = true; // Marquer que le son de game over a �t� jou�
        }
    }

    // M�thode pour jouer le son de victoire
    void PlayWinSound()
    {
        // V�rifie que le son est assign�
        if (winAudioSource != null)
        {
            winAudioSource.Play(); // Joue le son de victoire
        }
        else
        {
            Debug.LogError("Win sound n'est pas assign� !");
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
            Debug.LogError("GameOver sound n'est pas assign� !");
        }
    }
}