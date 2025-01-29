using System.Collections;
using UnityEngine;

public class Warrior : Unit
{
    public override void Start()
    {
        base.Start(); // Calls the Start method of the Unit class to initialize the warrior
    }

    void Awake()
    {
        // Set the warrior's stats
        hp = 150f;
        maxHp = 150f;
        damage = 20;
        attackInterval = 3f;
        armor = 20;
        mana = 0f;
        maxMana = 50f;
        manaRegenRate = 5f;
        cost = 25;
        goldReward = 20;
    }

    public override void SpecialAbility()
    {
        // Perform the special ability regardless of cooldown or mana level
        StartCoroutine(UseShieldSlam());
    }

    private IEnumerator UseShieldSlam()
    {
        // Deduct mana only if it was a mana-triggered activation
        if (mana >= maxMana)
        {
            mana = 0f; // Reset mana if the ability is triggered automatically
        }

        animator.SetTrigger("ability");

        // Temporarily increase damage by +10
        int originalDamage = damage; // Save the original damage
        int bonusDamage = 10;
        damage += bonusDamage;

        Debug.Log($"{name} uses Shield Slam! Damage temporarily increased to {damage}");

        // The effect lasts for one attack
        yield return new WaitForSeconds(attackInterval); // Wait for one attack interval
        damage = originalDamage; // Revert the damage back to its original value

        Debug.Log($"{name}'s Shield Slam effect has ended. Damage reverted to {damage}");

        animator.ResetTrigger("ability");
    }

    void Update()
    {
        // Automatically trigger the ability when mana is full (and in combat)
        if (mana >= maxMana && isAttacking)
        {
            SpecialAbility();
        }

        // Manual activation: press Space to trigger the ability, regardless of mana level
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (team == Team.Player)
            {
                mana = 0f; // Reset mana upon manual activation
                SpecialAbility();
            }

        }
    }
}