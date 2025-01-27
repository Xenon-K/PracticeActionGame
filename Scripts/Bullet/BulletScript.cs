using System.Collections;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    private float bulletLifetime;
    private float damageInterval;
    private int damagePerInterval;
    private bool hasHitEnemy = false;// Flag to stop movement when the bullet hits an enemy
    public EnemyController enemyController;
    private PlayerController playerController; // To store the reference passed from NicoleFire

    public void SetProperties(PlayerController controller, float lifetime, float interval, int damage)
    {
        playerController = controller;
        bulletLifetime = lifetime;
        damageInterval = interval;
        damagePerInterval = damage;

        // Automatically destroy the bullet after its lifetime
        Destroy(gameObject, bulletLifetime);
    }

    void Update()
    {
        // Move the bullet forward
        if (!hasHitEnemy)
        {
            transform.Translate(Vector3.forward * 10f * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemyController = other.GetComponent<EnemyController>();
            hasHitEnemy = true;
            StartCoroutine(DamageOverTime(other.gameObject));
            // Access the PlayerController on the parent and enter the SwitchInAttackEx state
            TriggerSwitchInAttackEx();
        }
    }

    private IEnumerator DamageOverTime(GameObject enemy)
    {
        CharacterStats enemyStats = enemy.GetComponent<CharacterStats>();
        if (enemyStats != null)
        {
            for (float timer = 0; timer < bulletLifetime; timer += damageInterval)
            {
                enemyStats.TakeDamage(damagePerInterval);
                yield return new WaitForSeconds(damageInterval);
            }
        }

        Destroy(gameObject);
    }

    private void TriggerSwitchInAttackEx()
    {
        if (playerController != null)
        {
            playerController.EnterSwitchInAttackEx(enemyController); // Call the method to enter the special state
        }
        else
        {
            Debug.LogError("PlayerController not found on the parent object!");
        }
    }
}
