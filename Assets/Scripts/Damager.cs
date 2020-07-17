using Entities;
using UnityEngine;

public abstract class Damager : MonoBehaviour, IHurt
{
    [SerializeField] protected LayerMask _hurtableMask = default;
    public abstract float Damage { get; }
    public float DamageModifier { get; set; } = 1;
    public float FinalDamage => Damage * DamageModifier;

    protected void DealDamage(IHittable h)
    {
        h.Hit(FinalDamage);
        OnDealDamage(h);
    }
    protected void DealDamage(RaycastHit ray, IHittable h, Projectiles.Projectile thisProjectile)
    {
        h.Hit(ray, thisProjectile, FinalDamage);
        OnDealDamage(h);
    }

    protected virtual void OnDealDamage(IHittable hitEntity)
    {

    }
}