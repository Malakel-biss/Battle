using System.Collections;
using UnityEngine;

public class Heavy : Unit
{
    public override void Start()
    {
        base.Start();  // Calls the start method of the Unit class to initialize the animator
    }

    void Awake()
    {
        hp = 200f;
        maxHp = 200f;
        damage = 10;
        attackInterval = 10f;
        armor = 40;
        mana = 0f;
        maxMana = 50f;
        manaRegenRate = 8f;
        cost = 30;
        goldReward = 25;
    }

    public override void SpecialAbility()
    {
        // Perform the special ability
        StartCoroutine(UseArmorBoost());
    }

    private IEnumerator UseArmorBoost()
    {
        // Deduct mana cost
        mana = 0f; // Reset mana after using the ability

        animator.SetTrigger("ability2");

        // Temporarily increase armor by +20
        int originalArmor = armor; // Save the original armor
        int bonusArmor = 20;
        armor += bonusArmor;

        Debug.Log($"{name} uses Armor Boost! Armor temporarily increased to {armor}");

        // The effect lasts for one attack
        yield return new WaitForSeconds(8f); // Wait for one attack interval
        armor = originalArmor; // Revert the armor back to its original value

        Debug.Log($"{name}'s Armor Boost effect has ended. Armor reverted to {armor}");

        animator.ResetTrigger("ability2");
    }

    void Update()
    {
        // Automatically trigger the special ability when mana is full and in combat
        if (mana >= maxMana && isAttacking)
        {
            SpecialAbility();
        }

        // Manual activation: press Space to trigger the ability
        if (Input.GetKeyDown(KeyCode.Space))
        {
            mana = 0f; // Reset mana upon manual activation
            SpecialAbility();
        }
    }

    protected override IEnumerator Attack(Unit target)
    {
        animator.SetBool("isattacking2", true);
        isAttacking = true;

        while (target != null && target.hp > 0 && hp > 0)
        {
            target.TakeDamage(damage); // Deal damage to the target
            Debug.Log($"{name} (Team {team}) attacks {target.name} for {damage} damage!");
            yield return new WaitForSeconds(attackInterval); // Wait for the next attack

        }

        isAttacking = false;
        animator.SetBool("isattacking2", false);
    }
}