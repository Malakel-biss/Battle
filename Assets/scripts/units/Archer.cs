using System.Collections;
using UnityEngine;

public class Archer : Unit
{
    private bool canUseSpecialAbility = true; // To track cooldown status
    public float specialAbilityCooldown = 10f; // Cooldown time in seconds after using the special ability

    protected Unit currentTarget;


    public override void Start()
    {
        base.Start();  // Calls the start method of the Unit class to initialize the animator
    }

    void Awake()
    {
        hp = 80f;
        maxHp = 80f;
        damage = 35;
        attackInterval = 8f;
        armor = 10;
        mana = 0f;
        maxMana = 130f;
        manaRegenRate = 3f;
        cost = 60;
        goldReward = 50;


        // Ensure the Archer's `currentCell` and grid state are initialized properly
        if (currentCell != null)
        {
            currentCell.placedUnit = this.gameObject;
            currentCell.isOccupied = true;
        }
    }

    public override void SpecialAbility()
    {
        if (!canUseSpecialAbility)
        {
            Debug.Log($"{name} cannot use Armor Boost yet! Cooldown in effect.");
            return;
        }

        if (mana < 70f)
        {
            Debug.Log($"{name} does not have enough mana to use Armor Boost!");
            return;
        }

        // Perform the special ability
        StartCoroutine(StormArrows());
    }

    private IEnumerator StormArrows()
    {
        // Deduct mana cost
        mana -= 70f;
        animator.SetTrigger("ability3");
        Debug.Log($"{name} is using Storm Arrows!");

        // Perform the attack 3 times
        int attacks = 3;
        while (attacks > 0)
        {
            foreach (GridCell neighbor in currentCell.neighbors)
            {
                if (neighbor != null && neighbor.isOccupied && neighbor.placedUnit != null)
                {
                    Unit enemyUnit = neighbor.placedUnit.GetComponent<Unit>();
                    if (enemyUnit != null && enemyUnit.team != team)
                    {
                        enemyUnit.TakeDamage(damage);
                        Debug.Log($"{name} attacks {enemyUnit.name} with Storm Arrows for {damage} damage.");

                        // Clear the enemy if it dies
                        if (enemyUnit.hp <= 0)
                        {
                            neighbor.ClearUnit();
                        }
                    }
                }
            }

            attacks--;
            yield return new WaitForSeconds(1); // Wait 1 second between attacks
        }

        animator.ResetTrigger("ability3");

        // Reset cooldown
        canUseSpecialAbility = false;
        StartCoroutine(ResetSpecialAbilityCooldown());
    }


    private IEnumerator ResetSpecialAbilityCooldown()
    {
        yield return new WaitForSeconds(specialAbilityCooldown);
        canUseSpecialAbility = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V)) // Press Space to trigger the special ability
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
        animator.SetBool("isattacking3", true);
        isAttacking = true;

        while (target != null && target.hp > 0 && hp > 0)
        {
            // Deal damage to the target
            target.TakeDamage(damage);
            Debug.Log($"{name} (Team {team}) attacks {target.name} for {damage} damage!");

            // Break the loop if the target dies
            if (target.hp <= 0)
            {
                target = null;
                currentTarget = null; // Clear the current target
                break;
            }

            yield return new WaitForSeconds(attackInterval); // Wait for the next attack
        }

        isAttacking = false;
        animator.SetBool("isattacking3", false);

        // Trigger AI to find a new target
        PerformAIActions();
    }


}
