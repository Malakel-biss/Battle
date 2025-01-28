using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BattleAudio : MonoBehaviour
{
    public static BattleAudio instance;
    private AudioSource battleAudioSource; 

    private bool isCombatStarted = false; 

    void Awake()
    {
       
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        
        Transform battleAudio = transform.Find("BattleAudio");
        if (battleAudio != null)
        {
            battleAudioSource = battleAudio.GetComponent<AudioSource>();
            if (battleAudioSource == null)
            {
                Debug.LogError("Aucun composant AudioSource trouvé sur BattleAudio.");
            }
            else
            {
                battleAudioSource.playOnAwake = false; 
                battleAudioSource.Stop(); 
            }
        }
        else
        {
            Debug.LogError("BattleAudio non trouvé comme enfant de " + gameObject.name);
        }
    }

    void Update()
    {
       
        if (GameManager.instance == null)
        {
            Debug.LogWarning("");
            return;
        }

      
        if (GameManager.instance.currentPhase == GameManager.GamePhase.Battle)
        {
            if (!isCombatStarted)
            {
                StartCombat(); // Démarrer le son de combat
            }
        }
        else
        {
            if (isCombatStarted)
            {
                StopCombat(); // Arrêter le son de combat
            }
        }
    }

  
    void StartCombat()
    {
        Debug.Log("!");

        // Vérifie que le son est assigné
        if (battleAudioSource != null)
        {
            battleAudioSource.loop = true;  // Active la boucle du son
            battleAudioSource.Play(); // Joue le son
            isCombatStarted = true;  // Marque le début du combat
            Debug.Log("La bataille a commencé !");
        }
        else
        {
            Debug.LogError("Combat sound n'est pas assigné !");
        }
    }

    // Méthode pour arrêter le combat
    void StopCombat()
    {
        Debug.Log("StopCombat appelé dans AudioManager !");

        // Vérifie que le son est assigné
        if (battleAudioSource != null && battleAudioSource.isPlaying)
        {
            battleAudioSource.Stop(); // Arrête le son
            isCombatStarted = false;  // Marque la fin du combat
            Debug.Log("La bataille est terminée !");
        }
    }
}