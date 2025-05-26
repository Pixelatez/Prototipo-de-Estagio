using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ItemAuxiliar))]
public class ItensAuxiliarEditor : ItensBaseEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var meuScript = target as ItemAuxiliar;

        GUILayout.Space(5);
        switch (meuScript.TipoDeAuxiliar)
        {
            case ItemAuxiliar.TipoAuxiliar.Equipamento:
                EditorGUILayout.LabelField("Valores de Equipamento", EditorStyles.boldLabel);

                int defesa = EditorGUILayout.IntField("Defesa", meuScript.Defesa);
                serializedObject.FindProperty("defesa").intValue = defesa;

                break;
            case ItemAuxiliar.TipoAuxiliar.Municao:
                EditorGUILayout.LabelField("Valores do Projetel", EditorStyles.boldLabel);

                int alvos = EditorGUILayout.IntField("Layer dos Alvos", meuScript.Alvos);
                serializedObject.FindProperty("alvos").intValue = alvos;

                float dano = EditorGUILayout.FloatField("Dano", meuScript.Dano);
                serializedObject.FindProperty("dano").floatValue = dano;

                float velocidade = EditorGUILayout.FloatField("Velocidade", meuScript.VelocidadeProjetel);
                serializedObject.FindProperty("velocidadeProjetel").floatValue = velocidade;

                float tempoDeVida = EditorGUILayout.FloatField("Tempo de Vida", meuScript.TempoDeVida);
                serializedObject.FindProperty("tempoDeVida").floatValue = tempoDeVida;

                float gravidade = EditorGUILayout.FloatField("Gravidade", meuScript.Gravidade);
                serializedObject.FindProperty("gravidade").floatValue = gravidade;

                serializedObject.FindProperty("defesa").intValue = 0;
                break;
        }
        serializedObject.ApplyModifiedProperties();
    }
}