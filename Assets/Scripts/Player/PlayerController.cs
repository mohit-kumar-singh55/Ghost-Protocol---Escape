using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D.Animation;

enum Rotation { Right = 0, Left = 180 }

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(SpriteSkin))]
public class PlayerController : MonoBehaviour
{
    #region Serialized Fields
    [Header("移動設定")]
    [SerializeField] private float _speed = 10f;
    [SerializeField] private float _dashForce = 10f;
    [Tooltip("次のダッシュまでの待機時間")]
    [SerializeField] private float _dashCooldownTime = 3f;

    [Space(10)]
    [Header("ジャンプ設定")]
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
    private Transform _spine;   // 射撃アニメーション付きスプライトはリグ済みのため、スパインボーンを回転させられる
    private float _moveX;
    private float _dashTimer = 0;
    private bool _isShooting = false;
    private bool _isJumping = false;
    private bool _isGrounded = true;
    private bool _isWall = false;
    private Vector3 _wallTouchPoint;
    private float _originalGravityScale;
    private UIManager _uiManager;

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

        // スパインを取得
        _spine = _spriteSkin.rootBone.GetChild(0);

        // 元の重力を保存
        _originalGravityScale = _rb.gravityScale;
    }

    void Start()
    {
        _crosshair = CrosshairController.Instance;
        _uiManager = UIManager.Instance;

        if (!_crosshair || !_gun || !_uiManager)
        {
            Debug.LogError("Crosshair not found or Gun or UIManager is not assigned!");
            enabled = false;
            return;
        }
    }

    void Update()
    {
        // ダッシュタイマー更新
        if (_dashTimer > 0)
        {
            _dashTimer -= Time.deltaTime;
            // タイマーが0になったらUIアニメーションを再生
            if (_dashTimer <= 0) _uiManager.PlayDashKeyAnimation(true);
        }

        // アニメーター更新
        _playerAnimator.SetFloat(Speed, Mathf.Abs(_rb.linearVelocityX) / _speed);

        // 射撃中はクロスヘアに基づいてプレイヤーの回転を更新する
        if (_isShooting)
        {
            HandleHorizontalRotationByCrosshair();
            HandleUpperBodyRotationByCrosshair();
        }
    }

    void FixedUpdate()
    {
        // プレイヤーの移動
        HandleMove();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // ground チェック
        if (IsInLayerMask(collision.gameObject.layer, _groundLayer))
        {
            _isGrounded = true;
            _isJumping = false;
            _rb.gravityScale = _originalGravityScale;       // 重力を元に戻す
        }

        // wall チェック
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

        // 移動中かつ射撃していない場合のみスプライトを反転する
        if (_moveX == 0 && !_isShooting) return;
        FlipPlayerHorizontally(_moveX < 0);
    }

    private void OnShoot(InputValue val)
    {
        ToogleShoot(val.isPressed);

        // 射撃をやめたら、移動方向に合わせてプレイヤーの回転をリセットする
        if (!val.isPressed) FlipPlayerHorizontally(_moveX < 0);
    }

    // 短い間の急ダッシュ
    private void OnSprint(InputValue val)
    {
        if (_dashTimer > 0) return;

        // クールダウンタイマーを開始する
        _dashTimer = _dashCooldownTime;

        // 射撃を停止
        ToogleShoot(false);

        // ダッシュUIアニメションを停止
        _uiManager.PlayDashKeyAnimation(false);

        // ダッシュ用の瞬間的な力を加える
        _rb.AddForceX(_moveX * _dashForce, ForceMode2D.Impulse);
    }

    private void OnJump(InputValue val)
    {
        // ** 壁ジャンプ **
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

        // 接地している場合のみ通常ジャンプを行う
        if (!_isGrounded) return;

        // ** 普通ジャンプ **
        _isWall = false;
        _isJumping = true;
        _isGrounded = false;
        _rb.gravityScale = _gravityScaleWhenFalling;        // 空中では重力を強める
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
        // 射撃
        _isShooting = isShooting;
        _playerAnimator.SetBool(IsShooting, _isShooting);
        _gun.SetActive(_isShooting);

        // 射撃イベントを呼び出す
        OnPlayerShoot?.Invoke(_isShooting);
    }

    private void FlipPlayerHorizontally(bool flipLeft)
    {
        // プレイヤーを反転する
        _playerCurrentRotation = flipLeft ? Rotation.Left : Rotation.Right;
        transform.rotation = Quaternion.Euler(0, (float)_playerCurrentRotation, 0);
    }

    private void HandleHorizontalRotationByCrosshair()
    {
        // 方向を取得する
        Vector3 playerRight = transform.right.normalized;
        Vector3 crosshairDir = (_crosshair.transform.position - transform.position).normalized;
        playerRight = new(playerRight.x, 0, 0);
        crosshairDir = new(crosshairDir.x, 0, 0);

        // クロスヘアがプレイヤーの背後にあるかをチェックする
        bool isCrosshairBehind = Vector3.Dot(crosshairDir, playerRight) < 0;
        if (!isCrosshairBehind) return;

        // 回転を変える
        FlipPlayerHorizontally(_playerCurrentRotation == Rotation.Right);
    }

    private void HandleUpperBodyRotationByCrosshair()
    {
        // スパインからクロスヘアへの方向を取得する
        Vector3 dir = (_crosshair.transform.position - _spine.position).normalized;

        // 右ベクトルとクロスヘア方向との角度を取得する
        float zAngle = Vector3.SignedAngle(Vector3.right, dir, Vector3.forward) + 90;
        zAngle = _playerCurrentRotation == Rotation.Right ? zAngle : -zAngle;

        // スパインに回転を適用する
        _spine.rotation = Quaternion.Euler(_spine.rotation.eulerAngles.x, _spine.rotation.eulerAngles.y, zAngle);
    }

    // レイヤーが LayerMask に含まれているかをチェックする
    private bool IsInLayerMask(int layer, LayerMask layerMask) => ((1 << layer) & layerMask.value) != 0;
    #endregion
}