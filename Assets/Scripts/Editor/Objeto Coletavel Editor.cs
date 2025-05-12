using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ObjetoColetavel))]
public class ObjetoColetavelEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var meuScript = target as ObjetoColetavel;

        if (meuScript.item != null)
        {
            if (meuScript.item.itemStackavel)
            {
                int quantidade = EditorGUILayout.IntSlider("Quantidade", meuScript.Quantidade, 1, meuScript.item.StackMaxima);
                serializedObject.FindProperty("m_Quantidade").intValue = quantidade;
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
