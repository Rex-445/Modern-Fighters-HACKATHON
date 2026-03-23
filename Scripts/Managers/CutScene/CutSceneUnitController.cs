using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSceneUnitController : MonoBehaviour
{
    public Unit unit;

    internal float horizontal = 0, vertical = 0;

    private void Update()
    {
        unit.horizontal = horizontal;
        unit.vertical = vertical;
    }

    public void Action(string actionName)
    {
        unit.ForcedAction(actionName);
    }
}
