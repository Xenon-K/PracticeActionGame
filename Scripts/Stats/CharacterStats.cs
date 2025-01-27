using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth { get; private set; }
    public int maxEnergy = 100;
    public int currentEnergy { get; private set; }
    public int maxResist = 50;
    public int currentResist { get; private set; }

    public Stat damage;
    public Stat resist_damage;
    public Stat defense;
    public Stat instantUse;
    public Stat constantUse;

    void Awake()
    {
        currentHealth = maxHealth;
        currentResist = maxResist;
        currentEnergy = maxEnergy / 2;
        Debug.Log($"{gameObject.name}, health = {currentHealth}");
    }

    void Update()
    {
        //debug testing
        if(Input.GetKeyDown(KeyCode.T))
        {
            TakeDamage(10);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            GainEnergy(100);
        }
    }

    //use for off field energy gain and normal attack energy gain
    public void GainEnergy(int energy)
    {
        currentEnergy += energy;

        if (currentEnergy > maxEnergy)
        {
            currentEnergy = maxEnergy;
        }
    }

    public bool useInstantSkill()
    {
        if (currentEnergy < instantUse.GetValue())
        {
            return false;//can not use this skill
        }

        currentEnergy -= instantUse.GetValue();
        return true;
    }

    public bool useConstantSkill()
    {
        if (currentEnergy < constantUse.GetValue())
        {
            return false;//can not use this skill
        }
        currentEnergy -= constantUse.GetValue();
        return true;
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
