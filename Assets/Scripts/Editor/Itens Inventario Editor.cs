using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ItemBase), true)]
public class ItensInventarioEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var meuScript = target as ItemBase;

        GUILayout.Space(5);
        EditorGUILayout.LabelField("Valores de Stack", EditorStyles.boldLabel);
        meuScript.itemStackavel = EditorGUILayout.Toggle("Item Stackavel", meuScript.itemStackavel);

        if (meuScript.itemStackavel)
        {
            meuScript.StackMaxima = EditorGUILayout.IntField("Stack Maxima", meuScript.StackMaxima);
        }
    }
}