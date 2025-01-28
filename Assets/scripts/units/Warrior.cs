using System.Collections;
using UnityEditor.Analytics;
using UnityEngine;

public class Warrior : Unit
{
    private bool canUseSpecialAbility = true; // To track cooldown status

    public override void Start()
    {
        base.Start();  // Calls the start method of the Unit class to initialize the animator
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
        manaRegenRate = 10f;
        cost = 25;
        goldReward = 20;
    }

    public override void SpecialAbility()
    {
        // Check if the Warrior has enough mana and the ability is off cooldown
        if (!canUseSpecialAbility)
        {
            Debug.Log($"{name} cannot use Shield Slam yet! Cooldown in effect.");
            return;
        }

        if (mana < 30f)
        {
            Debug.Log($"{name} does not have enough mana to use Shield Slam!");
            return;
        }

        // Perform the special ability
        StartCoroutine(UseShieldSlam());
    }

    // public override void Start()
    // {
    //     base.Start();
    //     manaRegenRate = 2f; // Warrior regenerates 2 mana per second
    // }

    private IEnumerator UseShieldSlam()
    {
        // Deduct mana cost
        mana -= 30f;

        animator.SetTrigger("ability");

        // Temporarily increase damage by +10
        int originaldamage = damage; // Save the original damage
        int bonusdamage = 10;
        damage += bonusdamage;

        Debug.Log($"{name} uses Shield Slam! damage temporarily increased to {damage}");

        // The effect lasts for one attack
        yield return new WaitForSeconds(attackInterval); // Wait for one attack interval
        damage = originaldamage; // Revert the damage back to its original value

        Debug.Log($"{name}'s Shield Slam effect has ended. damage reverted to {damage}");

        // Start the cooldown timer
        canUseSpecialAbility = false;
        yield return new WaitForSeconds(4f); // Cooldown period
        animator.ResetTrigger("ability");
        canUseSpecialAbility = true;

        Debug.Log($"{name}'s Shield Slam is ready to use again!");
    }

    void Update()
    {
        //base.Update(); // Calls the update method of the Unit class to regenerate mana,

        if (Input.GetKeyDown(KeyCode.Space)) // Press Space to trigger the special ability
        {
            Warrior warrior = GetComponent<Warrior>();
            if (warrior != null)
            {
                warrior.SpecialAbility();
            }
        }
    }

}
