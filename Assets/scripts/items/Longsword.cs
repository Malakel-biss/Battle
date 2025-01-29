using UnityEngine;

public class Longsword : Item
{
    private void Awake()
    {
        name = "Longsword";
        cost = 40;
        bonusDamage = 20;
        bonusArmor = 0;
        bonusHealth = 0;
    }
}
