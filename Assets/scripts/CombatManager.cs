using UnityEngine;
using System.Collections.Generic;

public class CombatManager : MonoBehaviour
{
    public static CombatManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void HandleCombat(Unit attacker, Unit target)
    {
        if (attacker.hp > 0 && target.hp > 0)
        {
            target.TakeDamage(attacker.damage);
            Debug.Log($"{attacker.name} attacked {target.name} for {attacker.damage} damage!");
        }
    }
}
