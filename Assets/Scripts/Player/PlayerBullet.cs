using UnityEngine;

public class PlayerBullet : Bullet
{
    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        // TODO: add effects, damage, etc.
        base.OnCollisionEnter2D(collision);
    }
}