﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Add a time limit. 
/// Mainly used for testing, but could be converted to kick in after a maximum number of turns.
/// </summary>
public class TimeBoundVictory : MonoBehaviour
{

    private GameStateManager gameStateManager;

    public UnitManager playerManager;

    private float time;

    public int maxTimeSeconds = 30;

    private bool invoked = false;

    public bool playerVictory;

    void Start()
    {
        gameStateManager = GetComponent<GameStateManager>();
    }

    void Update()
    {
        time += Time.deltaTime;
        if (time > maxTimeSeconds && !invoked) {
            invoked = true;
            if (playerVictory) {
                gameStateManager.FactionEscaped(playerManager);
            } else {
                gameStateManager.FactionDefeated(playerManager);
            }
        }
    }
}
