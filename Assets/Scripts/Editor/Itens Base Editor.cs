using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ItemBase), true)]
public class ItensBaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var meuScript = target as ItemBase;

        GUILayout.Space(5);
        EditorGUILayout.LabelField("Valores de Stack", EditorStyles.boldLabel);
        bool stackavel = EditorGUILayout.Toggle("Item Stackavel", meuScript.itemStackavel);
        serializedObject.FindProperty("itemStackavel").boolValue = stackavel;

        if (meuScript.itemStackavel)
        {
            int stackMaxima = EditorGUILayout.IntField("Stack Maxima", Mathf.Clamp(meuScript.StackMaxima, 1, meuScript.StackMaxima));
            serializedObject.FindProperty("m_StackMaxima").intValue = stackMaxima;
        }

        serializedObject.ApplyModifiedProperties();
    }
}