using System.Collections;
using UnityEngine;
using UnityEngine.Splines;

public class EnemyBehavior : MonoBehaviour, IDamageable
{
    [Header("Attack Data")]
    public float AttackRange;
    public float Damage;
    public float DetectionRange;
    public PlayerData PlayerData;

    [Header("Enemy data")]
    public float MoveSpeed;
    public float Health;
    public float MaxHealth;
    private bool isAlive;


    void Attack(PlayerData player)
    {
        player.Health -= 20;
        StartCoroutine(WaitForAttack());
    }

    public void Die()
    {
        isAlive = false;
        Destroy(gameObject);
    }

    public void TakeDamage(float damage)
    {
        Health -= damage;

        if (Health <= 0)
        {
            isAlive = false;
            Die();
        }
    }

    bool IDamageable.IsAlive()
    {
        return isAlive;
    }

    IEnumerator WaitForAttack()
    {
        yield return new WaitForSeconds(.7f);

    }
}
