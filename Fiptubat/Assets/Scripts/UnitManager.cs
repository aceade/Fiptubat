using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 
/// </summary>
public class UnitManager : MonoBehaviour {

	public List<BaseUnit> units;
	private List<BaseUnit> activeUnits;

	private int currentUnit = 0;
	private BaseUnit selectedUnit;

	public bool isPlayer = false;

	public string factionName;

	private GameStateManager gameStateManager;

	// Use this for initialization
	void Start () {
		if (String.IsNullOrWhiteSpace(factionName)) {
			factionName = transform.name;
		}
		units.ForEach(unit => unit.SetUnitManager(this));
		selectedUnit = units[currentUnit];
	}

    public void EndTurn() {
		int stillMovingUnits = units.FindAll(unit => unit.IsStillMoving()).Count;
		Debug.LogFormat("{0} still has {1} moving units", factionName, stillMovingUnits);
		if (stillMovingUnits == 0) {
			units.ForEach(unit => unit.StandDown());
        	gameStateManager.EndTurn(this);
		}
		
    }

    public void UnitDied(BaseUnit unit) {
		units.Remove(unit);
		activeUnits.Remove(unit);
		if (units.Count == 0) {
			gameStateManager.FactionDefeated(this);
		}
	}

	public void DisableAllUnits() {
		units.ForEach(unit => unit.DeselectUnit());
	}

	public void AllUnitsExtracted() {
		gameStateManager.FactionEscaped(this);
	}

	public void StartTurn() {
		activeUnits = units;
		activeUnits.ForEach(unit => unit.StartTurn());
		selectedUnit.SelectUnit();
	}

	public void CycleUnit() {
		selectedUnit.DeselectUnit();
		currentUnit ++;
		if (currentUnit >= activeUnits.Count) {
			currentUnit = 0;
		}
		
		selectedUnit = activeUnits[currentUnit];
		selectedUnit.SelectUnit();
	}

	public void SelectUnit(int unitIndex) {
		if (unitIndex < 0 || unitIndex >= activeUnits.Count) {
			unitIndex = 0;
		}
		selectedUnit.DeselectUnit();
		currentUnit = unitIndex;
		selectedUnit = activeUnits[currentUnit];
		selectedUnit.SelectUnit();
	}

	public void UnitFinished(BaseUnit unit) {
		activeUnits.Remove(unit);
		Debug.LogFormat("{0} active units left for {1}", activeUnits.Count, this);
		if(activeUnits.Count == 0) {
			gameStateManager.EndTurn(this);
		} else {
			if (!isPlayer) {
				// use a round-robin approach
				CycleUnit();
			}
		}
	}

	public void SetGameManager(GameStateManager manager) {
		this.gameStateManager = manager;
	}

	public int GetRemainingUnitCount() {
		return units.Count;
	}
	
}
