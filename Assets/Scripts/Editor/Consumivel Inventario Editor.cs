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
                meuScript.curaAoUso = EditorGUILayout.IntField("Cura ao Uso", meuScript.curaAoUso);
                break;
            case ItemConsumivel.TipoDeConsumivel.Buff:
                EditorGUILayout.LabelField("Valores de Buffs", EditorStyles.boldLabel);
                meuScript.duracaoBuff = EditorGUILayout.FloatField("Duração do Buff", meuScript.duracaoBuff);
                break;
        }
    }
}