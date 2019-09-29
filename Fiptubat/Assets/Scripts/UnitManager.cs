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

	private GameStateManager gameStateManager;

	// Use this for initialization
	void Start () {
		units.ForEach(unit => unit.SetUnitManager(this));
		selectedUnit = units[currentUnit];
		//selectedUnit.SelectUnit();
	}

    public void EndTurn() {
        gameStateManager.EndTurn(this);
    }

    public void UnitDied(BaseUnit unit) {
		units.Remove(unit);
		activeUnits.Remove(unit);
		if (units.Count == 0) {
			gameStateManager.FactionDefeated(this);
		}
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

	public void UnitFinished(BaseUnit unit) {
		activeUnits.Remove(unit);
		if(activeUnits.Count == 0) {
			gameStateManager.EndTurn(this);
		}
	}

	public void SetGameManager(GameStateManager manager) {
		this.gameStateManager = manager;
	}
	
}
