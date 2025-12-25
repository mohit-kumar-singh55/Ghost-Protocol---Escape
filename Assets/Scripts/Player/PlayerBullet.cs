using UnityEngine;

public class PlayerBullet : Bullet
{
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        // TODO: add effects, damage, etc.
    }
}