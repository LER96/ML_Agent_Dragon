using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IdleBehavior : BaseBehavior
{
    private Animator _animator;
    
    public IdleBehavior(BaseEnemy enemy, Transform player, EnemySettings settings, NavMeshAgent agent, Animator animator) : base(enemy, player, settings, agent, animator)
    {
        animator.SetTrigger("Idle");
    }

    public override void Execute()
    {
    }
}
