using UnityEngine;

public abstract class BaseEnemy : MonoBehaviour
{
    [SerializeField] protected EnemyBehaviorHandler _behaviorHandler;
    [SerializeField] protected Transform _enemyBody;
    [SerializeField] protected Animator _animator;
    
    public EnemyBehaviorHandler EnemyBehaviorHandler => _behaviorHandler;
    public Vector3 EnemyPosition => _enemyBody.position;
    public Transform EnemyTransform => _enemyBody;
    
    protected virtual void Start()
    {
        _behaviorHandler.InitData(this,_animator);
    }
}