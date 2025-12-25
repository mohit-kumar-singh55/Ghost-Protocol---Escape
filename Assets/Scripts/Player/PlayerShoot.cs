using UnityEngine;

public class PlayerShoot : Shoot
{
    #region Properties
    private bool _isShooting = false;
    private CrosshairController crosshair;
    #endregion

    void OnEnable()
    {
        PlayerController.OnPlayerShoot += ShouldStartShooting;
    }

    void OnDisable()
    {
        PlayerController.OnPlayerShoot -= ShouldStartShooting;
    }

    void Start()
    {
        crosshair = CrosshairController.Instance;
    }

    protected override void Update()
    {
        base.Update();

        if (_isShooting && _fireCooldown <= 0)
        {
            // get shoot direction
            Vector3 _shootDir = (crosshair.transform.position - transform.position).normalized;
            ShootBullet(_shootDir);     // shoot
        }
    }

    private void ShouldStartShooting(bool isShooting)
    {
        _isShooting = isShooting;
    }
}