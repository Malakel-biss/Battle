using System.Collections;
using UnityEngine;

public class Mage : Unit
{
    public override void Start()
    {
        base.Start();  // Calls the start method of the Unit class to initialize the animator
    }

    void Awake()
    {
        // Set the mage's stats
        hp = 60f;
        maxHp = 60;
        damage = 30;
        attackInterval = 4f;
        armor = 5;
        mana = 0f;
        maxMana = 200f;
        manaRegenRate = 20f;
        cost = 50;
        goldReward = 40;
    }

    public override void SpecialAbility()
    {
        // Perform the special ability
        StartCoroutine(CastFireball());
    }

    private IEnumerator CastFireball()
    {
        // Deduct mana cost
        mana = 0f; // Reset mana after using the ability

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

        animator.ResetTrigger("ability4");
        yield return new WaitForSeconds(1); // Wait 1 second between attacks
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