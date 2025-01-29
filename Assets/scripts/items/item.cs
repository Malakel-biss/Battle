using TMPro;
using UnityEngine;

public class Item : MonoBehaviour
{
    public TextMeshProUGUI costText;
    public int cost;
    public int bonusDamage;
    public int bonusArmor;
    public int bonusHealth;

    private void Start()
    {
        // Display item cost on UI (if applicable)
        if (costText != null)
        {
            transform.rotation = Quaternion.Euler(0, 90, 0); // Facing positive X on selection cells
            costText.text = cost.ToString();
        }
    }

}
