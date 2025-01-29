using UnityEngine;

public class greataxe : Item
{
    private void Awake()
    {
        name = "Greataxe";
        cost = 50;
        bonusDamage = 15;
        bonusArmor = 5;
        bonusHealth = 20;
    }
}
