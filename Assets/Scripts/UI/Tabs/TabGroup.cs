using System.Collections;
using System.Collections.Generic;
using UI;
using UI.TabEvents;
using UnityEngine;

public class TabGroup : MonoBehaviour
{
    private Tab[] _tabs;
    public Tab[] Tabs => _tabs;
    [SerializeField] private TabOnAnyOpened OnAnyOpened = default;
    private void Awake()
    {
        GetChildTabs();
    }

    public void GetChildTabs()
    {
        _tabs = GetComponentsInChildren<Tab>();
        if (Tabs != null)
        {
            for (int i = 0; i < _tabs.Length; i++)
                _tabs[i].ContainedTabGroup = this;
            SetDefaults();
        }
    }

    private void SetDefaults()
    {
        for (int i = 0; i < _tabs.Length; i++)
            if (i != 0) _tabs[i].ForceSilentClose();
            else _tabs[i].ForceSilentOpen();
    }

    public void RequestOpen(Tab t)
    {
        for (int i = 0; i < _tabs.Length; i++)
            if (_tabs[i] != t) _tabs[i].Close();
            else _tabs[i].Open();
        OnAnyOpened?.Invoke();
    }

    public void RequestOpen(int index)
    {
        if (index < 0 || index > _tabs.Length) return;
        for (int i = 0; i < _tabs.Length; i++)
            if (i != index) _tabs[i].Close();
            else _tabs[i].Open();
        OnAnyOpened?.Invoke();
    }
}