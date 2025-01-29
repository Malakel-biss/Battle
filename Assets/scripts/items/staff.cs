using UnityEngine;

public class staff : Item
{
    private void Awake()
    {
        name = "Staff";
        cost = 30;
        bonusDamage = 15;
        bonusArmor = 0;
        bonusHealth = 0;
    }
}
