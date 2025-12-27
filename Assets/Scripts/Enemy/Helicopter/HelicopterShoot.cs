using System.Collections;
using UnityEngine;

public class HelicopterShoot : Shoot
{
    #region Serialized Fields
    [Space(10)]
    [Header("Helicopter Shoot Settings")]
    [SerializeField] private int _bulletsPerBurst = 5;
    [SerializeField] private float _burstInterval = 2f;
    #endregion

    #region Properties
    private Camera _mainCam;
    private Transform _playerTransform;
    private bool _isShooting = false;
    #endregion

    protected override void Start()
    {
        base.Start();

        _mainCam = Camera.main;
        _playerTransform = FindAnyObjectByType<PlayerController>().transform;

        if (!_mainCam || !_playerTransform)
        {
            Debug.LogError("HelicopterShoot: No main camera or player found in scene!");
            enabled = false;
            return;
        }
    }

    protected override void Update()
    {
        base.Update();
        CheckIfCanShoot();
    }

    private void CheckIfCanShoot()
    {
        Vector3 dirToPlayer = (_playerTransform.position - _firePoint.position).normalized;
        dirToPlayer.z = 0;
        // ヘリコプターがプレイヤーの上空にいるかをチェックする
        bool isHelicopterAbovePlayer = Vector3.Dot(dirToPlayer, Vector3.up) < 0;

        // ヘリコプターがプレイヤーの上空にいるときのみ射撃する
        if (!_isShooting && isHelicopterAbovePlayer)
        {
            _isShooting = true;
            StartCoroutine(BurstShooting());    // 射撃を開始
        }
        else if (_isShooting && !isHelicopterAbovePlayer)
        {
            _isShooting = false;
            StopAllCoroutines();    // 射撃を停止
        }
    }

    private IEnumerator BurstShooting()
    {
        while (true)
        {
            // バースト射撃
            for (int i = 0; i < _bulletsPerBurst; i++)
            {
                // 射撃方向を取得する
                Vector3 _shootDir = (_playerTransform.position - _firePoint.position).normalized;
                ShootBullet(_shootDir);     // 射撃

                yield return new WaitForSeconds(_fireRate);
            }

            yield return new WaitForSeconds(_burstInterval);
        }
    }
}