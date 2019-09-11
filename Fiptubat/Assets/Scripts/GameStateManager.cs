using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour {
	
	private SoundManager soundManager;
	
	private bool isPaused = false;

	public List<UnitManager> factions;
	
	void Start() {
		soundManager = GetComponent<SoundManager>();
		factions[0].StartTurn();
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
        throw new System.NotImplementedException("Defeat has not been considered yet!");
	}

	private void Victory() {
        throw new System.NotImplementedException("We won! Now what?");
	}
}
