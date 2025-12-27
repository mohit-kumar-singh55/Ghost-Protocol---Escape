using System;
using UnityEngine;

public class HelicopterHealth : Health
{
    [SerializeField] private GameObject _explosionEffectPrefab;

    public event Action OnHelicopterDestroyed = delegate { };

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        if (_currentHealth == 0)
        {
            // エベント
            OnHelicopterDestroyed?.Invoke();

            // マズルフラッシュ
            CameraController.Instance.ShakeCamera(0.3f);

            // 爆発エフェクト
            if (_explosionEffectPrefab)
            {
                Vector3 explosionPos = transform.position;
                explosionPos.z = -2;
                Instantiate(_explosionEffectPrefab, explosionPos, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }

    // ** 弾によるダメージではなく、プレイヤーへの自爆突撃によるダメージ **
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Health health))
        {
            // 最大ダメージを与えると受ける
            health.TakeDamage(999);
            TakeDamage(999);
        }
    }
}