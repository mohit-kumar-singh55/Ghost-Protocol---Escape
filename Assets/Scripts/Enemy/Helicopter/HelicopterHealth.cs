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
            // invoke events
            OnHelicopterDestroyed?.Invoke();

            // camera shake
            CameraController.Instance.ShakeCamera(0.3f);

            // explosion effect
            if (_explosionEffectPrefab)
            {
                Vector3 explosionPos = transform.position;
                explosionPos.z = -2;
                Instantiate(_explosionEffectPrefab, explosionPos, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }

    // ** damage due to suicide crash into the player not by bullet **
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