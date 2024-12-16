using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviorHandler : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private EnemySettings _settings;  
    [SerializeField] private Transform[] _patrolPoints;

    private Animator _animator;
    private BaseEnemy _baseEnemy;
    private Transform _playerTransform;
    private BaseBehavior _currentBehavior;
    private AlertOthers _alertOthers;

    private int _lastPatrolIndex = 0;

    void Update()
    {
        if (_currentBehavior==null)
            return;
        
        _currentBehavior.Execute();

        if (ShouldChasePlayer())
        {
            if(_currentBehavior is ChaseBehavior)
                return;

            if (_currentBehavior is PatrolBehavior)
            {
                var patrolBehavior = (PatrolBehavior)_currentBehavior;
                _lastPatrolIndex = patrolBehavior.LastPatrolIndex;
            }
            
            SetBehavior(new ChaseBehavior(_baseEnemy, _playerTransform, _settings,_agent,_animator));
            _alertOthers.AlertNearbyEnemies(_baseEnemy, _playerTransform, _settings);
        }
        else if (ShouldAttackPlayer())
        {
            if(_currentBehavior is AttackBehavior)
                return;
            
            SetBehavior(new AttackBehavior(_baseEnemy, _playerTransform, _settings.attackRange, _settings,_agent,_animator));
        }
        else if (ShouldSearchPlayer())
        {
            if(_currentBehavior is SearchBehavior)
                return;
            
            SetBehavior(new SearchBehavior(_baseEnemy, _playerTransform, _settings,_agent, _animator));
        }
    }


    public void InitData(BaseEnemy enemy,Animator animator)
    {
        _baseEnemy = enemy;
        _animator = animator;
        _playerTransform = GameManager.Instance.PlayerManager.transform;
        _alertOthers = new AlertOthers(_settings.alertRange);
        _agent.speed = _settings.patrolSpeed; 
        
        if(_patrolPoints.Length>0)
        SetBehavior(new PatrolBehavior(_baseEnemy, _playerTransform, _patrolPoints, _settings,_agent,_animator,_lastPatrolIndex));

        else
            SetBehavior(new IdleBehavior(_baseEnemy, _playerTransform, _settings,_agent,_animator));
    }
    
    public void SetBehavior(BaseBehavior newBehavior)
    {
        _currentBehavior = newBehavior;
    }

    public void OnEnemyFinishSearch()
    {
        if(_patrolPoints.Length>0)
        SetBehavior(new PatrolBehavior(_baseEnemy, _playerTransform, _patrolPoints, _settings,_agent,_animator,_lastPatrolIndex));
        
        else
            SetBehavior(new IdleBehavior(_baseEnemy, _playerTransform, _settings,_agent,_animator));
    }

    private bool CanSeePlayer()
    {
        if (Vector3.Distance(_baseEnemy.EnemyPosition, _playerTransform.position) <= _settings.sightRange)
        {
            return true;
        }
        return false;
    }

    private bool ShouldChasePlayer()
    {
        if (CanSeePlayer() && Vector3.Distance(_baseEnemy.EnemyPosition, _playerTransform.position) > _settings.attackRange)
            return true;

        return false;
    }
    
    private bool ShouldAttackPlayer()
    {
        if (Vector3.Distance(_baseEnemy.EnemyPosition, _playerTransform.position) <= _settings.attackRange)
            return true;

        return false;
    }
    
    private bool ShouldSearchPlayer()
    {
        if (!CanSeePlayer() && _currentBehavior.GetType() == typeof(ChaseBehavior))
            return true;

        return false;
    }
}