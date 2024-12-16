using UnityEngine;
using UnityEngine.AI;

public class AttackBehavior : BaseBehavior
{
    private float attackCooldown = 1f;
    private float _attackRange;
    private float lastAttackTime;

    public AttackBehavior(BaseEnemy enemy, Transform player, float attackRange, EnemySettings settings,NavMeshAgent agent, Animator animator) 
        : base(enemy, player, settings, agent, animator)
    {
        attackCooldown = settings.attackCooldown;
        _attackRange = attackRange;
    }

    public override void Execute()
    {
        if (Vector3.Distance(_enemy.EnemyPosition, _player.position) <= _attackRange && Time.time > lastAttackTime + attackCooldown)
        {
            Vector3 direction = (_player.position - _enemy.EnemyPosition).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            _enemy.EnemyTransform.rotation = Quaternion.Slerp(_enemy.EnemyTransform.rotation, lookRotation, Time.deltaTime * 10f);
            
            
            _animator.SetTrigger("Attack");
            lastAttackTime = Time.time;
        }
    }
}