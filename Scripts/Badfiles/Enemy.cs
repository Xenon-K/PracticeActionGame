using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Interactable
{
    PlayerManager playerManager;
    CharacterStats myStats;
    // Start is called before the first frame update
    void Start()
    {
        //playerManager = PlayerManager.instance;
        myStats = GetComponent<CharacterStats>();
    }

    // Update is called once per frames
    void Update()
    {
        
    }

    public override void Interact()
    {
        base.Interact();
        EnemyCombat playerCombat = playerManager.player.GetComponent<EnemyCombat>();
        if (playerCombat != null)
        {
            playerCombat.Attack(myStats);
        }
    }
}
