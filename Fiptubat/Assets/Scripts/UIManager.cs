﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the UI. This is basically the player's controller.
/// </summary>
public class UIManager : MonoBehaviour {
	
	public UnitManager unitManager;

	public GameStateManager gameStateManager;	

	public Text playerDistanceText;

	public Text turnEndedText;

	public Image mainScreen;

	public Image gameOverScreen, pauseScreen;

	public Text gameStatusText;

	public Text turnCounter;

	private List<Button> mainScreenButtons = new List<Button>();

	public Sprite gameOverImage, victoryImage;

	void Start() {
		gameOverScreen.gameObject.SetActive(false);
		pauseScreen.gameObject.SetActive(false);
		mainScreen.GetComponentsInChildren<Button>(mainScreenButtons);
	}

	public void Pause() {
		pauseScreen.gameObject.SetActive(true);
		gameStateManager.Pause();
	}

	public void Resume() {
		gameStateManager.Resume();
		pauseScreen.gameObject.SetActive(false);
	}

	public void EndTurn() {
		unitManager.EndTurn();
	}

	public void CycleUnit() {
		unitManager.CycleUnit();
	}

	/// <summary>
	/// Show the user how much it would cost to move to a particular point
	/// </summary>
	/// <param name="cost">How much it would cost</param>
	/// <param name="remainingPoints">How many action points they have left</param>
	public void ShowMoveCost(int cost, int remainingPoints) {
		playerDistanceText.text = string.Format("Cost: {0}/{1}", cost, remainingPoints);
	}

	public void ClearDistanceText() {
		playerDistanceText.text = "";
	}

	public void AnnounceTurn(UnitManager faction) {
		turnEndedText.text = faction.factionName + " Turn";
		if (!faction.canAttack) {
			turnEndedText.text += " (CANNOT ATTACK!)";
		}
		Invoke("ClearAnnounceText", 2f);
	}

	public void AnnounceSectorClear() {
		turnEndedText.text = "Sector clear, proceed to extraction point";
		Invoke("ClearAnnounceText", 3f);
	}

	public void ShowTurnsRemaining(int turnsRemaining, bool isCritical) {
		turnCounter.text = string.Format("{0}", turnsRemaining);
		if (isCritical) {
			turnCounter.color = Color.blue;
		}
	}

	private void ClearAnnounceText() {
		turnEndedText.text = "";
	}

	public void SelectUnit(int unitIndex) {
		unitManager.SelectUnit(unitIndex);
	}

	public void ShowGameOverScreen(bool victory) {

		if (victory) {
			gameStatusText.text = "VICTORY!";
			gameOverScreen.sprite = victoryImage;
		} else {
			gameStatusText.text = "GAME\nOVER!";
			gameOverScreen.sprite = gameOverImage;
		}

		mainScreen.gameObject.SetActive(false);
		gameOverScreen.gameObject.SetActive(true);
		
	}

	public void Restart() {
		gameStateManager.Restart();
	}

	public void Quit() {
		gameStateManager.Quit();
	}

	public bool isPaused() {
		return gameStateManager.IsCurrentlyPaused();
	}

	/// <summary>
	/// Enable or disable buttons.
	/// </summary>
	/// <param name="usingUi"></param>
	public void ToggleUiStatus(bool usingUi) {
		for (int i = 0; i < mainScreenButtons.Count; i++) {
			mainScreenButtons[i].interactable = usingUi;
		}
	}
}
