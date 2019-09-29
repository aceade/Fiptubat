using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour {
	
	private SoundManager soundManager;
	
	private bool isPaused = false;

	public List<UnitManager> factions;
	
	private int currentFactionIndex = 0;

	void Start() {
		soundManager = GetComponent<SoundManager>();
		factions.ForEach (faction => faction.SetGameManager(this));
		Invoke("StartTheFirstTurn", 0.1f);
	}

	/// <summary>
	/// Gives the level time to set up
	/// <summary>
	private void StartTheFirstTurn() {
		factions[0].StartTurn();
	}

	public void Pause() {
		if (!isPaused) {
			isPaused = true;
			Time.timeScale = 0f;
		}
	}

	public void Resume() {
		isPaused = false;
		Time.timeScale = 1f;
	}
	
	public void EndTurn(UnitManager manager) {
		if (isPaused) {
			return;
		}

		Debug.LogFormat("{0} has ended their turn", manager);
		currentFactionIndex ++;
		if (currentFactionIndex >= factions.Count) {
			currentFactionIndex = 0;
		}
		factions[currentFactionIndex].StartTurn();
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
