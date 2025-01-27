using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwitchInAttackExState : PlayerStateBase
{
    private bool isAttacking = false;
    private bool hitOnce = false;
    public override void Enter()
    {
        playerController.DisableChainUI();
        playerController.RestoreGlobalTimeScale();
        playerController.canEx = false;
        base.Enter();
        // Find the closest enemy within range
        Transform closestEnemy = playerController.FindClosestEnemy(20f);//same as enemy see range
        if (closestEnemy != null)
        {
            //Debug.Log("See enemy: Branch");
            playerController.RotateTowards(closestEnemy);
        }

        playerController.PlayAnimation("SwitchIn_Attack_Ex", 0f);
        // Enable attack collider at the start of the attack
        playerController.EnableAttackCollider();
    }

    public override void Update()
    {
        base.Update();

        #region detect actual attack range
        if (statePlayTime > 0.1f && statePlayTime < 2f)
        {
            isAttacking = true;

            Transform closestEnemy = playerController.FindClosestEnemy(10f);
            if (closestEnemy != null)
            {
                playerController.RotateTowards(closestEnemy);
            }
        }

        if (statePlayTime >= 2f)
        {
            isAttacking = false;
        }
        #endregion

        #region Whether the animation has finished playing
        if (IsAnimationEnd())
        {
            //back to idle
            playerController.SwitchState(PlayerState.SwitchInAttackExEnd);
            return;
        }
        #endregion
    }

    /// <summary>
    /// Handle collision detection for dealing damage.
    public void OnAttackHit(Collider other)
    {
        //Debug.Log($"OnAttackHit triggered with {other.name} tagged as {other.tag}");
        //if (!isAttacking)
        //Debug.Log("is not attacking");
        //if (hitOnce)
        //Debug.Log("hit");
        //if (!other.CompareTag("Enemy"))
        //Debug.Log("not tag");
        if (isAttacking && !hitOnce && other.CompareTag("Enemy"))
        {
            //Debug.Log("Feel Enemy");
            // Detect if the enemy hit the player
            var enemyStats = other.GetComponent<CharacterStats>();
            if (enemyStats != null)
            {
                enemyStats.TakeDamage(playerController.playerStats.damage.GetValue());
                enemyStats.TakeResistDamage(playerController.playerStats.resist_damage.GetValue());
                playerController.playerStats.GainEnergy(3);
                hitOnce = true;
                var enemyController = other.GetComponent<EnemyController>();
                if (enemyController.exChance > 0)
                {
                    enemyController.usedExChance();
                    playerController.canEx = true;
                    playerController.ApplyGlobalSlowMotion(3f);
                    playerController.BroadcastCurrentOrder();
                    playerController.ChainUI();
                    playerController.ChargeUlt(800);
                }
                Debug.Log("Player hit the enemy!");
            }

            // Prevent multiple damage triggers during the same attack
            isAttacking = false;
            playerController.DisableAttackCollider();
        }
    }

    /// <summary>
    /// Called when the state is exited.
    public override void Exit()
    {
        base.Exit();

        // Ensure the attack collider is disabled when exiting the state
        hitOnce = false;
        playerController.DisableAttackCollider();
        isAttacking = false;
    }
}
