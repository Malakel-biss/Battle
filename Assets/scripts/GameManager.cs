using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public TextMeshProUGUI goldText;
    public GameObject victoryPanel; // Panel to show when enemies are defeated
    public GameObject gameOverPanel; // Panel to show on game over
    public Button startBattleButton; // Start battle button

    public int playergold = 100;

    public GameObject playerUnitPrefab;
    public GameObject enemyUnitPrefab;

    public List<Unit> playerUnits = new List<Unit>();
    public List<Unit> enemyUnits = new List<Unit>();

    public enum GamePhase
    {
        Placing,
        Battle,
        GameOver
    }


    public GamePhase currentPhase = GamePhase.Placing; // Start with the placing phase

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        startBattleButton.onClick.AddListener(StartBattle); // Add listener to start battle
    }

    private void Update()
    {
        goldText.text = " " + playergold.ToString();

        if (currentPhase == GamePhase.Battle)
        {
            if (playerUnits.Count == 0)
            {
                if (playergold >= 30)
                {
                    Debug.Log("Switching back to Placing phase.");
                    currentPhase = GamePhase.Placing; // Return to Placing phase
                    startBattleButton.gameObject.SetActive(true); // Enable the start battle button
                }
                else
                {
                    Debug.Log("Game Over!");
                    currentPhase = GamePhase.GameOver; // Switch to Game Over phase
                    gameOverPanel.SetActive(true); // Show game over UI
                }
            }
        }
    }

    public void TryPlaceUnit(GridCell cell)
    {
        if (currentPhase != GamePhase.Placing)
        {
            Debug.Log("Cannot place units during the battle or game over phase.");
            return;
        }

        if (cell.PlaceUnit(playerUnitPrefab, Unit.Team.Player))
        {
            Debug.Log("Player unit placed successfully!");
        }
        else
        {
            Debug.Log("Cell is occupied or invalid!");
        }
    }

    public void SetPlayerUnitPrefab(GameObject newPrefab)
    {
        playerUnitPrefab = newPrefab;
        Debug.Log($"GameManager has set the new player unit prefab to: {playerUnitPrefab.name}");
    }

    public void StartBattle()
    {
        if (playerUnits.Count == 0)
        {
            Debug.Log("No player units placed. Cannot start battle.");
            return;
        }

        currentPhase = GamePhase.Battle;
        startBattleButton.gameObject.SetActive(false); // Disable the start battle button

        // Trigger PerformAIActions only for player units (excluding selection cells)
        foreach (Unit playerUnit in playerUnits)
        {
            if (playerUnit.currentCell != null && !playerUnit.currentCell.isseletioncell)
            {
                playerUnit.PerformAIActions();
            }
        }

        StartCoroutine(CheckForVictoryCondition());
    }


    IEnumerator CheckForVictoryCondition()
    {
        while (currentPhase == GamePhase.Battle) // Only check during the battle phase
        {
            yield return new WaitForSeconds(1); // Check every second

            if (enemyUnits.Count == 0)
            {
                Debug.Log("Victory! All enemies defeated.");
                victoryPanel.SetActive(true);
                currentPhase = GamePhase.Placing; // Switch back to placing phase
                startBattleButton.gameObject.SetActive(true); // Enable the start battle button
                yield break; // Stop the coroutine
            }
        }
    }
}
