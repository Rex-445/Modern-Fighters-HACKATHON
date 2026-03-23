using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class SceneEvent : MonoBehaviour
{
    public SceneType sceneType = SceneType.movement;
    public CutSceneUnitController targetUnit;
    public int targetDirection = 1;
    public Vector2 moveDirection = new Vector2(0,0);
    public float time = 1;
    public string actionName = "None";


    public void StartEvent()
    {

        //Reset Everything
        if (sceneType == SceneType.none)
        {
            actionName = "None";
            targetDirection = 0;
            moveDirection = new Vector2(0,0);
        }


        //Generic 
        if (targetDirection != 0)
        {
            targetUnit.unit.ChangeDirection(targetDirection);
        }


        //Basic Movement
        if (sceneType == SceneType.movement)
        {
            targetUnit.horizontal = moveDirection[0];
            targetUnit.vertical = moveDirection[1];
        }

        //Basic Action
        if (sceneType == SceneType.action)
        {
            targetUnit.Action(actionName);
        }
    }

    public void EndEvent()
    {
        targetUnit.horizontal = 0;
        targetUnit.vertical = 0;
    }



    public enum SceneType
    {
        none,
        movement,
        action
    }


}
