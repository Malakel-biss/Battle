using UnityEngine;

public class TowerShield : Item
{
    private void Awake()
    {
        name = "Tower Shield";
        cost = 35;
        bonusDamage = 0;
        bonusArmor = 15;
        bonusHealth = 10;
    }
}
