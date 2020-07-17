using Projectiles.Enhancements;
using UnityEngine;

public class Explosive : ProjectileEnhancement
{
    [SerializeField] private GameObject _explosionPrefab = default;
    private ParticleSystem _explosion;

    private const float EXPLOSION_STRENGTH = 50;
    private const int BASE_EXPLOSION_RADIUS = 2;

    private float Chance => Handler.ExplosiveEnhancement.AllModules[0].Value;
    private float Radius => Handler.ExplosiveEnhancement.AllModules[1].Value;

    private void Awake()
    {
        _explosion = Instantiate(_explosionPrefab).GetComponent<ParticleSystem>();
        _explosion.gameObject.hideFlags = HideFlags.HideInHierarchy;
    }

    public override void OnAnyCollision(ref RaycastHit hit)
    {
        bool triggered = Random.Range(0f, 1f) < Chance;

        if (triggered)
            TriggerExplosion(hit.point);
    }

    private void TriggerExplosion(Vector3 at)
    {
        _explosion.transform.localScale = Vector3.one * Radius;
        _explosion.transform.position = at;
        _explosion.Play();
        HitAllInRange(at);
    }

    private void HitAllInRange(Vector3 at)
    {
        Collider[] hits =
            Physics.OverlapSphere(at, Radius);

        for (int i = 0; i < hits.Length; i++)
        {
            Rigidbody rb;
            if (hits[i].TryGetComponent<Rigidbody>(out rb))
            {
                rb.AddExplosionForce(EXPLOSION_STRENGTH * (Handler.ExplosiveEnhancement.AllModules[1].Level / 2),
                    at, Radius * BASE_EXPLOSION_RADIUS, 20);
            }
        }
    }
}