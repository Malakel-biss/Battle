using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class ReadyFight : MonoBehaviour
{
    private TextMeshProUGUI readyFightText; // Référence au texte "Ready" et "Fight"
    private AudioSource readyFightAudioSource; // L'AudioSource pour le son de "ReadyFight"

    void Awake()
    {
       
        Transform readyFightAudio = transform.Find("ReadyFightAudio");
        if (readyFightAudio != null)
        {
            readyFightAudioSource = readyFightAudio.GetComponent<AudioSource>();
            if (readyFightAudioSource == null)
            {
                Debug.LogError("Aucun composant AudioSource trouvé sur ReadyFightAudio.");
            }
            else
            {
                readyFightAudioSource.playOnAwake = false; 
            }
        }
        else
        {
            Debug.LogError("ReadyFightAudio non trouvé comme enfant de " + gameObject.name);
        }

        // S'abonner à l'événement de changement de scène
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        // Se désabonner de l'événement pour éviter les fuites de mémoire
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Vérifie si la scène actuelle est une scène de combat
        if (scene.name == "map1" || scene.name == "Map2" || scene.name == "Map3")
        {
            Debug.Log("Scène de combat chargée : " + scene.name);

            // Trouver le TextMeshPro dans la scène actuelle
            readyFightText = GameObject.Find("ReadyFightText")?.GetComponent<TextMeshProUGUI>();
            if (readyFightText == null)
            {
                Debug.LogError("TextMeshProUGUI 'ReadyFightText' non trouvé dans la scène.");
            }

            // Démarrer la séquence "Ready" puis "Fight"
            StartCoroutine(ShowReadyFight());
        }
    }

    IEnumerator ShowReadyFight()
    {
        // Afficher "Ready"
        if (readyFightText != null)
        {
            readyFightText.text = "Ready";
            readyFightText.gameObject.SetActive(true);
        }

        // Jouer le son de "ReadyFight"
        if (readyFightAudioSource != null)
        {
            readyFightAudioSource.Play();
        }

        // Attendre 1 seconde
        yield return new WaitForSeconds(1f);

        // Changer le texte pour "Fight"
        if (readyFightText != null)
        {
            readyFightText.text = "Fight";
        }

        // Attendre 1 seconde supplémentaire
        yield return new WaitForSeconds(1f);

        // Désactiver le texte
        if (readyFightText != null)
        {
            readyFightText.gameObject.SetActive(false);
        }
    }
}