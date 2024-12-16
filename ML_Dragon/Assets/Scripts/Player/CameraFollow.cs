using System;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Vector3 offset;   
    [SerializeField] private float followSpeed = 5f;

    private Transform _playerTransform;  

    private void Start()
    {
        _playerTransform = GameManager.Instance.PlayerManager.transform;
    }

    private void Update()
    {
        if (_playerTransform != null)
        {
            Vector3 targetPosition = _playerTransform.position + offset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }
    }
}