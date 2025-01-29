using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AudioSettings : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource Background; // Musique de fond
    public AudioSource ButtonClickSound; // Son de clic de bouton
    public AudioSource BattleAudio; // Son de bataille
    public AudioSource PainAudio; // Son de douleur
    public AudioSource WinAudio; // Son de victoire
    public AudioSource GameOverAudio; // Son de d�faite

    [Header("Sliders")]
    public Slider backgroundSlider; // Slider pour la musique de fond
    public Slider buttonClickSlider; // Slider pour le son de clic
    public Slider battleSlider; // Slider pour le son de bataille
    public Slider painSlider; // Slider pour le son de douleur
    public Slider winSlider; // Slider pour le son de victoire
    public Slider gameOverSlider; // Slider pour le son de d�faite

    [Header("Volume Texts")]
    public TextMeshProUGUI backgroundVolumeText;
    public TextMeshProUGUI buttonClickVolumeText;
    public TextMeshProUGUI battleVolumeText;
    public TextMeshProUGUI painVolumeText;
    public TextMeshProUGUI winVolumeText;
    public TextMeshProUGUI gameOverVolumeText;

    [Header("Text Settings")]
    public float textSize = 20f; 
    public Vector2 textOffset = new Vector2(-50f, 0f); 

    void Start()
    {
        
        InitializeSlider(backgroundSlider, "BackgroundVolume", Background, backgroundVolumeText, "Background");
        InitializeSlider(buttonClickSlider, "ButtonClickVolume", ButtonClickSound, buttonClickVolumeText, "Click", true);
        InitializeSlider(battleSlider, "BattleVolume", BattleAudio, battleVolumeText, "Fight", true);
        InitializeSlider(painSlider, "PainVolume", PainAudio, painVolumeText, "Pain", true);
        InitializeSlider(winSlider, "WinVolume", WinAudio, winVolumeText, "Victory", true);
        InitializeSlider(gameOverSlider, "GameOverVolume", GameOverAudio, gameOverVolumeText, "Game Over", true);
    }

    // M�thode pour initialiser un slider
    private void InitializeSlider(Slider slider, string playerPrefKey, AudioSource audioSource, TextMeshProUGUI volumeText, string displayName, bool playSound = false)
    {
        if (PlayerPrefs.HasKey(playerPrefKey))
        {
            float savedVolume = PlayerPrefs.GetFloat(playerPrefKey);
            slider.value = savedVolume;
            SetVolume(audioSource, savedVolume, volumeText, displayName);
        }
        else
        {
            slider.value = audioSource.volume;
            SetVolume(audioSource, audioSource.volume, volumeText, displayName);
        }

        // un �couteur pour les changements de volume
        slider.onValueChanged.AddListener((value) => SetVolume(audioSource, value, volumeText, displayName, playSound));
    }

    // M�thode pour ajuster le volume, mettre � jour le texte et jouer le son si n�cessaire
    private void SetVolume(AudioSource audioSource, float volume, TextMeshProUGUI volumeText, string displayName, bool playSound = false)
    {
        if (audioSource != null)
        {
            // Ajuster le volume
            audioSource.volume = volume;
            PlayerPrefs.SetFloat(audioSource.name + "Volume", volume); // Sauvegarder le volume

            // Mettre � jour le texte avec le nom personnalis� et le pourcentage
            SetVolumeText(volumeText, displayName, volume);

            // Jouer le son pour le tester
            if (playSound)
            {
                // V�rifier si le son est d�j� en train de jouer
                if (!audioSource.isPlaying)
                {
                    audioSource.loop = false; // D�sactiver la boucle
                    audioSource.Play(); // Jouer le son une seule fois
                }
            }
        }
    }

    // M�thode pour mettre � jour le texte du volume avec le nom personnalis� et le pourcentage
    private void SetVolumeText(TextMeshProUGUI volumeText, string displayName, float volume)
    {
        if (volumeText != null)
        {
            int volumePercentage = Mathf.RoundToInt(volume * 100);
            volumeText.text = $"{displayName}: {volumePercentage}%"; // Afficher le nom et le pourcentage
            volumeText.fontSize = textSize; // D�finir la taille de la police
        }
    }
}