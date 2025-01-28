using UnityEngine;
using UnityEngine.UI;
using TMPro; // Nécessaire pour utiliser TextMeshPro

public class VolumeController : MonoBehaviour
{
    public Slider volumeSlider; 
    public TextMeshProUGUI volumeText;
    public AudioSource backgroundMusic; 

    [Header("Volume Settings")]
    public float defaultVolume = 0.5f; 

    void Start()
    {
        // Initialiser la valeur du Slider avec le volume actuel ou le volume par défaut
        if (PlayerPrefs.HasKey("Volume"))
        {
            float savedVolume = PlayerPrefs.GetFloat("Volume");
            volumeSlider.value = savedVolume;
            SetVolume(savedVolume); // Appliquer le volume sauvegardé
        }
        else
        {
            volumeSlider.value = defaultVolume; 
            SetVolume(defaultVolume); 
        }

        // Ajouter un écouteur pour les changements de volume
        volumeSlider.onValueChanged.AddListener(SetVolume);

        // Mettre à jour le texte du volume au démarrage
        UpdateVolumeText(volumeSlider.value);
    }

    // Méthode pour ajuster le volume
    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        if (backgroundMusic != null)
        {
            backgroundMusic.volume = volume; // Appliquer le volume à la musique de fond 
        }
        PlayerPrefs.SetFloat("Volume", volume); // Sauvegarder le volume

        
        UpdateVolumeText(volume);
    }

    // Méthode pour mettre à jour le texte du volume
    private void UpdateVolumeText(float volume)
    {
        if (volumeText != null)
        {
            // Convertir le volume en pourcentage (0.0 -> 0%, 1.0 -> 100%)
            int volumePercentage = Mathf.RoundToInt(volume * 100);
            volumeText.text = "Music:" +volumePercentage + "%"; 
        }
    }
}