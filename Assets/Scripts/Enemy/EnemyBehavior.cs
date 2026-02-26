using UnityEngine;

public class EnemyBehavior : MonoBehaviour
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
    private bool IsAlive;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Attack(GameObject player)
    {
        
    }

    void Die()
    {
        IsAlive = false;
        Destroy(gameObject);
    }
}
