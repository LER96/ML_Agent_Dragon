using UnityEngine;
using UnityEngine.AI;

public class SearchBehavior : BaseBehavior
{
    private float _startingSearchStartTime;

    public SearchBehavior(BaseEnemy enemy, Transform player, EnemySettings settings,NavMeshAgent agent, Animator animator) 
        : base(enemy, player, settings,agent, animator)
    {
        _startingSearchStartTime = Time.time;
        _agent.isStopped = true;
        _animator.SetTrigger("Search");
    }

    public override void Execute()
    {
        if (Time.time - _startingSearchStartTime > _settings.searchDuration)
        {
            _enemy.GetComponent<EnemyBehaviorHandler>().OnEnemyFinishSearch();
        }
        else
        {
            // Maybe search logic?
        }
    }
}