using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour {
	
	private SoundManager soundManager;
	
	private bool isPaused = false;
	
	void Start() {
		soundManager = GetComponent<SoundManager>();
	}

	public void Pause() {
		if (!isPaused) {
			isPaused = true;
		}
	}

	public void Resume() {
		isPaused = false;
	}
	
	public void EndTurn(UnitManager manager) {
		if (isPaused) {
			return;
		}
	}

	public void FactionDefeated(UnitManager manager) {
		if (manager.isPlayer) {
			GameOver();
		} else {
			// assume one faction for now. Could just say "No enemies left"
			Victory();
		}
	}

	private void GameOver() {

	}

	private void Victory() {

	}
}
