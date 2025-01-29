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
        // Récupérer PainAudio
        Transform painAudio = transform.Find("PainAudio");
        if (painAudio != null)
        {
            painAudioSource = painAudio.GetComponent<AudioSource>();
            if (painAudioSource == null)
            {
                Debug.LogError("Aucun composant AudioSource trouvé sur PainAudio.");
            }
            else
            {
                painAudioSource.playOnAwake = false;
                Debug.Log("AudioSource initialisé avec succès.");
            }
        }
        else
        {
            Debug.LogError("PainAudio non trouvé comme enfant de " + gameObject.name);
        }

        // Initialiser les listes précédentes
        if (GameManager.instance != null)
        {
            previousPlayerUnits = new List<Unit>(GameManager.instance.playerUnits);
            previousEnemyUnits = new List<Unit>(GameManager.instance.enemyUnits);
            Debug.Log("Listes d'unités initialisées.");
        }
        else
        {
            Debug.LogError("GameManager non trouvé !");
        }

        // S'abonner à l'événement de changement de scène
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        // Se désabonner de l'événement de changement de scène pour éviter les fuites de mémoire
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Réinitialiser les listes lorsque la scène change
        previousPlayerUnits.Clear();
        previousEnemyUnits.Clear();
        Debug.Log("Listes d'unités réinitialisées après changement de scène.");
    }

    void Update()
    {
        if (GameManager.instance == null)
        {
            Debug.LogWarning("GameManager non trouvé !");
            return;
        }

        // Vérifie si une unité joueur est morte
        foreach (Unit unit in previousPlayerUnits)
        {
            if (!GameManager.instance.playerUnits.Contains(unit))
            {
                Debug.Log("Une unité joueur est morte.");
                PlayPainSound("Player");
                break;
            }
        }

        // Vérifie si une unité ennemie est morte
        foreach (Unit unit in previousEnemyUnits)
        {
            if (!GameManager.instance.enemyUnits.Contains(unit))
            {
                Debug.Log("Une unité ennemie est morte.");
                PlayPainSound("Enemy");
                break;
            }
        }

        // Mettre à jour les listes précédentes
        previousPlayerUnits = new List<Unit>(GameManager.instance.playerUnits);
        previousEnemyUnits = new List<Unit>(GameManager.instance.enemyUnits);
    }

    // Méthode pour jouer le son de "pain"
    void PlayPainSound(string team)
    {
        // Vérifie si la scène actuelle est une scène de combat
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "map1" || sceneName == "Map2" || sceneName == "Map3")
        {
            Debug.Log($"Une unité {team} est morte dans la scène {sceneName}.");

            if (painAudioSource != null)
            {
                painAudioSource.Play(); // Joue le son de "pain"
                Debug.Log("Son de pain joué.");
            }
            else
            {
                Debug.LogError("Pain sound n'est pas assigné !");
            }
        }
        else
        {
            Debug.LogWarning($"Scène actuelle ({sceneName}) n'est pas une scène de combat.");
        }
    }
}