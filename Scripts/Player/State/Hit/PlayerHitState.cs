using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitState : PlayerStateBase
{
    float hitTimer = 0f;
    public override void Enter()
    {
        base.Enter();

        string animationName = "Hit";

        #region set hit animation
        //find out resist
        var playerStats = playerModel.GetComponent<CharacterStats>();

        // Find the closest enemy within range
        Transform closestEnemy = playerController.FindClosestEnemy(10f);//same as enemy see range

        // Calculate the direction from the player to the enemy
        Vector3 directionToEnemy = (closestEnemy.position - playerModel.transform.position).normalized;

        // Check if the player is facing the enemy (using dot product)
        float dotProduct = Vector3.Dot(playerModel.transform.forward, directionToEnemy);

        if (playerStats.currentResist > playerStats.maxResist * 0.5f) 
        {
            animationName = animationName + "_L";
            hitTimer = 0.05f;
            playerController.ChargeUlt(-200);
        }
        else if(playerStats.currentResist > 0)
        {
            animationName = animationName + "_H";
            hitTimer = 0.08f;
            playerController.ChargeUlt(-300);
        }
        else if(playerStats.currentResist <= 0)
        {
            playerStats.RestoreResist();
            animationName = animationName + "Fly";
            hitTimer = 0.12f;
            playerController.ChargeUlt(-500);
        }

        if (dotProduct > 0.5f)//facing the enemy
        {
            animationName = animationName + "_Front";
        }
        else
        {
            animationName = animationName + "_Back";
        }
        #endregion
        //Debug.Log(animationName);
        playerController.PlayAnimation(animationName, 0.25f);
        playerController.ApplyHitLag(hitTimer);// hit lag
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        #region Whether the animation has finished playing
        if (IsAnimationEnd())
        {
            playerController.SwitchState(PlayerState.Idle);
            return;
        }
        #endregion
    }
}
