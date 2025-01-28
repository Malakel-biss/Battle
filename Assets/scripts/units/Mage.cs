using System.Collections;
using UnityEngine;

public class Mage : Unit
{
    private bool canUseSpecialAbility = true; // To track cooldown status

    public override void Start()
    {
        base.Start();  // Calls the start method of the Unit class to initialize the animator
    }

    void Awake()
    {
        // Set the mage's stats
        hp = 60f;
        maxHp = 60;
        damage = 20;
        attackInterval = 6f;
        armor = 5;
        mana = 0f;
        maxMana = 200f;
        manaRegenRate = 5f;
        cost = 50;
        goldReward = 40;
    }

    public override void SpecialAbility()
    {
        // Check if the Mage has enough mana and the ability is off cooldown
        if (!canUseSpecialAbility)
        {
            Debug.Log($"{name} cannot cast Fireball yet! Cooldown in effect.");
            return;
        }

        if (mana < 100f)
        {
            Debug.Log($"{name} does not have enough mana to cast Fireball!");
            return;
        }

        // Perform the special ability
        StartCoroutine(CastFireball());
    }

    private IEnumerator CastFireball()
    {
        // Deduct mana cost
        mana -= 100f;

        // Trigger the fireball animation
        animator.SetTrigger("ability4");

        Debug.Log($"{name} casts Fireball!");

        // Simulate AoE damage and visualize the area of effect
        Vector3 center = transform.position;
        float radius = 3f; // Radius of the AoE

        // Debug visualization in the Scene View
        Debug.DrawRay(center, Vector3.up * radius, Color.red, 3f); // Ray upwards for visualization
        Debug.Log($"Fireball AoE visualized at {center} with radius {radius}");

        // Check enemies within range from the GameManager's enemyUnits list
        foreach (Unit enemy in GameManager.instance.enemyUnits)
        {
            if (enemy == null) continue;

            float distance = Vector3.Distance(center, enemy.transform.position);
            if (distance <= radius)
            {
                // Apply damage
                enemy.TakeDamage(60);
                Debug.Log($"{name} dealt 60 damage to {enemy.name} with Fireball.");
            }
        }

        // Start the cooldown timer
        canUseSpecialAbility = false;
        yield return new WaitForSeconds(6f); // Cooldown period
        animator.ResetTrigger("ability4");
        canUseSpecialAbility = true;

        Debug.Log($"{name}'s Fireball is ready to use again!");
    }

    void Update()
    {
        // Check for player input to trigger the special ability
        if (Input.GetKeyDown(KeyCode.N))
        {
            Mage mage = GetComponent<Mage>();
            if (mage != null)
            {
                mage.SpecialAbility();
            }
        }
    }

    // Optional: Visualize the Fireball AoE in the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 3f); // Radius of the Fireball AoE
    }

    protected override IEnumerator Attack(Unit target)
    {
        animator.SetBool("isattacking4", true);
        isAttacking = true;

        while (target != null && target.hp > 0 && hp > 0)
        {
            target.TakeDamage(damage); // Deal damage to the target
            Debug.Log($"{name} (Team {team}) attacks {target.name} for {damage} damage!");
            yield return new WaitForSeconds(attackInterval); // Wait for the next attack

        }

        isAttacking = false;
        animator.SetBool("isattacking4", false);
    }
}
