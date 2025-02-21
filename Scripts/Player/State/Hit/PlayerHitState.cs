using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitState : PlayerStateBase
{
    float hitTimer = 0f;
    private string currentAnimation;
    private float lastHitTime = 0f; // Add a timer

    public override void Enter()
    {
        base.Enter();
        PlayHitAnimation();
    }

    private void PlayHitAnimation()
    {
        string animationName = "Hit";

        var playerStats = playerModel.GetComponent<CharacterStats>();
        Transform closestEnemy = playerController.FindClosestEnemy(10f);
        Vector3 directionToEnemy = (closestEnemy.position - playerModel.transform.position).normalized;
        float dotProduct = Vector3.Dot(playerModel.transform.forward, directionToEnemy);

        if (playerStats.currentResist > playerStats.maxResist * 0.5f)
        {
            animationName += "_L";
            hitTimer = 0.05f;
            playerController.ChargeUlt(-200);
        }
        else if (playerStats.currentResist > 0)
        {
            animationName += "_H";
            hitTimer = 0.08f;
            playerController.ChargeUlt(-300);
        }
        else
        {
            playerStats.RestoreResist();
            animationName += "Fly";
            hitTimer = 0.12f;
            playerController.ChargeUlt(-500);
        }

        animationName += (dotProduct > 0.5f) ? "_Front" : "_Back";

        playerController.PlayAnimation(animationName, 0.25f);
        currentAnimation = animationName;
        playerController.ApplyHitLag(hitTimer);

        lastHitTime = Time.time; // Update the last hit time each time an animation plays
    }

    public override void Update()
    {
        base.Update();

        if (IsAnimationEnd())
        {
            playerController.SwitchState(PlayerState.Idle);
            return;
        }
    }

    public void OnHitAgain()
    {
        // Only allow re-trigger if at least a short duration has passed
        if (Time.time - lastHitTime > 0.2f) // Adjust the threshold as needed
        {
            PlayHitAnimation(); // Restart animation when hit again
        }
    }
}
