using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth { get; private set; }
    public int maxResist = 50;
    public int currentResist { get; private set; }

    public Stat damage;
    public Stat resist_damage;
    public Stat defense;

    void Awake()
    {
        currentHealth = maxHealth;
        currentResist = maxResist;
    }

    void Update()
    {
        //take damage logic
        if(Input.GetKeyDown(KeyCode.T))
        {
            TakeDamage(10);
        }
    }

    public void TakeDamage (int damage)
    {
        damage -= defense.GetValue();
        damage = Mathf.Clamp(damage, 0, int.MaxValue);
        currentHealth -= damage;

        if (currentHealth < 0) 
        {
            Die();
        }
    }

    public void TakeResistDamage(int att_resist)
    {
        att_resist = Mathf.Clamp(att_resist, 0, int.MaxValue);
        currentResist -= att_resist;

        // Trigger hit animation based on current resist
        var playerModel = GetComponent<PlayerModel>();
        /*
        if (playerModel == null)
        {
            Debug.Log("playerModel component not found on the GameObject.");
            return;
        }
        */
        playerModel?.TriggerHitAnimation();
    }

    public void RestoreResist()
    {
        currentResist = maxResist;
    }

    public virtual void Die()
    {
        //Debug.Log(transform.name + "have no more health");
    }
}
