using UnityEngine;

public class GameManager : MonoBehaviour
{
   public static GameManager Instance;

   public PlayerManager PlayerManager => _playerManager;
   
   [SerializeField] private PlayerManager _playerManager;

   private void Awake()
   {
      if (Instance != null && Instance != this)
      {
         Destroy(gameObject);
         return;
      }
      
      Instance = this;
   }
}
