using UnityEngine;
using UnityEngine.AI;

public class ChaseBehavior : BaseBehavior
{
    public ChaseBehavior(BaseEnemy enemy, Transform player, EnemySettings settings,NavMeshAgent agent, Animator animator) 
        : base(enemy, player, settings,agent, animator)
    {
        _agent.isStopped = false;
        _animator.SetTrigger("Chase");
        agent.speed = settings.chaseSpeed; 
    }

    public override void Execute()
    {
        _agent.destination = _player.position;
    }
}