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
                Debug.LogError("Aucun composant AudioSource trouv� sur BattleAudio.");
            }
            else
            {
                battleAudioSource.playOnAwake = false; 
                battleAudioSource.Stop(); 
            }
        }
        else
        {
            Debug.LogError("BattleAudio non trouv� comme enfant de " + gameObject.name);
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
                StartCombat(); // D�marrer le son de combat
            }
        }
        else
        {
            if (isCombatStarted)
            {
                StopCombat(); // Arr�ter le son de combat
            }
        }
    }

  
    void StartCombat()
    {
        Debug.Log("!");

        // V�rifie que le son est assign�
        if (battleAudioSource != null)
        {
            battleAudioSource.loop = true;  // Active la boucle du son
            battleAudioSource.Play(); // Joue le son
            isCombatStarted = true;  // Marque le d�but du combat
            Debug.Log("La bataille a commenc� !");
        }
        else
        {
            Debug.LogError("Combat sound n'est pas assign� !");
        }
    }

    // M�thode pour arr�ter le combat
    void StopCombat()
    {
        Debug.Log("StopCombat appel� dans AudioManager !");

        // V�rifie que le son est assign�
        if (battleAudioSource != null && battleAudioSource.isPlaying)
        {
            battleAudioSource.Stop(); // Arr�te le son
            isCombatStarted = false;  // Marque la fin du combat
            Debug.Log("La bataille est termin�e !");
        }
    }
}