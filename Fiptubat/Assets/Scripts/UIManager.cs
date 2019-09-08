using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the UI. This is basically the player's controller.
/// </summary>
public class UIManager : MonoBehaviour {
	
	public UnitManager unitManager;

	public GameStateManager gameStateManager;	

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
}
