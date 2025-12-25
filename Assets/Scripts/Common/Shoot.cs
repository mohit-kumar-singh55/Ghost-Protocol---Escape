using UnityEngine;

public abstract class Shoot : MonoBehaviour
{
    #region Serialized Fields
    [Header("Settings")]
    [SerializeField] protected float _shootSpeed = 10f;
    [SerializeField] protected float _fireRate = 0.5f;

    [Header("References")]
    [SerializeField] protected Transform _firePoint;
    [SerializeField] protected GameObject _bulletPrefab;
    #endregion

    #region Properties
    protected float _fireCooldown = 0f;
    #endregion

    protected virtual void Update()
    {
        if (_fireCooldown > 0) _fireCooldown -= Time.deltaTime;
    }

    protected void ShootBullet(Vector3 dir)
    {
        // cooldown
        if (_fireCooldown > 0) return;
        _fireCooldown = _fireRate;

        // TODO: change to object pool
        // spawn bullet
        GameObject bullet = Instantiate(_bulletPrefab, _firePoint.position, _firePoint.rotation);

        // Debug.Break();

        // no rigidbody on bullet
        if (!bullet.TryGetComponent(out Rigidbody2D rb))
        {
            Destroy(bullet);
            Debug.LogError("Bullet has no rigidbody");
            return;
        }

        // shoot in the given direction
        dir.z = 0;
        rb.AddForce(dir * _shootSpeed, ForceMode2D.Impulse);
    }
}