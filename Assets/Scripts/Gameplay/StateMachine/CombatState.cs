using System;
using System.Collections;
using System.Collections.Generic;
using Pixelplacement;
using UnityEngine;

public class CombatState : State
{
    private void OnEnable()
    {
        GameplayManager.Instance.waveController.SpawnMonsters();
    }
}
