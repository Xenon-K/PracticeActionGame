using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySkillState : EnemyStateBase
{
    private bool isAttacking = false;
    private float lastHitTime = 0f;
    private float hitCoolDown = 0.5f;
    private float animationDuration;
    private string animationName;
    /// <summary>
    /// Called when the state is entered.
    public override void Enter()
    {
        base.Enter();

        // Generate a random index between 1 and 6
        int randomIndex = UnityEngine.Random.Range(1, enemyController.skillSet+1);

        // Construct the animation name
        animationName = $"Skill{randomIndex}";

        // Play the randomly selected animation
        enemyController.PlayAnimation(animationName, 0.25f);

        // Enable attack collider at the start of the attack
        enemyController.EnableAttackCollider();
    }

    public override void Update()
    {
        base.Update();

        animationDuration = enemyModel.animator.GetCurrentAnimatorStateInfo(0).length; // Get animation duration

        #region detect stun
        if (enemyController.enemyStats.currentResist <= 0)
        {
            // Transition to the Death state
            enemyController.SwitchState(EnemyState.Stun_Start);
            return;
        }
        #endregion

        #region detect death
        if (enemyController.enemyStats.currentHealth <= 0)
        {
            // Transition to the Death state
            enemyController.SwitchState(EnemyState.Death);
            return;
        }
        #endregion

        #region detect actual attack range
        if(enemyController.ObjectCheck())
        {
            if (enemyController.SkillRange(animationName, statePlayTime) && (Time.time - lastHitTime > hitCoolDown))
            {
                isAttacking = true;
            }
        }
        else
        {
            if (statePlayTime > 1.3f && (Time.time - lastHitTime > hitCoolDown))
            {
                isAttacking = true;
            }
            if(statePlayTime > animationDuration - 3f)
            {
                isAttacking = false;
            }
        }
        #endregion

        // Check if the attack animation has ended
        if (IsAnimationEnd())
        {
            // Back to idle
            enemyController.DisableAttackCollider();
            isAttacking = false;
            statePlayTime = 0;
            enemyController.SwitchState(EnemyState.Idle);
            return;
        }
    }

    /// <summary>
    /// Handle collision detection for dealing damage.
    public void OnAttackHit(Collider other)
    {
        //Debug.Log($"OnAttackHit triggered with {other.name} tagged as {other.tag}");
        //Debug.Log($"OnAttackHit triggered with {hitCounter}");
        if (isAttacking && other.CompareTag("Player"))// Detect if the enemy hit the player, and we only hit once
        {
            // Detect if the enemy hit the player
            var playerModelState = other.GetComponent<PlayerModel>();
            var playerStats = other.GetComponent<CharacterStats>();
            if (playerStats != null)
            {
                if (playerModelState != null)
                {
                    // Check if the player is evading
                    if (playerModelState.currentState == PlayerState.Evade_Front || playerModelState.currentState == PlayerState.Evade_Back)
                    {
                        playerModelState.fightBack = true;
                        // Trigger special effect for evade
                        TriggerEvadeEffect(playerModelState);
                    }
                    else if (playerModelState.currentState == PlayerState.BigSkillStart || playerModelState.currentState == PlayerState.BigSkill || playerModelState.currentState == PlayerState.SwitchInAttack || playerModelState.currentState == PlayerState.SwitchInAttackEx)
                    {
                        ;//can not get hit during ult or followUp attack
                    }
                    else
                    {
                        // If not evading, apply damage
                        playerStats.TakeDamage(enemyController.enemyStats.damage.GetValue()/3);
                        playerStats.TakeResistDamage(enemyController.enemyStats.resist_damage.GetValue()/3);
                    }
                    // **Update lastHitTime to enforce cooldown**
                    lastHitTime = Time.time;
                    //playerStats.TakeDamage(enemyController.enemyStats.damage.GetValue());
                    isAttacking = false;
                    //Debug.Log("Enemy hit the player!");
                }

            }
            //enemyController.DisableAttackCollider();
        }
    }


    /// <summary>
    /// Trigger a special effect when the player is evading.
    /// <param name="playerModelState">The player's model state.</param>
    private void TriggerEvadeEffect(PlayerModel playerModelState)
    {
        //Debug.Log($"Player is evading with state: {playerModelState.currentState}");

        // Start slow motion for the enemy
        enemyController.ApplySlowMotion(3f);
    }

    /// <summary>
    /// Called when the state is exited.
    public override void Exit()
    {
        base.Exit();

        // Ensure the attack collider is disabled when exiting the state
        enemyController.DisableAttackCollider();
        isAttacking = false;
    }
}