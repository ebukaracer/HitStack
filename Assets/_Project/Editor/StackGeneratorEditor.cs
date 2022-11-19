#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(StackGenerator))]
internal class StackGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var stackGenerator = (StackGenerator)target;

        GUILayout.Space(20);
        GUILayout.Label("Stack Generator");

        GUILayout.BeginHorizontal();
        var b1 = GUILayout.Button("Generate Stacks");
        var b4 = GUILayout.Button("Clear Stacks");
        var b6 = GUILayout.Button("Refresh Checkpoints");
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        GUILayout.Label("Material Assignment");
        GUILayout.BeginVertical();
        var b2 = GUILayout.Button("Assign Random Material");
        var b7 = GUILayout.Button("Assign Random Material 2");
        var b5 = GUILayout.Button("Assign Good Material");
        GUILayout.EndVertical();

        if (b1)
            stackGenerator.GenerateStack();

        if (b4)
            stackGenerator.ClearStacks();

        if (b6)
            stackGenerator.RefreshCheckpoint();

        if (b2)
            stackGenerator.AssignRandomMaterial();

        if (b5)
            stackGenerator.AssignOnlyGood();

        if (b7)
            stackGenerator.AssignRandomMaterial2();
    }
}
#endif