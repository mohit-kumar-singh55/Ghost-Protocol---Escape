using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D.Animation;

enum Rotation { Right = 0, Left = 180 }

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(SpriteSkin))]
public class PlayerController : MonoBehaviour
{
    #region Serialized Fields
    [Header("Settings")]
    [SerializeField] private float _speed = 10f;
    [SerializeField] private float _dashForce = 10f;
    [Tooltip("Time to wait before next dash")]
    [SerializeField] private float _dashCooldownTime = 3f;

    [Space(10)]
    [Header("References")]
    [SerializeField] private GameObject _gun;
    #endregion

    #region Properties
    private Rigidbody2D _rb;
    private Animator _playerAnimator;
    private CrosshairController _crosshair;
    private SpriteSkin _spriteSkin;
    private Rotation _playerCurrentRotation = Rotation.Right;
    private Transform _spine;
    private Quaternion _originalSpineRotation;
    private float _moveX;
    private float _dashTimer = 0;
    private bool _isShooting;

    // animator params
    private readonly int Speed = Animator.StringToHash("Speed");
    private readonly int IsShooting = Animator.StringToHash("IsShooting");
    #endregion

    #region Events
    public static event Action<bool> OnPlayerShoot = delegate { };
    #endregion

    void Awake()
    {
        // get references
        _rb = GetComponent<Rigidbody2D>();
        _playerAnimator = GetComponent<Animator>();
        _spriteSkin = GetComponent<SpriteSkin>();

        // get spine
        _spine = _spriteSkin.rootBone.GetChild(0);
        _originalSpineRotation = _spine.rotation;
    }

    void Start()
    {
        _crosshair = CrosshairController.Instance;

        if (!_crosshair || !_gun)
        {
            Debug.LogError("Crosshair not found or Gun is not assigned!");
            return;
        }
    }

    void Update()
    {
        // update dash timer
        if (_dashTimer > 0) _dashTimer -= Time.deltaTime;

        // update animator
        _playerAnimator.SetFloat(Speed, Mathf.Abs(_rb.linearVelocityX) / _speed);

        // update player rotation based on crosshair if player is shooting
        if (_isShooting)
        {
            HandleVerticalRotationByCrosshair();
            HandleUpperBodyRotationByCrosshair();
        }
        // ? if needed,reset spine rotation if not shooting
        // else ResetUpperBodyRotation();
    }

    void FixedUpdate()
    {
        // move player
        HandleMove();
    }

    #region Input System Callbacks
    private void OnMove(InputValue val)
    {
        _moveX = val.Get<Vector2>().x;

        // flip sprite, only if the player is moving and not shooting
        if (_moveX == 0 && !_isShooting) return;
        _playerCurrentRotation = _moveX < 0 ? Rotation.Left : Rotation.Right;
        transform.rotation = Quaternion.Euler(0, (float)_playerCurrentRotation, 0);
    }

    private void OnShoot(InputValue val)
    {
        ToogleShoot(val.isPressed);
    }

    // 短い間の急ダッシュ
    private void OnSprint(InputValue val)
    {
        if (_dashTimer > 0) return;

        // start cooldown timer
        _dashTimer = _dashCooldownTime;

        // stop shooting
        ToogleShoot(false);

        // sudden force to dash
        _rb.AddForceX(_moveX * _dashForce, ForceMode2D.Impulse);
    }
    #endregion

    private void HandleMove()
    {
        float targetVelocity = _moveX * _speed;
        float velocityChange = targetVelocity - _rb.linearVelocityX;
        _rb.AddForceX(velocityChange, ForceMode2D.Force);
    }

    private void ToogleShoot(bool isShooting)
    {
        // shoot
        _isShooting = isShooting;
        _playerAnimator.SetBool(IsShooting, _isShooting);
        _gun.SetActive(_isShooting);

        // invoke shoot events
        OnPlayerShoot?.Invoke(_isShooting);
    }

    private void HandleVerticalRotationByCrosshair()
    {
        // get directions
        Vector3 playerRight = transform.right.normalized;
        Vector3 crosshairDir = (_crosshair.transform.position - transform.position).normalized;
        playerRight = new(playerRight.x, 0, 0);
        crosshairDir = new(crosshairDir.x, 0, 0);

        // check if crosshair is behind player
        bool isCrosshairBehind = Vector3.Dot(crosshairDir, playerRight) < 0;
        if (!isCrosshairBehind) return;

        // change rotation
        _playerCurrentRotation = _playerCurrentRotation == Rotation.Right ? Rotation.Left : Rotation.Right;
        transform.rotation = Quaternion.Euler(0, (float)_playerCurrentRotation, 0);
    }

    private void HandleUpperBodyRotationByCrosshair()
    {
        // get spine transform
        Vector3 dir = (_crosshair.transform.position - _spine.position).normalized;

        // get the angle between right vector and direction to crosshair
        float zAngle = Vector3.SignedAngle(Vector3.right, dir, Vector3.forward) + 90;
        zAngle = _playerCurrentRotation == Rotation.Right ? zAngle : -zAngle;

        // apply rotation to spine
        _spine.rotation = Quaternion.Euler(_spine.rotation.eulerAngles.x, _spine.rotation.eulerAngles.y, zAngle);
    }

    // reset spine rotation to original
    private void ResetUpperBodyRotation() => _spine.rotation = _originalSpineRotation;
}