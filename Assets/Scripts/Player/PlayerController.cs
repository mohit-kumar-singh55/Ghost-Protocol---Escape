using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D.Animation;

enum Rotation { Right = 0, Left = 180 }

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(SpriteSkin))]
public class PlayerController : MonoBehaviour
{
    #region Serialized Fields
    [Header("Move Settings")]
    [SerializeField] private float _speed = 10f;
    [SerializeField] private float _dashForce = 10f;
    [Tooltip("Time to wait before next dash")]
    [SerializeField] private float _dashCooldownTime = 3f;

    [Space(10)]
    [Header("Jump Settings")]
    [SerializeField] private float _jumpForce = 8f;     // normal jump force
    [SerializeField] private float _wallJumpForce = 12f;     // wall jump force
    [SerializeField] private float _gravityScaleWhenFalling = 4f;   // increase gravity when in air
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask _wallLayer;

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
    private Transform _spine;   // sprites with shooting animation are rigged, so we can rotate the spine bone
    private float _moveX;
    private float _dashTimer = 0;
    private bool _isShooting = false;
    private bool _isJumping = false;
    private bool _isGrounded = true;
    private bool _isWall = false;
    private Vector3 _wallTouchPoint;
    private float _originalGravityScale;

    // animator params
    private readonly int Speed = Animator.StringToHash("Speed");
    private readonly int IsShooting = Animator.StringToHash("IsShooting");
    #endregion

    #region Events
    public static event Action<bool> OnPlayerShoot = delegate { };
    #endregion

    #region Unity Methods
    void Awake()
    {
        // get references
        _rb = GetComponent<Rigidbody2D>();
        _playerAnimator = GetComponent<Animator>();
        _spriteSkin = GetComponent<SpriteSkin>();

        // get spine
        _spine = _spriteSkin.rootBone.GetChild(0);

        // original gravity scale
        _originalGravityScale = _rb.gravityScale;
    }

    void Start()
    {
        _crosshair = CrosshairController.Instance;

        if (!_crosshair || !_gun)
        {
            Debug.LogError("Crosshair not found or Gun is not assigned!");
            enabled = false;
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
            HandleHorizontalRotationByCrosshair();
            HandleUpperBodyRotationByCrosshair();
        }
    }

    void FixedUpdate()
    {
        // move player
        HandleMove();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // ground check
        if (IsInLayerMask(collision.gameObject.layer, _groundLayer))
        {
            _isGrounded = true;
            _isJumping = false;
            _rb.gravityScale = _originalGravityScale;       // reset gravity scale
        }

        // wall check
        if (IsInLayerMask(collision.gameObject.layer, _wallLayer))
        {
            _isWall = true;
            _wallTouchPoint = collision.ClosestPoint(transform.position);
        }
    }
    #endregion

    #region Input System Callbacks
    private void OnMove(InputValue val)
    {
        _moveX = val.Get<Vector2>().x;

        // flip sprite, only if the player is moving and not shooting
        if (_moveX == 0 && !_isShooting) return;
        FlipPlayerHorizontally(_moveX < 0);
    }

    private void OnShoot(InputValue val)
    {
        ToogleShoot(val.isPressed);

        // reset player rotation in the direction of movement when stop shooting
        if (!val.isPressed) FlipPlayerHorizontally(_moveX < 0);
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

    private void OnJump(InputValue val)
    {
        // ** wall jump **
        if (_isJumping && _isWall && !_isGrounded)
        {
            // check for side wall jump
            Vector3 dirToJump = (transform.position - _wallTouchPoint).normalized + Vector3.up;
            dirToJump.z = 0;
            dirToJump.Normalize();

            // Debug.DrawRay(transform.position, dirToJump, Color.red, 0f, true);

            _rb.AddForce(dirToJump * _wallJumpForce, ForceMode2D.Impulse);

            _isWall = false;
            return;
        }

        // will do normal jump only if grounded
        if (!_isGrounded) return;

        // ** normal jump **
        _isWall = false;
        _isJumping = true;
        _isGrounded = false;
        _rb.gravityScale = _gravityScaleWhenFalling;        // increase gravity when in air
        _rb.AddForceY(_jumpForce, ForceMode2D.Impulse);
    }
    #endregion

    #region Private Methods
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

    private void FlipPlayerHorizontally(bool flipLeft)
    {
        // flip player
        _playerCurrentRotation = flipLeft ? Rotation.Left : Rotation.Right;
        transform.rotation = Quaternion.Euler(0, (float)_playerCurrentRotation, 0);
    }

    private void HandleHorizontalRotationByCrosshair()
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
        FlipPlayerHorizontally(_playerCurrentRotation == Rotation.Right);
    }

    private void HandleUpperBodyRotationByCrosshair()
    {
        // get direction from spine to crosshair
        Vector3 dir = (_crosshair.transform.position - _spine.position).normalized;

        // get the angle between right vector and direction to crosshair
        float zAngle = Vector3.SignedAngle(Vector3.right, dir, Vector3.forward) + 90;
        zAngle = _playerCurrentRotation == Rotation.Right ? zAngle : -zAngle;

        // apply rotation to spine
        _spine.rotation = Quaternion.Euler(_spine.rotation.eulerAngles.x, _spine.rotation.eulerAngles.y, zAngle);
    }

    // check if a layer is in the layermask
    private bool IsInLayerMask(int layer, LayerMask layerMask) => ((1 << layer) & layerMask.value) != 0;
    #endregion
}