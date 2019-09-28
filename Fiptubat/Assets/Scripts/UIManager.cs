using System.Collections;
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
		playerDistanceText.text = string.Format("Cost: {0}/{1}\nDistance: {2}", cost, remainingPoints, distance);
	}

	public void ClearDistanceText() {
		playerDistanceText.text = "";
	}
}
