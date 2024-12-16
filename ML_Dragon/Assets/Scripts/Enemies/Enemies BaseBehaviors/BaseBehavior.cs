using UnityEngine;
using UnityEngine.AI;

public abstract class BaseBehavior
{
    protected BaseEnemy _enemy;
    protected Transform _player;
    protected EnemySettings _settings;
    protected NavMeshAgent _agent;
    protected Animator _animator;

    public BaseBehavior(BaseEnemy enemy, Transform player, EnemySettings settings, NavMeshAgent agent, Animator animator)
    {
        _enemy = enemy;
        _player = player;
        _settings = settings;
        _agent = agent;
        _animator = animator;
    }

    public abstract void Execute();
}