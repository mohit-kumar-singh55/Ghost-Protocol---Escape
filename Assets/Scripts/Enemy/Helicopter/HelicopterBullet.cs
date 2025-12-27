using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class HelicopterBullet : Bullet
{
    [SerializeField] private float _homingBulletSpeed = 20f;

    private Rigidbody2D _rb;
    private Transform _playerTransform;

    protected override void Start()
    {
        base.Start();
        _rb = GetComponent<Rigidbody2D>();
        _playerTransform = FindAnyObjectByType<PlayerController>().transform;

        if (!_playerTransform)
        {
            Debug.LogError("HelicopterBullet: PlayerTransform not found!");
            enabled = false;
            return;
        }
    }

    private void FixedUpdate()
    {
        HomingBullet();
    }

    private void HomingBullet()
    {
        // ある程度プレイヤーを追尾する（完全なホーミングではない）
        Vector3 dirToPlayer = (_playerTransform.position - transform.position).normalized;
        dirToPlayer.z = 0;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, dirToPlayer);
        _rb.AddForce(dirToPlayer * _homingBulletSpeed, ForceMode2D.Force);
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
    }
}