using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterStats))]
public class EnemyCombat : MonoBehaviour
{
    CharacterStats myStats;
    void Start ()
    {
        myStats = GetComponent<CharacterStats>();
    }
    public void Attack (CharacterStats targetStats)
    { 
        targetStats.TakeDamage(myStats.damage.GetValue());
    }
}
