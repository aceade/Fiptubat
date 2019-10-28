using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
	
	private AudioSource audioSource;

	public AudioClip defeatMusic;
	public AudioClip victoryMusic;

	public List<AudioClip> regularMusic;

	private bool isGameInProgress;

	private int currentMusicIndex;

	void Start () {
		audioSource = GetComponent<AudioSource>();
	}

	public void StartTheMusic() {
		isGameInProgress = true;
	}

	void Update() {
		if (isGameInProgress && !audioSource.isPlaying) {
			PlayRegularMusic();
		}
	}

	private void PlayRegularMusic() {
		AudioClip clip = regularMusic[currentMusicIndex];
		PlaySound(clip);
		currentMusicIndex++;
		if (currentMusicIndex >= regularMusic.Count) {
			// loop back to the start
			currentMusicIndex = 0;
		}
	}
	
	
	private void PlaySound(AudioClip audioClip) {
		audioSource.Stop();
		audioSource.PlayOneShot(audioClip);
	}

	public void PlayDefeatMusic() {
		isGameInProgress = false;
		PlaySound(defeatMusic);
	}

	public void PlayVictoryMusic() {
		isGameInProgress = false;
		PlaySound(victoryMusic);
	}

}
