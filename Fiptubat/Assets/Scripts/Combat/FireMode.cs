using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A specific firing mode, e.g. Snap Shot, Aimed Shot, Short Burst, etc.
/// <summary>
[CreateAssetMenu(fileName = "FireMode", menuName = "Aceade/Combat/FireMode", order = 1)]
public class FireMode : ScriptableObject
{
    public string description;

    [Tooltip("How much the shot will deviate from right-on-target")]
    public float deviation = 0.01f;

    [Tooltip("How much does this affect the shooting cost")]
    public float costModifier = 1f;
}
