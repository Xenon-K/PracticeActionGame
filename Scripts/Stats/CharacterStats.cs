using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth { get; private set; }
    
    public Stat damage;
    public Stat defense;

    void Awake()
    {
        currentHealth = maxHealth;
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

    public virtual void Die()
    {
        //Die animation
        //Debug.Log(transform.name + "have no more health");
    }
}
