using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBase : MonoBehaviour
{
    public int damage;

    public int baseCost = 5;

    public List<FireMode> fireModes;

    private int currentFireMode;

    private Transform muzzle;

    void Start()
    {
        muzzle = transform;
    }

    /// <summary>
    /// FIIIIIIRE!
    /// </summary>
    /// <return>
    ///     Return true if it hit the target.
    /// </return>
    public bool Fire() {
        return false;
    }

    public FireMode GetCurrentFireMode() {
        return fireModes[currentFireMode];
    }

    public void CycleFireMode() {
        currentFireMode++;
        if (currentFireMode >= fireModes.Count) {
            currentFireMode = 0;
        }
    }

    public void SelectFireMode (int fireMode) {
        if (fireMode >= fireModes.Count) {
            fireMode = 0;
        }
        currentFireMode = fireMode;
    }

    public int GetCurrentFireCost() {
        return Mathf.RoundToInt(GetCurrentFireMode().modifier * baseCost);
    }
}
