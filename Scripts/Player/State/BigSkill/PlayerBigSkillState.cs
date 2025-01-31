using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ult state
public class PlayerBigSkillState : PlayerStateBase
{
    private bool isAttacking = false;
    private int hitCounter = 0;
    private float hitCoolDown = 0.3f;
    private float lastHitTime;

    public override void Enter()
    {
        base.Enter();

        //switch camera
        playerModel.bigSkillStartShot.SetActive(false);
        playerModel.bigSkillShot.SetActive(true);

        //afterswing
        playerController.PlayAnimation("BigSkill", 0.0f);
        // Find the closest enemy within range
        Transform closestEnemy = playerController.FindClosestEnemy(10f);//same as enemy see range
        if (closestEnemy != null)
        {
            playerController.RotateTowards(closestEnemy);
        }
        // Enable attack collider at the start of the attack
        playerController.EnableAttackCollider();
    }

    public override void Update()
    {
        base.Update();

        // Find the closest enemy within range
        Transform closestEnemy = playerController.FindClosestEnemy(10f);//same as enemy see range
        if (closestEnemy != null)
        {
            playerController.RotateTowards(closestEnemy);
        }

        #region detect actual attack range
        if (statePlayTime > 0.1f && (Time.time - lastHitTime > hitCoolDown))
        {
            isAttacking = true;
        }

        if (statePlayTime >= 2f)
        {
            isAttacking = false;
        }
        #endregion

        #region Whether the animation has finished playing
        if (IsAnimationEnd())
        {
            //end BigSkill
            playerController.SwitchState(PlayerState.BigSkillTransition);
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
        //if (!other.CompareTag("Enemy"))
        //Debug.Log("not tag");
        if (isAttacking && hitCounter < 5 && other.CompareTag("Enemy"))
        {
            //Debug.Log("Feel Enemy");
            // Detect if the enemy hit the player
            var enemyStats = other.GetComponent<CharacterStats>();
            if (enemyStats != null)
            {
                enemyStats.TakeDamage(playerController.playerStats.damage.GetValue()*3);
                enemyStats.TakeResistDamage(playerController.playerStats.resist_damage.GetValue()*3);
                hitCounter++;
                lastHitTime = Time.time;
                Debug.Log("Player hit the enemy!");
            }
            // Prevent multiple damage triggers during the same attack
            isAttacking = false;
            playerController.ApplyHitLag(0.12f);// hit lag
        }
    }

    /// <summary>
    /// Called when the state is exited.
    public override void Exit()
    {
        base.Exit();

        // Ensure the attack collider is disabled when exiting the state
        hitCounter = 0;
        playerController.DisableAttackCollider();
        isAttacking = false;
    }
}
