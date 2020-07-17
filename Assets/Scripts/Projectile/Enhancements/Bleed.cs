using Projectiles.Enhancements;
using Entities;

public class Bleed : ProjectileEnhancement
{
    public override void OnHittableCollision(ref IHittable i)
    {
        //Debug.Log(e.name + " is bleeding!");
    }
}
