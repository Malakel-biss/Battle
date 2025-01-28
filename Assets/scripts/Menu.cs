using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Make sure to include this for UI components

public class Menu : MonoBehaviour
{
    public GameObject optionsPanel;
    public GameObject lvlsPanel;
    public GameObject EscPanel;

    public void PlayGame()
    {
        StartCoroutine(LoadSceneCoroutine("Map1"));
    }

    IEnumerator LoadSceneCoroutine(string sceneName)
    {
        var op = SceneManager.LoadSceneAsync(sceneName);
        while (!op.isDone)
        {
            yield return null;
        }
    }

    // Method to load a specific scene by name
    public void LoadLevelByName(string sceneName)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }

    public void LoadNextLevel()
    {
        // Get the current scene index
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Calculate the index for the next scene
        int nextSceneIndex = currentSceneIndex + 1;

        // Check if the next scene index is within the range of available scenes
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("No more levels to load, or loop to the first level");
            // SceneManager.LoadScene(0);
        }
    }

    public void ToggleOptions()
    {
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(!optionsPanel.activeSelf);
        }
    }

    public void ToggleLvls()
    {
        if (lvlsPanel != null)
        {
            lvlsPanel.SetActive(!lvlsPanel.activeSelf);
        }
    }

    public void ToggleEsc()
    {
        if (EscPanel != null)
        {
            EscPanel.SetActive(!EscPanel.activeSelf);
        }
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("mainmenu");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void lvlloadingbutton(GameObject button)
    {
        // Safely access the Text component in the children of the button
        TextMeshProUGUI levelText = button.GetComponentInChildren<TextMeshProUGUI>();
        if (levelText != null)
        {
            string lvl = levelText.text;
            LoadLevelByName(lvl);  // Make sure the scene names are correct and match the text
        }
        else
        {
            Debug.LogError("No Text component found on the button or the button is null.");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleEsc();
        }
    }
}
