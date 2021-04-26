using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameSettings
{
    public static string PlayerName = "Player";
    public static Color PlayerColor = Color.white;

    public static int LevelsCompleted = 0;

    public static HandController.MOVE_TYPE MoveType = HandController.MOVE_TYPE.CLICK;

    public static float Volume;
}
