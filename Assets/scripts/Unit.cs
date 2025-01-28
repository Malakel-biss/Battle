using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
    protected Animator animator;
    public HealthBar healthBarPrefab;
    private HealthBar healthBarInstance;
    public TextMeshProUGUI costText;

    public enum Team
    {
        Player,
        Enemy
    }

    [Header("Unit Stats")]
    public Team team; // The team this unit belongs to
    public float hp = 100f;
    public float maxHp = 100f; // Maximum health
    public int damage = 20;
    public float attackInterval = 1f;
    public int armor = 0;
    public float mana = 100f;
    public float maxMana = 100f; // Maximum mana
    public float manaRegenRate = 5f; // Mana regenerated per second
    public int cost = 50; // Cost to spawn this unit
    public int goldReward = 10; // Gold rewarded when this unit is killed

    public GridCell currentCell; // The cell the unit is currently on 
    public float speed = 5f;
    public bool isMoving = false; // Prevent overlapping movement calls
    public bool isAttacking = false; // Whether the unit is currently attacking
    private Unit currentTarget = null; // Keep track of the current target

    private NavMeshAgent navMeshAgent;

    private float lastActionTime; // Tracks the last time an action was performed

    public virtual void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.avoidancePriority = Random.Range(1, 100); // Randomize priority for better avoidance

        animator = GetComponent<Animator>();

        lastActionTime = Time.time;

        // Get all Renderer components in this GameObject and its children
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        // Start the coroutine to regenerate mana
        StartCoroutine(RegenerateMana());

        if (currentCell.isseletioncell == false)
        {
            if (team == Team.Player)
            {
                transform.rotation = Quaternion.Euler(0, -90, 0); // Facing negative X
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, 90, 0); // Facing positive X
            }

            // Instantiate health bar
            healthBarInstance = Instantiate(healthBarPrefab, transform.position + Vector3.up, Quaternion.identity, transform);  // Adjust position as needed
            UpdateHealthBar();
            costText.gameObject.SetActive(false);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 90, 0); // Facing positive X on selection cells
            costText.text = cost.ToString();
        }

    }

    protected void UpdateHealthBar()
    {
        if (healthBarInstance != null)
        {
            healthBarInstance.SetHealth(hp, maxHp);
        }
    }

    public void MoveTo(GridCell targetCell)
    {
        lastActionTime = Time.time;
        if (team == Team.Enemy)
        {
            return; // Prevent movement for enemy units
        }

        if (isMoving || targetCell.isOccupied || targetCell.isReserved)
        {
            Debug.LogWarning($"{name}: Cannot move to target cell (already moving or occupied).");
            return;
        }

        isMoving = true; // Set moving flag immediately

        // Reserve the target cell
        targetCell.isReserved = true;
        targetCell.placedUnit = currentCell.placedUnit; //***
        // Start movement
        // isMoving = true;
        // animator.SetBool("iswalking", true);

        // Clear current cell's `placedUnit`
        if (currentCell != null)
        {
            currentCell.placedUnit = null;
            currentCell.isOccupied = false;
            currentCell.isReserved = false;
        }
        // Update the target cell's state
        // targetCell.placedUnit = gameObject;


        targetCell.isOccupied = true;

        // Set the target cell as the current cell
        currentCell = targetCell;

        // Notify neighbors about the move
        targetCell.NotifyNeighbors(this);

        // Reset NavMeshAgent state
        // navMeshAgent.ResetPath();
        // navMeshAgent.isStopped = false;

        // // Use NavMeshAgent to move to the target cell
        Vector3 destination = new Vector3(targetCell.transform.position.x, transform.position.y, targetCell.transform.position.z);
        navMeshAgent.SetDestination(destination);

        StartCoroutine(WaitForArrival(targetCell));
    }




    private IEnumerator WaitForArrival(GridCell targetCell)
    {
        // float timeout = 5f; // Timeout duration to avoid infinite waiting
        float timeout = Vector3.Distance(transform.position, targetCell.transform.position) / speed + 2f; // Add buffer time
        float elapsedTime = 0f;

        // Ensure the agent is moving
        navMeshAgent.isStopped = false;

        while (elapsedTime < timeout)
        {
            elapsedTime += Time.deltaTime;

            // Check if the agent has reached the destination
            if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                break;
            }

            yield return null;
        }

        // Check if the unit failed to reach the destination
        if (elapsedTime >= timeout)
        {
            RecoverFromStuckState();
            yield break;
        }

        // Ensure the agent stops moving
        navMeshAgent.isStopped = true;

        isMoving = false;
        animator.SetBool("iswalking", false);

        PerformAIActions(); // Trigger the next AI action
    }


    public void Update()
    {
        // Log current states for debugging
        Debug.Log($"{name} - Moving: {isMoving}, Attacking: {isAttacking}, Target: {currentTarget?.name ?? "None"}");

        // Handle selection cells
        if (currentCell != null && currentCell.isseletioncell)
        {
            transform.rotation = Quaternion.Euler(0, 90, 0); // Always face positive X
            return;
        }

        // Automatically recover if idle or stuck for too long
        if (Time.time - lastActionTime > 3f) // 3-second recovery timer
        {
            Debug.LogWarning($"{name} is stuck. Resetting states.");
            RecoverFromStuckState();
        }


        // Check for enemies and perform actions if not busy
        if (!isAttacking && !isMoving)
        {
            PerformAIActions();
        }

        // if (!isAttacking)
        // {
        //     CheckForEnemiesAndAttack();
        // }
    }
    private void RecoverFromStuckState()
    {
        // Forcefully reset states
        isMoving = false;
        isAttacking = false;
        currentTarget = null;

        // Stop animations
        animator.SetBool("iswalking", false);
        animator.SetBool("isattacking", false);

        // Stop NavMeshAgent if used
        if (navMeshAgent != null)
        {
            navMeshAgent.isStopped = true;
            navMeshAgent.ResetPath();
        }
        // Immediately reattempt AI actions
        PerformAIActions();

        // Log recovery
        Debug.Log($"{name} has recovered from being stuck.");
    }



    private void CheckForEnemiesAndAttack() // Check for enemies in neighboring cells and attack
    {
        if (currentCell == null || isAttacking || currentCell.isseletioncell) return;

        if (currentTarget != null && currentTarget.hp > 0)
        {
            float distanceToTarget = Vector3.Distance(transform.position, currentTarget.transform.position);
            // Validate neighboring cells and distance
            if (currentCell.neighbors.Contains(currentTarget.currentCell) && distanceToTarget <= navMeshAgent.stoppingDistance)
            {
                StartCoroutine(Attack(currentTarget));
                return;
            }
            else
            {
                currentTarget = null;
            }
        }

        foreach (GridCell neighbor in currentCell.neighbors)
        {
            if (neighbor != null && neighbor.isOccupied && neighbor.placedUnit != null)
            {
                Unit enemyUnit = neighbor.placedUnit.GetComponent<Unit>();
                if (enemyUnit != null && enemyUnit.team != team && !enemyUnit.currentCell.isseletioncell)
                {
                    currentTarget = enemyUnit;

                    // float distanceToEnemy = Vector3.Distance(transform.position, enemyUnit.transform.position);
                    // if (distanceToEnemy <= navMeshAgent.stoppingDistance)
                    // {
                    //     StartCoroutine(Attack(currentTarget));
                    // }
                    float distanceToEnemy = Vector3.Distance(transform.position, enemyUnit.transform.position);
                    if (distanceToEnemy <= 1.5f) // Use a stricter range check
                    {
                        StartCoroutine(Attack(currentTarget));
                    }
                    return;
                }
            }
        }
    }



    protected virtual IEnumerator Attack(Unit target)
    {
        isAttacking = true;
        animator.SetBool("isattacking", true);

        while (target != null && target.hp > 0 && hp > 0 && !target.isMoving)
        {
            lastActionTime = Time.time;
            // Check if the target has been destroyed
            if (target == null)
            {
                Debug.LogWarning($"{name}: Target has been destroyed during the attack.");
                break;
            }

            target.TakeDamage(damage);
            Debug.Log($"{name} (Team {team}) attacks {target.name} for {damage} damage!");
            yield return new WaitForSeconds(attackInterval); // Wait before the next attack
        }

        // Reset attacking state
        isAttacking = false;
        animator.SetBool("isattacking", false);

        if (target == null || target.hp <= 0)
        {
            currentTarget = null; // Clear current target
            PerformAIActions(); // Trigger AI to find a new target
        }
    }



    public virtual void TakeDamage(int damageAmount)
    {
        float effectiveDamage = damageAmount * (100f / (100f + armor));
        hp -= effectiveDamage; // Reduce HP 
        //Debug.Log($"{name} takes {damageAmount} damage! Remaining HP: {hp}");

        UpdateHealthBar(); // Update the health bar
        if (hp <= 0)
        {
            animator.SetBool("isdead", true);
            Die();
        }
    }
    protected void Die()
    {
        // Stop all coroutines for this unit
        StopAllCoroutines();

        // If this unit was the target of another unit, stop that attack
        foreach (Unit unit in GameManager.instance.playerUnits)
        {
            if (unit.currentTarget == this)
            {
                unit.currentTarget = null;
                unit.isAttacking = false;
            }
        }

        foreach (Unit unit in GameManager.instance.enemyUnits)
        {
            if (unit.currentTarget == this)
            {
                unit.currentTarget = null;
                unit.isAttacking = false;
            }
        }

        Debug.Log($"{name} (Team {team}) has died!");

        // Remove the unit from the appropriate list
        if (team == Team.Enemy)
        {
            GameManager.instance.enemyUnits.Remove(this);

            // Increment player's gold when an enemy unit dies
            GameManager.instance.playergold += goldReward;
            Debug.Log($"Player earned {goldReward} gold! Total: {GameManager.instance.playergold}");
        }
        else if (team == Team.Player)
        {
            GameManager.instance.playerUnits.Remove(this);
        }

        if (healthBarInstance != null)
        {
            Destroy(healthBarInstance.gameObject); // Clean up the health bar
        }

        currentCell?.ClearUnit(); // Clear the unit from the grid cell safely
        Destroy(gameObject); // Destroy the unit object
    }


    public void StartCombatWith(Unit enemyUnit)
    {
        lastActionTime = Time.time;
        if (enemyUnit != null && enemyUnit.hp > 0 && hp > 0)
        {
            animator.SetBool("iswalking", false); // Stop walking animation
            StartCoroutine(Attack(enemyUnit));
        }
    }

    private IEnumerator RegenerateMana()
    {
        while (true)
        {
            if (mana < maxMana && GameManager.instance.currentPhase == GameManager.GamePhase.Battle && isAttacking)
            {
                mana += manaRegenRate * Time.deltaTime;
                mana = Mathf.Min(mana, maxMana);

                if (healthBarInstance != null)
                {
                    healthBarInstance.SetMana(mana, maxMana); // Update the mana bar
                }

                // Trigger special ability when mana is full
                if (mana >= maxMana)
                {
                    SpecialAbility();
                    mana = 0; // Reset mana after using the special ability

                    if (healthBarInstance != null)
                    {
                        healthBarInstance.ResetManaBar(); // Reset the mana bar visually
                    }
                }
            }
            yield return null; // Wait for the next frame
        }
    }


    public virtual void SpecialAbility()
    {
        // Placeholder for special ability
    }
    private Unit FindClosestEnemy()
    {
        Unit closestEnemy = null;
        float minDistance = float.MaxValue;

        List<Unit> enemyUnits = team == Team.Player ? GameManager.instance.enemyUnits : GameManager.instance.playerUnits;

        foreach (Unit enemy in enemyUnits)
        {
            if (enemy == null || enemy.hp <= 0 || enemy.isMoving) continue; // Skip null or dead enemies

            NavMeshPath path = new NavMeshPath();
            if (navMeshAgent.CalculatePath(enemy.transform.position, path) && path.status == NavMeshPathStatus.PathComplete)
            {

                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestEnemy = enemy;
                }
            }
        }

        return closestEnemy;
    }


    private GridCell FindNeighboringCell(GridCell enemyCell)
    {
        GridCell closestNeighbor = null;
        float shortestDistance = float.MaxValue;

        foreach (GridCell neighbor in enemyCell.neighbors)
        {
            if (neighbor != null && !neighbor.isOccupied && !neighbor.isReserved)
            {
                float distance = Vector3.Distance(transform.position, neighbor.transform.position);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    closestNeighbor = neighbor;
                }
            }
        }

        return closestNeighbor; // Return the closest valid neighboring cell
    }


    public void PerformAIActions()
    {
        if (isMoving || isAttacking || navMeshAgent.pathPending) return;
        else StartCoroutine(RecoverAndRetry());
        // Update the last action timestamp to prevent recovery
        lastActionTime = Time.time;

        if (currentCell.isseletioncell) return;

        // Find the closest enemy
        Unit closestEnemy = FindClosestEnemy();
        if (closestEnemy == null)
        {
            Debug.Log($"{name} found no enemies.");
            return;
        }

        // If already in a neighboring cell, start combat
        if (currentCell.neighbors.Contains(closestEnemy.currentCell))
        {
            Debug.Log($"{name} is in range to attack {closestEnemy.name}");
            StartCombatWith(closestEnemy);
            return;
        }

        if (team == Team.Player)
        {
            // Find the next cell closer to the enemy
            GridCell targetCell = FindNeighboringCell(closestEnemy.currentCell);
            if (targetCell != null && targetCell != currentCell)
            {
                Debug.Log($"{name} is moving towards {targetCell.x}, {targetCell.z}");
                MoveTo(targetCell);
            }
            else
            {
                Debug.LogWarning($"{name} could not find a valid cell to move closer to {closestEnemy.name}.");
                // Fallback: Force a re-evaluation of actions after a delay
                StartCoroutine(RecoverAndRetry());
                return;
            }
        }
    }

    private IEnumerator RecoverAndRetry()
    {
        // Small delay before re-evaluating actions to prevent immediate retries
        yield return new WaitForSeconds(1f);

        Debug.Log($"{name} attempting recovery action.");
        PerformAIActions(); // Retry performing AI actions
    }


    void OnDrawGizmos()
    {
        if (navMeshAgent != null && navMeshAgent.hasPath)
        {
            Gizmos.color = Color.blue;

            // Draw the path as a line connecting the corners
            Vector3 previousCorner = navMeshAgent.path.corners[0];
            foreach (Vector3 corner in navMeshAgent.path.corners)
            {
                Gizmos.DrawLine(previousCorner, corner);
                previousCorner = corner;
            }

            // Highlight the target destination
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(navMeshAgent.destination, 0.2f);
        }
    }
}
