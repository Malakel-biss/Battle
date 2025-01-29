using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class ReadyFight : MonoBehaviour
{
    private TextMeshProUGUI readyFightText; // R�f�rence au texte "Ready" et "Fight"
    private AudioSource readyFightAudioSource; // L'AudioSource pour le son de "ReadyFight"

    void Awake()
    {
       
        Transform readyFightAudio = transform.Find("ReadyFightAudio");
        if (readyFightAudio != null)
        {
            readyFightAudioSource = readyFightAudio.GetComponent<AudioSource>();
            if (readyFightAudioSource == null)
            {
                Debug.LogError("Aucun composant AudioSource trouv� sur ReadyFightAudio.");
            }
            else
            {
                readyFightAudioSource.playOnAwake = false; 
            }
        }
        else
        {
            Debug.LogError("ReadyFightAudio non trouv� comme enfant de " + gameObject.name);
        }

        // S'abonner � l'�v�nement de changement de sc�ne
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        // Se d�sabonner de l'�v�nement pour �viter les fuites de m�moire
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // V�rifie si la sc�ne actuelle est une sc�ne de combat
        if (scene.name == "map1" || scene.name == "Map2" || scene.name == "Map3")
        {
            Debug.Log("Sc�ne de combat charg�e : " + scene.name);

            // Trouver le TextMeshPro dans la sc�ne actuelle
            readyFightText = GameObject.Find("ReadyFightText")?.GetComponent<TextMeshProUGUI>();
            if (readyFightText == null)
            {
                Debug.LogError("TextMeshProUGUI 'ReadyFightText' non trouv� dans la sc�ne.");
            }

            // D�marrer la s�quence "Ready" puis "Fight"
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

        // Attendre 1 seconde suppl�mentaire
        yield return new WaitForSeconds(1f);

        // D�sactiver le texte
        if (readyFightText != null)
        {
            readyFightText.gameObject.SetActive(false);
        }
    }
}