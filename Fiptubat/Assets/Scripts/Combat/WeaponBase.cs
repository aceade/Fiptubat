using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBase : MonoBehaviour
{
    public int damage = 1;

    public int baseCost = 5;

    public float maxDistance = 30f;

    public List<FireMode> fireModes;

    public int magSize = 5;

    protected int currentAmmo;

    public int reloadCost = 5;

    private int currentFireMode;

    protected Transform muzzle;

    public float fireRate = 0.5f;

    protected WaitForSeconds fireCycle;

    protected bool canAttack = true;

    private AudioSource audioSource;

    public TracerPool tracerPool;

    protected int layerMask;

    protected void Start()
    {
        muzzle = transform;
        currentAmmo = magSize;
        fireCycle = new WaitForSeconds(fireRate);
        audioSource = GetComponent<AudioSource>();
        layerMask = 1 << 1; // ignore transparent effects
        layerMask = ~layerMask;
    }

    /// <summary>
    /// FIIIIIIRE!
    /// </summary>
    /// <return>
    ///     Return true if it hit the target.
    /// </return>
    public virtual bool Fire() {
        StartCoroutine(performAttackCycle());

        RaycastHit hit;
        Vector3 fireDir = CalculateFireDirection();
        ShowTracers(fireDir);
        if (Physics.Raycast(muzzle.position, fireDir, out hit, maxDistance, layerMask)) {
            
            var hitTransform = hit.transform;
            Debug.LogFormat("I hit {0}", hitTransform);
            var damageScript = hitTransform.root.GetComponent<IDamage>();
            if (damageScript == null) {
                return false;
            } else {
                Debug.LogFormat("I shot {0}", damageScript);
                damageScript.Damage(DamageType.REGULAR, damage);
                return true;
            }
        } 
        return false;
    }

    private IEnumerator performAttackCycle() {
        canAttack = false;
        currentAmmo--;
        yield return fireCycle;
        if (currentAmmo > 0) {
            canAttack = true;
        }
        
    }

    protected Vector3 CalculateFireDirection() {
        Vector3 firDir = muzzle.forward;
        float deviation = fireModes[currentFireMode].deviation;
        firDir.x += Random.Range(-deviation, deviation);
        firDir.y += Random.Range(-deviation, deviation);
        firDir.z += Random.Range(-deviation, deviation);
        return firDir;
    }

    public FireMode GetCurrentFireMode() {
        return fireModes[currentFireMode];
    }

    public void Reload() {
        currentAmmo = magSize;
        canAttack = true;
    }

    public int GetRemainingAmmo() {
        return currentAmmo;
    }

    public string GetAmmoCounter() {
        return string.Format("{0}/{1}", currentAmmo, magSize);
    }

    public bool CanAttack() {
        return canAttack;
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

    protected void PlayNoise() {
        if (!audioSource.isPlaying) {
            audioSource.Play();
        }
    }

    protected void ShowTracers(Vector3 direction) {
        TracerEffect tracer = tracerPool.GetTracer();
        tracer.Launch(muzzle.position + muzzle.forward, direction);
    }

    public int GetCurrentFireCost() {
        return Mathf.RoundToInt(GetCurrentFireMode().costModifier * baseCost);
    }
}
