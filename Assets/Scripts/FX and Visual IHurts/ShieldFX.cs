using System;
using Entities;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ShieldFX : Entity
{
    private ParticleSystem _ps;
    public bool Active { get; private set; }

    protected override void OnAwake()
    {
        gameObject.SetActive(false);
        _ps = GetComponent<ParticleSystem>();
    }
    public void Protect(Action OnDeathCallback = null)
    {
        Revive();
        Active = true;
        OnDeath += OnDeathCallback;
        gameObject.SetActive(true);
    }

    protected override void DeathTriggered()
    {
        Debug.Log("shield down");
        Active = false;
        OnDeath += ResetOnDeathAction;
        gameObject.SetActive(false);
    }
}