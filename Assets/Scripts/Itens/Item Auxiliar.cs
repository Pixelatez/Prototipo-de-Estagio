using UnityEngine;

[CreateAssetMenu(fileName = "Item Auxiliar", menuName = "Scriptable Objects/Item Auxiliar")]
public class ItemAuxiliar : ItemBase
{
    public TipoAuxiliar TipoDeAuxiliar { get { return tipoDeAuxiliar; } }
    public Sprite SpriteProjetel { get { return spriteProjetel; } }
    public int Defesa { get { return defesa; } }
    public float Dano { get { return dano; } }
    public float VelocidadeProjetel { get { return velocidadeProjetel; } }
    public float TempoDeVida { get { return tempoDeVida; } }
    public float Gravidade { get { return gravidade; } }

    [Header("Valores de Item Auxiliar")]

    [SerializeField]
    protected TipoAuxiliar tipoDeAuxiliar;
    [SerializeField]
    protected Sprite spriteProjetel;

    [SerializeField, HideInInspector]
    protected int defesa;

    [SerializeField, HideInInspector]
    protected float dano;

    [SerializeField, HideInInspector]
    protected float velocidadeProjetel;

    [SerializeField, HideInInspector]
    protected float tempoDeVida;

    [SerializeField, HideInInspector]
    protected float gravidade;

    public enum TipoAuxiliar
    {
        Equipamento,
        Municao
    }
}