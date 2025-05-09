using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ItemBase))]
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
            meuScript.Quantidade = EditorGUILayout.IntSlider("Quantidade", meuScript.Quantidade, 0, meuScript.StackMaxima);
        }
    }
}

[CustomEditor(typeof(ArmaBase))]
public class ArmasInventarioEditor : ItensInventarioEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();



    }
}

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