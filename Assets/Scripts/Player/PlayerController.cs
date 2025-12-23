using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    #region Serialized Fields
    [Header("Settings")]
    [SerializeField] private float _speed = 5f;

    [Space(10)]
    [Header("References")]
    [SerializeField] private GameObject _gun;
    #endregion

    #region Properties
    private Rigidbody2D _rb;
    private Animator _playerAnimator;
    private Animator _gunAnimator;
    private float _moveX;
    private bool _isShooting;

    // animator params
    private readonly int Speed = Animator.StringToHash("Speed");
    private readonly int IsShooting = Animator.StringToHash("IsShooting");
    #endregion

    void Awake()
    {
        // get references
        _rb = GetComponent<Rigidbody2D>();
        _playerAnimator = GetComponent<Animator>();
    }

    void Start()
    {
        _gunAnimator = _gun.TryGetComponent(out Animator anim) ? anim : null;
    }

    void Update()
    {
        _playerAnimator.SetFloat(Speed, Mathf.Abs(_rb.linearVelocityX) / _speed);
        // _playerAnimator.SetBool(IsShooting, false);
    }

    void FixedUpdate()
    {
        float targetVelocity = _moveX * _speed;
        float velocityChange = targetVelocity - _rb.linearVelocityX;
        // _rb.linearVelocityX = move;
        _rb.AddForceX(velocityChange, ForceMode2D.Force);
    }

    // Input System Callbacks
    private void OnMove(InputValue val)
    {
        _moveX = val.Get<Vector2>().x;

        // flip sprite, only if the player is moving
        if (_moveX == 0) return;
        transform.rotation = _moveX < 0 ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;
    }

    private void OnShoot(InputValue val)
    {
        _isShooting = val.isPressed;
        _playerAnimator.SetBool(IsShooting, _isShooting);
        _gun.SetActive(_isShooting);
    }
}