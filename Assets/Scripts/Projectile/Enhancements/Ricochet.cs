using System.Collections;
using System.Collections.Generic;
using Projectiles.Enhancements;
using UnityEngine;

public class Ricochet : ProjectileEnhancement
{
    int _hits;
    int MaxHits => (int) Handler.RicochetEnhancement.AllModules[0].Value;
    private Collider _lastCollider;

    public override void OnObstacleCollision(ref RaycastHit hit)
    {
        if (_lastCollider == hit.collider) return;

        _lastCollider = hit.collider;
        AttachedProjectile.Reflect(hit.normal, hit.point);
        if (_hits >= MaxHits) AttachedProjectile.Disable(true);
        _hits++;
    }

    public override void OnShoot()
    {
        _lastCollider = null;
        _hits = 0;
    }
}