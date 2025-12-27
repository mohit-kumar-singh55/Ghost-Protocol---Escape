using UnityEngine;

public abstract class Bullet : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField] private float _lifetime = 3f;
    [SerializeField] private GameObject _explosionEffectPrefab;
    #endregion

    protected virtual void Start()
    {
        // もし、何かに当たらなかったら、一定時間後に消える
        Destroy(gameObject, _lifetime);
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Health health))
        {
            // ダメージを与える
            health.TakeDamage();
        }

        // 衝突エフェクト
        AfterCollisionEffects();

        Destroy(gameObject);
    }

    private void AfterCollisionEffects()
    {
        // 爆発エフェクト
        if (_explosionEffectPrefab)
            Instantiate(_explosionEffectPrefab, transform.position, Quaternion.identity);

        // カメラシェイク
        CameraController.Instance.ShakeCamera(0.05f);
    }
}