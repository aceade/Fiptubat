using UnityEngine;
using Unity;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class UnitVoiceSystem : MonoBehaviour {

    private AudioSource voice;

    public List<AudioClip> targetSpottedLines;

    public List<AudioClip> targetHeardLines;

    public List<AudioClip> targetDestroyedLines;

    public List<AudioClip> targetMissedLines;

    public List<AudioClip> outOfAmmoLines;

    public List<AudioClip> reloadingLines;

    public List<AudioClip> takingDamageLines;

    public List<AudioClip> deathLines;

    public List<AudioClip> nearlyHitLines;

    public List<AudioClip> movingLines;

    void Start() {
        voice = GetComponent<AudioSource>();
    }

    private AudioClip GetRandomClip(List<AudioClip> possibleClips) {
        int index = Random.Range(0, possibleClips.Count);
        return possibleClips[index];
    }

    private void playClip(AudioClip clip) {
        if (!voice.isPlaying) {
            voice.PlayOneShot(clip);
        }
    }

    public void Moving() {
        AudioClip voiceLine = GetRandomClip(movingLines);
        playClip(voiceLine);
    }

    public void TargetSpotted() {
        AudioClip voiceLine = GetRandomClip(targetSpottedLines);
        playClip(voiceLine);
    }

    public void TargetHeard() {
        AudioClip voiceLine = GetRandomClip(targetHeardLines);
        playClip(voiceLine);
    }

    public void TargetMissed() {
        AudioClip voiceLine = GetRandomClip(targetMissedLines);
        playClip(voiceLine);
    }

    public void TargetDestroyed() {
        AudioClip voiceLine = GetRandomClip(targetDestroyedLines);
        playClip(voiceLine);
    }
    
    public void OutOfAmmo() {
        AudioClip voiceLine = GetRandomClip(outOfAmmoLines);
        playClip(voiceLine);
    }

    public void Reloading() {
        AudioClip voiceLine = GetRandomClip(reloadingLines);
        playClip(voiceLine);
    }

    public void NearlyHit() {
        AudioClip voiceLine = GetRandomClip(nearlyHitLines);
        playClip(voiceLine);
    }

    public void Hit() {
        AudioClip voiceLine = GetRandomClip(takingDamageLines);
        playClip(voiceLine);
    }

    public void Die() {
        AudioClip voiceLine = GetRandomClip(deathLines);
        playClip(voiceLine);
    }

}