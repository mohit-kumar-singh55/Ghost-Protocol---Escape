using System;
using UnityEngine;

/// <summary>
/// ***** 神風ヘリコプター *****
/// move towards player to blast,
/// handle collision with player bullets,
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class HelicopterController : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField] private float _speed = 5f;
    #endregion

    #region Properties
    private Rigidbody2D _rb;
    private Transform _playerTransform;

    public Transform PlayerTransform { set => _playerTransform = value; }
    #endregion

    #region Events
    public event Action OnHelicopterDestroyed = delegate { };
    #endregion

    #region Unity Methods
    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        if (!_playerTransform)
        {
            Debug.LogError("HelicopterController: PlayerTransform not assigned!");
            enabled = false;
            return;
        }
    }

    void FixedUpdate()
    {
        if (_playerTransform) MoveTowardsPlayer();
    }
    #endregion

    private void MoveTowardsPlayer()
    {
        // move towards player
        Vector3 dir = (_playerTransform.position - transform.position).normalized;
        dir.z = 0;
        _rb.AddForce(dir * _speed, ForceMode2D.Force);
    }
}