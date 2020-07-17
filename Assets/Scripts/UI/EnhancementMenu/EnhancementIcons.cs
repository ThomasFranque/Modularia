using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enhancement/Icons")]
public class EnhancementIcons : ScriptableObject
{
    [SerializeField] private Sprite _generic = default;
    [SerializeField] private Sprite _bleed = default;
    [SerializeField] private Sprite _multishot = default;
    [SerializeField] private Sprite _explosive = default;
    [SerializeField] private Sprite _ricochet = default;
    [SerializeField] private Sprite _piercing = default;

    public Sprite Generic { get => _generic; set => _generic = value; }
    public Sprite Bleed { get => _bleed; set => _bleed = value; }
    public Sprite Multishot { get => _multishot; set => _multishot = value; }
    public Sprite Explosive { get => _explosive; set => _explosive = value; }
    public Sprite Ricochet { get => _ricochet; set => _ricochet = value; }
    public Sprite Piercing { get => _piercing; set => _piercing = value; }
}