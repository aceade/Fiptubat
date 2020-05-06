using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverBotAnimator : UnitAnimator
{
    private ParticleSystem exhaust;

    private List<AudioSource> propellors;

    public AudioClip idlePropNoise, activePropNoise, dyingPropNoise;
    
    protected override void Start() {
        base.Start();
        exhaust = GetComponentInChildren<ParticleSystem>();
        propellors = new List<AudioSource>(GetComponentsInChildren<AudioSource>());
        propellors.RemoveAll(obj => obj.GetComponent<TurretWeapon>() != null || obj.GetComponent<UnitVoiceSystem>() != null);
        SetPropellorNoise(idlePropNoise);
    }

    private void SetPropellorNoise(AudioClip noise) {
        propellors.ForEach(x => x.clip = noise);
        propellors.ForEach(x => x.Play());
    }

    public override void StartMoving() {
        base.StartMoving();
        SetPropellorNoise(activePropNoise);
    }

    public override void StopMoving() {
        base.StopMoving();
        SetPropellorNoise(idlePropNoise);
    }

    public override void Die() {
        base.Die();
        SetPropellorNoise(dyingPropNoise);
        exhaust.Stop();
    }
}
