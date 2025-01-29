using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public RawImage healthFill;  // Reference to the health bar
    public RawImage manaFill;    // Reference to the mana (ability) bar

    public void SetHealth(float health, float maxHealth)
    {
        float widthPercent = health / maxHealth;
        healthFill.uvRect = new Rect(0f, 0f, widthPercent, 1f);
    }

    public void SetMana(float mana, float maxMana)
    {
        float widthPercent = mana / maxMana;
        manaFill.uvRect = new Rect(0f, 0f, widthPercent, 1f);
    }

    public void ResetManaBar()
    {
        manaFill.uvRect = new Rect(0f, 0f, 0f, 1f); // Resets the mana bar to empty
    }
}
