using System;
using System.Collections;
using UnityEngine;

public class Assassin : Unit
{
    private bool isImmuneToDamage = false; // Local immunity flag for Camouflage ability

    public override void Start()
    {
        base.Start();  // Calls the start method of the Unit class to initialize the animator
    }

    void Awake()
    {
        // Set the mage's stats
        hp = 130f;
        maxHp = 130;
        damage = 15;
        attackInterval = 2f;
        armor = 15;
        mana = 0f;
        maxMana = 130f;
        manaRegenRate = 15f;
        cost = 70;
        goldReward = 55;
    }

    public override void SpecialAbility()
    {
        // Perform the special ability
        StartCoroutine(Camouflage());
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

    private IEnumerator Camouflage()
    {
        // Deduct mana cost
        mana = 0f; // Reset mana after using the ability

        // Trigger the camouflage animation
        animator.SetTrigger("ability5");
        Debug.Log($"{name} activates Camouflage and becomes immune to damage for 7 seconds!");

        // Set local immunity flag
        isImmuneToDamage = true;

        // Wait for 7 seconds
        yield return new WaitForSeconds(7f);

        // Remove immunity
        isImmuneToDamage = false;
        Debug.Log($"{name}'s Camouflage effect has ended. No longer immune to damage.");

        animator.ResetTrigger("ability5");
    }

    public override void TakeDamage(int damageAmount)
    {
        if (isImmuneToDamage)
        {
            Debug.Log($"{name} is immune to damage due to Camouflage!");
            return; // Ignore damage if immune
        }

        // Proceed with normal damage calculation
        float effectiveDamage = damageAmount * (100f / (100f + armor));
        hp -= effectiveDamage; // Reduce HP 
        UpdateHealthBar(); // Update the health bar

        if (hp <= 0)
        {
            animator.SetBool("isdead", true);
            Die();
        }
    }

    protected override IEnumerator Attack(Unit target)
    {
        animator.SetBool("isattacking5", true);
        isAttacking = true;

        while (target != null && target.hp > 0 && hp > 0)
        {
            target.TakeDamage(damage); // Deal damage to the target
            Debug.Log($"{name} (Team {team}) attacks {target.name} for {damage} damage!");
            yield return new WaitForSeconds(attackInterval); // Wait for the next attack

        }

        isAttacking = false;
        animator.SetBool("isattacking5", false);
    }
}