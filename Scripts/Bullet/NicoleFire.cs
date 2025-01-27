using System.Collections;
using UnityEngine;

public class NicoleFire : MonoBehaviour
{
    public GameObject bulletPrefab; // Prefab of the bullet to be fired
    public Transform firePoint; // The point where the bullet is fired
    public float bulletLifetime = 3f; // How long the bullet lasts after hitting an enemy
    public float damageInterval = 0.5f; // Damage interval (every 0.5 seconds)
    public int damagePerInterval = 5; // Damage dealt per interval

    private PlayerModel playerModel;
    private bool hasFiredBullet = false; // Tracks if the bullet has been fired in the current state
    private float lastFireTime; // Tracks the last time a bullet was fired
    private float fireCooldown = 2f; // Time before the flag is reset

    void Start()
    {
        firePoint = GetComponent<Transform>();
        playerModel = GetComponent<PlayerModel>(); // Get the PlayerModel component
        if (playerModel == null)
        {
            Debug.LogError("PlayerModel component not found on the object!");
        }
    }

    void Update()
    {
        if(Time.time - lastFireTime > fireCooldown)
        {
            hasFiredBullet = false;//reset flag
        }
        // Check if the playerModel is in the BigSkill stage
        if (playerModel != null && 
           (playerModel.currentState == PlayerState.BigSkill || 
           playerModel.currentState == PlayerState.Attack_Branch_Explode ||
           playerModel.currentState == PlayerState.SwitchInAttack ||
           playerModel.currentState == PlayerState.SwitchInAttackEx ||
           playerModel.currentState == PlayerState.AttackRushBack))
        {
            if (!hasFiredBullet) // Fire only if the bullet hasn't been fired in the current state
            {
                FireBullet();
                hasFiredBullet = true; // Set the flag to prevent multiple bullets
                lastFireTime = Time.time;
            }
        }
        else
        {
            hasFiredBullet = false; // Reset the flag when exiting the state
        }
    }

    void FireBullet()
    {
        // Calculate a new position with Y + 1
        Vector3 firePosition = firePoint.position + new Vector3(0, 1f, 0);
        // Instantiate the bullet at the fire point
        GameObject bullet = Instantiate(bulletPrefab, firePosition, firePoint.rotation);

        PlayerController playerController = transform.parent.GetComponent<PlayerController>(); // Assuming the parent object has PlayerController
        // Get the BulletScript component and set its properties
        BulletScript bulletScript = bullet.GetComponent<BulletScript>();
        if (bulletScript != null)
        {
            bulletScript.SetProperties(playerController, bulletLifetime, damageInterval, damagePerInterval);
        }
    }
}
