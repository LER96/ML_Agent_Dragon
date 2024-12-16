using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }

    public EventManagerEvents events = new EventManagerEvents();
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}

public class EventManagerEvents{
    public UnityEvent<Vector3> onEnemyAttack = new UnityEvent<Vector3>();
    public UnityEvent onRoundEnd = new UnityEvent();
}
