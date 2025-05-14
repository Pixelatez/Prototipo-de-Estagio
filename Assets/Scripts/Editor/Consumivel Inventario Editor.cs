using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ItemConsumivel))]
public class ConsumivelInventarioEditor : ItensInventarioEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var meuScript = target as ItemConsumivel;

        GUILayout.Space(5);
        switch (meuScript.tipoDeConsumivel)
        {
            case ItemConsumivel.TipoDeConsumivel.Cura:
                EditorGUILayout.LabelField("Valores de Cura", EditorStyles.boldLabel);
                int cura = EditorGUILayout.IntField("Cura ao Uso", meuScript.curaAoUso);
                serializedObject.FindProperty("curaAoUso").intValue = cura;
                break;
            case ItemConsumivel.TipoDeConsumivel.Buff:
                EditorGUILayout.LabelField("Valores de Buffs", EditorStyles.boldLabel);
                float duracao = EditorGUILayout.FloatField("Duração do Buff", meuScript.duracaoBuff);
                serializedObject.FindProperty("duracaoBuff").floatValue = duracao;
                break;
        }
        serializedObject.ApplyModifiedProperties();
    }
}