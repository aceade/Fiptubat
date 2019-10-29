using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour {
	
	private SoundManager soundManager;

	public UIManager uiManager;
	
	private bool isPaused = false;

	public List<UnitManager> factions;
	
	private int currentFactionIndex = 0;

	public int maxTurns = 10;

	private int turns = 0;
	public int criticalTurnCount = 8;

	void Start() {
		soundManager = GetComponent<SoundManager>();
		factions.ForEach (faction => faction.SetGameManager(this));
		Invoke("StartTheFirstTurn", 0.1f);
	}

	/// <summary>
	/// Gives the level time to set up
	/// <summary>
	private void StartTheFirstTurn() {
		uiManager.ShowTurnsRemaining(maxTurns, false);
		factions[0].StartTurn();
		soundManager.StartTheMusic();
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

	public void Quit() {
		SceneManager.LoadScene(1);
	}

	public void Restart() {
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
	
	public void EndTurn(UnitManager manager) {
		if (isPaused) {
			return;
		}
		Debug.LogFormat("{0} wants to end their turn. Current player is {1}", manager, factions[currentFactionIndex]);
		if (factions.IndexOf(manager) != currentFactionIndex) {
			Debug.LogFormat("{0}, wait your turn!", manager);
			return;
		}

		bool isGameOver = false;
		if (manager.isPlayer) {
			turns++;
			bool isCritical = turns >= criticalTurnCount;
			uiManager.ShowTurnsRemaining(maxTurns - turns, isCritical);
			if (turns > maxTurns) {
				isGameOver = true;

			}
		}

		if (isGameOver) {
			FactionDefeated(manager);
		} else {
			currentFactionIndex ++;
			if (currentFactionIndex >= factions.Count) {
				currentFactionIndex = 0;
			}
			uiManager.AnnounceTurn(factions[currentFactionIndex].factionName);
			factions[currentFactionIndex].StartTurn();
		}
	}

	public void FactionDefeated(UnitManager manager) {
		if (manager.isPlayer) {
			GameOver();
		} else {
			// assume one faction for now.
			factions.Remove(manager);
			uiManager.AnnounceSectorClear();
		}
	}

	public void FactionEscaped(UnitManager manager) {
		if (manager.isPlayer) {
			Victory();
		}
	}

	private void GameOver() {
		factions.ForEach(faction => faction.DisableAllUnits());
		uiManager.ShowGameOverScreen(false);
		soundManager.PlayDefeatMusic();
	}

	private void Victory() {
		factions.ForEach(faction => faction.DisableAllUnits());
		uiManager.ShowGameOverScreen(true);
		soundManager.PlayVictoryMusic();
	}
}
