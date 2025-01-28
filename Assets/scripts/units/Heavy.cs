using System.Collections;
using UnityEngine;

public class Heavy : Unit
{
    private bool canUseSpecialAbility = true; // To track cooldown status

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
        manaRegenRate = 2f;
        cost = 30;
        goldReward = 25;
    }

    public override void SpecialAbility()
    {
        if (!canUseSpecialAbility)
        {
            Debug.Log($"{name} cannot use Armor Boost yet! Cooldown in effect.");
            return;
        }

        if (mana < 40f)
        {
            Debug.Log($"{name} does not have enough mana to use Armor Boost!");
            return;
        }

        // Perform the special ability
        StartCoroutine(UseArmorBoost());
    }

    private IEnumerator UseArmorBoost()
    {
        // Deduct mana cost
        mana -= 40f;

        animator.SetTrigger("ability2");

        // Temporarily increase armor by +20
        int originalArmor = armor; // Save the original armor
        int bonusArmor = 20;
        armor += bonusArmor;

        Debug.Log($"{name} uses Armor Boost! Armor temporarily increased to {armor}");

        // The effect lasts for one attack
        yield return new WaitForSeconds(8f); // Wait for one attack interval
        armor = originalArmor; // Revert the armor back to its original value

        // Start the cooldown timer
        canUseSpecialAbility = false;
        yield return new WaitForSeconds(12f); // Cooldown period
        animator.ResetTrigger("ability2");
        canUseSpecialAbility = true;

        Debug.Log($"{name} can now use Armor Boost again!");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B)) // Press Space to trigger the special ability
        {
            Heavy heavy = GetComponent<Heavy>();
            if (heavy != null)
            {
                heavy.SpecialAbility();
            }
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
