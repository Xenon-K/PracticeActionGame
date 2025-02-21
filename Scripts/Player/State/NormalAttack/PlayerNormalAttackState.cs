using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// attack state
/// </summary>
public class PlayerNormalAttackState : PlayerStateBase
{
    private bool isAttacking = false;
    private bool hitOnce = false;
    private bool enterNextAttack;
    //private Transform closestEnemy;

    public override void Enter()
    {
        base.Enter();

        playerModel.skiilConfig.HasComboed = false;

        enterNextAttack = false;

        // Find the closest enemy within range
        Transform closestEnemy = playerController.FindClosestEnemy(10f);//same as enemy see range
        if (closestEnemy != null)
        {
            //Debug.Log("See enemy: Normal Attack");
            playerController.RotateTowards(closestEnemy);
        }

        //play attack animation
        if (playerModel.skiilConfig.isPerfect)
        {
            playerController.PlayAnimation($"Attack_Normal_{playerModel.skiilConfig.currentNormalAttackIndex}_Perfect", 0.1f);
        }
        else
        {
            playerController.PlayAnimation("Attack_Normal_" + playerModel.skiilConfig.currentNormalAttackIndex, 0.1f);
        }
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

        #region detect hit
        if (playerModel.currentState == PlayerState.Hit)
        {
            //cancel looping state
            playerController.SwitchState(PlayerState.Hit);
            return;
        }
        #endregion

        #region detect ult
        if (playerController.inputSystem.Player.BigSkill.triggered && playerController.CheckUlt())
        {
            //cancel perfect combo
            playerModel.skiilConfig.isPerfect = false;
            // reset combo
            playerModel.skiilConfig.currentNormalAttackIndex = 1;
            //ult state
            playerController.SwitchState(PlayerState.BigSkillStart);
            return;
        }
        #endregion

        #region detect evade
        if (playerController.inputSystem.Player.Evade.triggered)
        {
            //cancel perfect combo
            playerModel.skiilConfig.isPerfect = false;
            //evade state
            if (playerController.inputMoveVec2.y > 0)
            {
                playerController.SwitchState(PlayerState.Evade_Front);
            }
            else
            {
                playerController.SwitchState(PlayerState.Evade_Back);
            }
            // reset combo
            playerModel.skiilConfig.currentNormalAttackIndex = 1;
            return;
        }
        #endregion

        // detect combo
        if (NormalizedTime() >= 0.5f && playerController.inputSystem.Player.Fire.triggered) 
        {
            enterNextAttack = true;
        }

        #region Whether the animation has finished playing
        if (IsAnimationEnd())
        {
            if (enterNextAttack)
            {
                // next attack 
                //Accumulate attack combos
                playerModel.skiilConfig.currentNormalAttackIndex++;
                if (playerModel.skiilConfig.currentNormalAttackIndex > playerModel.skiilConfig.normalAttackDamageMultiple.Length)
                {
                    // reset combo
                    playerModel.skiilConfig.currentNormalAttackIndex = 1;
                }

                //normal sttack state
                playerController.SwitchState(PlayerState.NormalAttack);
                return;
            }
            else
            {
                //afterswing for this combo
                playerController.SwitchState(PlayerState.NormalAttackEnd);
                return;
            }
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
                playerController.playerStats.GainEnergy(2);
                hitOnce = true;
                //if it is a perfect branch attack or it is the last normal attack stage
                if (playerModel.skiilConfig.isPerfect || playerModel.skiilConfig.currentNormalAttackIndex == playerModel.skiilConfig.normalAttackDamageMultiple.Length)
                {
                    var enemyController = other.GetComponent<EnemyController>();
                    if (enemyController.exChance > 0)
                    {
                        enemyController.usedExChance();
                        playerController.canEx = true;
                        playerController.ApplyGlobalSlowMotion(3f);
                        playerController.BroadcastCurrentOrder();
                        playerController.ChainUI();
                        playerController.ChargeUlt(100);
                    }
                }
                playerController.ApplyHitLag(0.05f); // hit lag
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
