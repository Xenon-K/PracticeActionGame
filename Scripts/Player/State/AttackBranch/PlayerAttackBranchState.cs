using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attack branch state
public class PlayerAttackBranchState : PlayerStateBase
{
    private bool isAttacking = false;
    private bool hitOnce = false;
    private bool hasEnergy = false;
    public override void Enter()
    {
        base.Enter();
        //check if can use perfect skill
        hasEnergy = playerController.playerStats.useInstantSkill();
        //play attack animation
        string moveName = "Attack_Branch_" + playerModel.skiilConfig.currentNormalAttackIndex;
        if (hasEnergy) 
        {
            moveName += "_Perfect";
        }
        int successPlayed = playerController.PlayAnimation(moveName, 0.1f);

        // Find the closest enemy within range
        Transform closestEnemy = playerController.FindClosestEnemy(10f);//same as enemy see range
        if (closestEnemy != null)
        {
            //Debug.Log("See enemy: Branch");
            playerController.RotateTowards(closestEnemy);
        }

        if (successPlayed < 0) //not found is -1
        {
            // reset combo
            playerModel.skiilConfig.currentNormalAttackIndex = 1;
            if(hasEnergy)
            {
                playerController.PlayAnimation("Attack_Branch_0_Perfect", 0.1f);
            }
            else
            {
                playerController.PlayAnimation("Attack_Branch_0", 0.1f);
            }
        }
        else
        {
            playerModel.skiilConfig.isPerfect = true;
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

        #region detect combo
        if (playerController.inputSystem.Player.Fire.triggered)
        {
            playerModel.skiilConfig.HasComboed = true;
        }
        #endregion

        #region detect ult
        if (playerController.inputSystem.Player.BigSkill.triggered && playerController.CheckUlt())
        {
            //ult state
            playerController.SwitchState(PlayerState.BigSkillStart);
            return;
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

        #region Whether the animation has finished playing
        if (IsAnimationEnd())
        {
            #region detect attack
            if (playerController.inputSystem.Player.Fire.triggered || playerModel.skiilConfig.HasComboed)
            {
                if (!playerModel.skiilConfig.isPerfect)
                {
                    // reset combo
                    playerModel.skiilConfig.currentNormalAttackIndex = 1;
                }
                else
                {
                    //Accumulate attack combos
                    playerModel.skiilConfig.currentNormalAttackIndex++;
                }
                //back to normal attack
                playerController.SwitchState(PlayerState.NormalAttack);
                return;
            }
            #endregion
            if(hasEnergy && (playerModel.animator.HasState(0, Animator.StringToHash($"Attack_Branch_{playerModel.skiilConfig.currentNormalAttackIndex}_Loop")) ||
               playerModel.animator.HasState(0, Animator.StringToHash("Attack_Branch_0_Loop"))))
            {
                //detect charge attack
                playerController.SwitchState(PlayerState.Attack_Branch_Loop);
                return;
            }
            //detect end phase
            else
            {
                if(hasEnergy)
                {
                    playerController.SwitchState(PlayerState.Attack_Branch_Perfect_End);
                    return;
                }
                else
                {
                    playerController.SwitchState(PlayerState.Attack_Branch_End);
                    return;
                }
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
                if (hasEnergy)
                {
                    enemyStats.TakeDamage(playerController.playerStats.damage.GetValue()*2);
                    enemyStats.TakeResistDamage(playerController.playerStats.resist_damage.GetValue()*2);
                }
                else
                {
                    enemyStats.TakeDamage(playerController.playerStats.damage.GetValue());
                    enemyStats.TakeResistDamage(playerController.playerStats.resist_damage.GetValue());
                }
                hitOnce = true;
                var enemyController = other.GetComponent<EnemyController>();
                if (enemyController.exChance > 0)
                {
                    enemyController.usedExChance();
                    playerController.canEx = true;
                    playerController.ApplyGlobalSlowMotion(3f);
                    playerController.BroadcastCurrentOrder();
                    playerController.ChainUI();
                    playerController.ChargeUlt(300);
                }
                Debug.Log("Player hit the enemy!");
            }
            // Prevent multiple damage triggers during the same attack
            isAttacking = false;
            playerController.ApplyHitLag(0.07f); // hit lag
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
