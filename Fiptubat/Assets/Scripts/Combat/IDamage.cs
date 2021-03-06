using UnityEngine;

public interface IDamage
{
    void Damage(DamageType damageType, int damageAmount, Vector3 attackDirection);

    Transform GetTransform();

    int GetRemainingHealth();

    /// <summary>
    /// Get the damage of their current weapon, if applicable. Used for target selection.
    /// <summary>
    int GetPotentialDamage();

    void HitNearby();
}