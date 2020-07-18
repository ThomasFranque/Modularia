using System;
using Entities;
using UnityEngine;

public class HealFX : MonoBehaviour
{
    public const float BASE_RADIUS = 3f;
    private const float BASE_HEAL = 10.0f;
    //
    [SerializeField] private LayerMask _healAbles = default;
    [SerializeField] private float _hps = BASE_HEAL;
    [SerializeField, Range(0f, 2f)] private float _healSplitSecs = 1.5f;
    private float _hpsCountdown;
    private bool CanHeal => _hpsCountdown <= 0;
    public float HealAmount => _healSplitSecs * _hps;

    private Entity _caster;

    private bool _emitting;
    public bool Healing => _emitting;
    //

    private ParticleSystem _particleSystem;

    private void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        _hpsCountdown -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (!_emitting || !CanHeal) return;
        Collider[] within =
            Physics.OverlapSphere(transform.position, BASE_RADIUS, _healAbles);
        foreach (Collider c in within)
        {
            Entity e;
            if (c.TryGetComponent<Entity>(out e))
                Heal(e);
        }
        _particleSystem.Play();
    }

    private void Heal(Entity e)
    {
        bool overHeal = e.Heal(HealAmount);

        if (e == _caster && overHeal)
            OnCasterOverHeal?.Invoke();
        OnHeal();
    }

    private void OnHeal()
    {
        _hpsCountdown = _healSplitSecs;
    }

    public void StartHeal(Entity caster, Action overhealCallback = default)
    {
        _hpsCountdown = _healSplitSecs;
        gameObject.SetActive(true);
        _emitting = true;
        _caster = caster;
        OnCasterOverHeal = overhealCallback;
        _particleSystem.Play();
    }
    public void EndHeal()
    {
        _emitting = false;
        _particleSystem.Stop();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, BASE_RADIUS);
    }

    private Action OnCasterOverHeal;
}