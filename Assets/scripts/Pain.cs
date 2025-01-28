using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class Pain : MonoBehaviour
{
    private AudioSource painAudioSource;

    private List<Unit> previousPlayerUnits = new List<Unit>();
    private List<Unit> previousEnemyUnits = new List<Unit>();

    void Awake()
    {
        // R�cup�rer PainAudio
        Transform painAudio = transform.Find("PainAudio");
        if (painAudio != null)
        {
            painAudioSource = painAudio.GetComponent<AudioSource>();
            if (painAudioSource == null)
            {
                Debug.LogError("Aucun composant AudioSource trouv� sur PainAudio.");
            }
            else
            {
                painAudioSource.playOnAwake = false;
                Debug.Log("AudioSource initialis� avec succ�s.");
            }
        }
        else
        {
            Debug.LogError("PainAudio non trouv� comme enfant de " + gameObject.name);
        }

        // Initialiser les listes pr�c�dentes
        if (GameManager.instance != null)
        {
            previousPlayerUnits = new List<Unit>(GameManager.instance.playerUnits);
            previousEnemyUnits = new List<Unit>(GameManager.instance.enemyUnits);
            Debug.Log("Listes d'unit�s initialis�es.");
        }
        else
        {
            Debug.LogError("GameManager non trouv� !");
        }

        // S'abonner � l'�v�nement de changement de sc�ne
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        // Se d�sabonner de l'�v�nement de changement de sc�ne pour �viter les fuites de m�moire
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // R�initialiser les listes lorsque la sc�ne change
        previousPlayerUnits.Clear();
        previousEnemyUnits.Clear();
        Debug.Log("Listes d'unit�s r�initialis�es apr�s changement de sc�ne.");
    }

    void Update()
    {
        if (GameManager.instance == null)
        {
            Debug.LogWarning("GameManager non trouv� !");
            return;
        }

        // V�rifie si une unit� joueur est morte
        foreach (Unit unit in previousPlayerUnits)
        {
            if (!GameManager.instance.playerUnits.Contains(unit))
            {
                Debug.Log("Une unit� joueur est morte.");
                PlayPainSound("Player");
                break;
            }
        }

        // V�rifie si une unit� ennemie est morte
        foreach (Unit unit in previousEnemyUnits)
        {
            if (!GameManager.instance.enemyUnits.Contains(unit))
            {
                Debug.Log("Une unit� ennemie est morte.");
                PlayPainSound("Enemy");
                break;
            }
        }

        // Mettre � jour les listes pr�c�dentes
        previousPlayerUnits = new List<Unit>(GameManager.instance.playerUnits);
        previousEnemyUnits = new List<Unit>(GameManager.instance.enemyUnits);
    }

    // M�thode pour jouer le son de "pain"
    void PlayPainSound(string team)
    {
        // V�rifie si la sc�ne actuelle est une sc�ne de combat
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "map1" || sceneName == "Map2" || sceneName == "Map3")
        {
            Debug.Log($"Une unit� {team} est morte dans la sc�ne {sceneName}.");

            if (painAudioSource != null)
            {
                painAudioSource.Play(); // Joue le son de "pain"
                Debug.Log("Son de pain jou�.");
            }
            else
            {
                Debug.LogError("Pain sound n'est pas assign� !");
            }
        }
        else
        {
            Debug.LogWarning($"Sc�ne actuelle ({sceneName}) n'est pas une sc�ne de combat.");
        }
    }
}