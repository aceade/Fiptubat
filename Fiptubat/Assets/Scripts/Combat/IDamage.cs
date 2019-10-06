using UnityEngine;

public interface IDamage
{
    void Damage(DamageType damageType, int damageAmount);

    Transform GetTransform();
}