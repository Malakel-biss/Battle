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

        // Clear current cell's `placedUnit`
        if (currentCell != null)
        {
            currentCell.placedUnit = null;
            currentCell.isOccupied = false;
            currentCell.isReserved = false;
        }

        targetCell.isOccupied = true;

        // Set the target cell as the current cell
        currentCell = targetCell;

        // Notify neighbors about the move
        targetCell.NotifyNeighbors(this);

        Vector3 destination = new Vector3(targetCell.transform.position.x, transform.position.y, targetCell.transform.position.z);
        navMeshAgent.SetDestination(destination);

        StartCoroutine(WaitForArrival(targetCell));
    }

    private IEnumerator WaitForArrival(GridCell targetCell)
    {
        float timeout = Vector3.Distance(transform.position, targetCell.transform.position) / speed + 2f; // Add buffer time
        float elapsedTime = 0f;

        navMeshAgent.isStopped = false;

        while (elapsedTime < timeout)
        {
            elapsedTime += Time.deltaTime;

            if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                break;
            }

            yield return null;
        }

        if (elapsedTime >= timeout)
        {
            RecoverFromStuckState();
            yield break;
        }

        navMeshAgent.isStopped = true;

        isMoving = false;
        animator.SetBool("iswalking", false);

        PerformAIActions(); // Trigger the next AI action
    }

    public void Update()
    {
        if (team == Team.Enemy && !isAttacking && !isMoving) return; // Prevent unnecessary updates for stationary enemies

        if (Time.time - lastActionTime > 3f) // 3-second recovery timer
        {
            Debug.LogWarning($"{name} is stuck. Resetting states.");
            RecoverFromStuckState();
        }

        if (!isAttacking && !isMoving)
        {
            PerformAIActions();
        }
    }

    private void RecoverFromStuckState()
    {
        if (team == Team.Enemy) return; // Prevent recovery attempts for enemy units

        isMoving = false;
        isAttacking = false;
        currentTarget = null;

        animator.SetBool("iswalking", false);
        animator.SetBool("isattacking", false);

        if (navMeshAgent != null)
        {
            navMeshAgent.isStopped = true;
            navMeshAgent.ResetPath();
        }

        PerformAIActions();
    }

    private void CheckForEnemiesAndAttack()
    {
        if (currentCell == null || isAttacking || currentCell.isseletioncell) return;

        if (currentTarget != null && currentTarget.hp > 0)
        {
            float distanceToTarget = Vector3.Distance(transform.position, currentTarget.transform.position);
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

                    float distanceToEnemy = Vector3.Distance(transform.position, enemyUnit.transform.position);
                    if (distanceToEnemy <= 1.5f)
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
            if (target == null)
            {
                Debug.LogWarning($"{name}: Target has been destroyed during the attack.");
                break;
            }

            target.TakeDamage(damage);
            Debug.Log($"{name} (Team {team}) attacks {target.name} for {damage} damage!");
            yield return new WaitForSeconds(attackInterval);
        }

        isAttacking = false;
        animator.SetBool("isattacking", false);

        if (target == null || target.hp <= 0)
        {
            currentTarget = null;
            PerformAIActions();
        }
    }

    public virtual void TakeDamage(int damageAmount)
    {
        float effectiveDamage = damageAmount * (100f / (100f + armor));
        hp -= effectiveDamage;

        UpdateHealthBar();
        if (hp <= 0)
        {
            animator.SetBool("isdead", true);
            Die();
        }
    }

    protected void Die()
    {
        StopAllCoroutines();

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

        if (team == Team.Enemy)
        {
            GameManager.instance.enemyUnits.Remove(this);

            GameManager.instance.playergold += goldReward;
            Debug.Log($"Player earned {goldReward} gold! Total: {GameManager.instance.playergold}");
        }
        else if (team == Team.Player)
        {
            GameManager.instance.playerUnits.Remove(this);
        }

        if (healthBarInstance != null)
        {
            Destroy(healthBarInstance.gameObject);
        }

        currentCell?.ClearUnit();
        Destroy(gameObject);
    }

    public void StartCombatWith(Unit enemyUnit)
    {
        lastActionTime = Time.time;
        if (enemyUnit != null && enemyUnit.hp > 0 && hp > 0)
        {
            animator.SetBool("iswalking", false);
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
                    healthBarInstance.SetMana(mana, maxMana);
                }

                if (mana >= maxMana)
                {
                    SpecialAbility();
                    mana = 0;

                    if (healthBarInstance != null)
                    {
                        healthBarInstance.ResetManaBar();
                    }
                }
            }
            yield return null;
        }
    }

    public virtual void SpecialAbility()
    {
    }

    private Unit FindClosestEnemy()
    {
        Unit closestEnemy = null;
        float minDistance = float.MaxValue;

        List<Unit> enemyUnits = team == Team.Player ? GameManager.instance.enemyUnits : GameManager.instance.playerUnits;

        foreach (Unit enemy in enemyUnits)
        {
            if (enemy == null || enemy.hp <= 0 || enemy.isMoving) continue;

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

        return closestNeighbor;
    }

    public void PerformAIActions()
    {
        if (team == Team.Enemy && !isAttacking) return; // Skip AI actions for stationary enemies

        if (isMoving || isAttacking || navMeshAgent.pathPending) return;

        lastActionTime = Time.time;

        if (currentCell.isseletioncell) return;

        Unit closestEnemy = FindClosestEnemy();
        if (closestEnemy == null)
        {
            Debug.Log($"{name} found no enemies.");
            return;
        }

        if (currentCell.neighbors.Contains(closestEnemy.currentCell))
        {
            Debug.Log($"{name} is in range to attack {closestEnemy.name}");
            StartCombatWith(closestEnemy);
            return;
        }

        if (team == Team.Player)
        {
            GridCell targetCell = FindNeighboringCell(closestEnemy.currentCell);
            if (targetCell != null && targetCell != currentCell)
            {
                Debug.Log($"{name} is moving towards {targetCell.x}, {targetCell.z}");
                MoveTo(targetCell);
            }
            else
            {
                Debug.LogWarning($"{name} could not find a valid cell to move closer to {closestEnemy.name}.");
                StartCoroutine(RecoverAndRetry());
                return;
            }
        }
    }

    private IEnumerator RecoverAndRetry()
    {
        yield return new WaitForSeconds(1f);

        Debug.Log($"{name} attempting recovery action.");
        PerformAIActions();
    }

    void OnDrawGizmos()
    {
        if (navMeshAgent != null && navMeshAgent.hasPath)
        {
            Gizmos.color = Color.blue;

            Vector3 previousCorner = navMeshAgent.path.corners[0];
            foreach (Vector3 corner in navMeshAgent.path.corners)
            {
                Gizmos.DrawLine(previousCorner, corner);
                previousCorner = corner;
            }

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(navMeshAgent.destination, 0.2f);
        }
    }


    public List<Item> inventory = new List<Item>();
    private Item equippedItem = null; // The currently equipped item

    public void BuyItem(Item item, ref int playerGold)
    {
        if (playerGold >= item.cost)
        {
            inventory.Add(item);
            playerGold -= item.cost;
            Debug.Log($"{name} bought {item.name}!");
        }
        else
        {
            Debug.Log("Not enough gold to buy this item.");
        }
    }

    public void EquipItem(Item item)
    {
        if (inventory.Contains(item))
        {
            // Unequip the current item, if any
            if (equippedItem != null)
            {
                damage -= equippedItem.bonusDamage;
                armor -= equippedItem.bonusArmor;
                maxHp -= equippedItem.bonusHealth;
                hp -= item.bonusHealth;
            }

            // Equip the new item
            equippedItem = item;
            damage += item.bonusDamage;
            armor += item.bonusArmor;
            maxHp += item.bonusHealth;
            hp += item.bonusHealth;

            Debug.Log($"{name} equipped {item.name}. Damage: {damage}, Armor: {armor}, Max HP: {maxHp}");

            // Update the indicator
            UpdateEquippedIndicator();
        }
        else
        {
            Debug.Log($"{name} does not have {item.name} in their inventory.");
        }
    }


    public GameObject equippedIndicatorPrefab; // Drag the red circle prefab in Unity
    private GameObject equippedIndicatorInstance; // Stores the spawned indicator

    // Add this method to update the indicator
    public void UpdateEquippedIndicator()
    {
        if (equippedItem != null) // If the unit has an item
        {
            if (equippedIndicatorInstance == null) // If the indicator doesn't exist, create it
            {
                equippedIndicatorInstance = Instantiate(equippedIndicatorPrefab, transform.position, Quaternion.identity, transform);
                equippedIndicatorInstance.transform.localPosition = new Vector3(0, 0.1f, 0); // Slightly above ground
            }
        }
        else // If no item is equipped, remove the indicator
        {
            if (equippedIndicatorInstance != null)
            {
                Destroy(equippedIndicatorInstance);
                equippedIndicatorInstance = null;
            }
        }
    }
}
