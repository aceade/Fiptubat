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

	public Image gameOverScreen;

	public Text gameStatusText;

	public Text turnCounter;

	void Start() {
		gameOverScreen.gameObject.SetActive(false);
	}

	public void Pause() {
		gameStateManager.Pause();
	}

	public void Resume() {
		gameStateManager.Resume();
	}

	public void EndTurn() {
		unitManager.EndTurn();
	}

	public void CycleUnit() {
		unitManager.CycleUnit();
	}

	/// <summary>
	/// Show the user the distance to their possible target.
	/// </summary>
	public void ShowDistanceCost(float distance, int cost, int remainingPoints) {
		int roundedDistance = (Mathf.RoundToInt(distance * 10)/ 10);
		playerDistanceText.text = string.Format("Cost: {0}/{1}\nDistance: {2}", cost, remainingPoints, roundedDistance);
	}

	public void ClearDistanceText() {
		playerDistanceText.text = "";
	}

	public void AnnounceTurn(string playerName) {
		turnEndedText.text = playerName + " Turn";
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
			// TODO: show different background image
			gameStatusText.text = "VICTORY!";
		} else {
			gameStatusText.text = "GAME\nOVER!";
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
}
