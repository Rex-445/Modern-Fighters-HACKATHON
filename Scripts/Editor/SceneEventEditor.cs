using UnityEditor;



[CanEditMultipleObjects]
[CustomEditor(typeof(SceneEvent))]
public class SceneEventEditor : Editor
{

    //Defaults
    SerializedProperty sceneType;
    SerializedProperty targetUnit;
    SerializedProperty time;

    //Movement
    SerializedProperty targetDirection;
    SerializedProperty moveDirection;


    SerializedProperty actionName;


    bool isNone, isMovement, isAction;

    private void OnEnable()
    {
        sceneType = serializedObject.FindProperty("sceneType");
        targetUnit = serializedObject.FindProperty("targetUnit");
        time = serializedObject.FindProperty("time");

        targetDirection = serializedObject.FindProperty("targetDirection");
        moveDirection = serializedObject.FindProperty("moveDirection");
        actionName = serializedObject.FindProperty("actionName");
    }


    public override void OnInspectorGUI()
    {
        SceneEvent sceneEvent = (SceneEvent)target;
        isMovement = sceneEvent.sceneType == SceneEvent.SceneType.movement;
        isAction = sceneEvent.sceneType == SceneEvent.SceneType.action;
        isNone = sceneEvent.sceneType == SceneEvent.SceneType.none;

        serializedObject.Update();


        //Defaults
        EditorGUILayout.PropertyField(sceneType);
        EditorGUILayout.PropertyField(time);


        //Nothing
        if (!isNone)
        {
            EditorGUILayout.PropertyField(targetUnit);
        }


        if (isMovement)
        {
            EditorGUILayout.PropertyField(targetDirection);
            EditorGUILayout.PropertyField(moveDirection);
        }

        //Action
        if (isAction)
        {
            EditorGUILayout.PropertyField(actionName);
        }









        serializedObject.ApplyModifiedProperties();
    }

}
