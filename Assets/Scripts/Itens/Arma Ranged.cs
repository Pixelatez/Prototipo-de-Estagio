using UnityEngine;

[CreateAssetMenu(fileName = "Arma Ranged", menuName = "Scriptable Objects/Arma Ranged")]
public class ArmaRanged : ArmaBase
{
    public ItemAuxiliar Municao { set { municao = value; } }

    [Header("Valores de Arma Ranged")]

    [SerializeField]
    protected bool usaMunicao;

    private ItemAuxiliar municao;

    public override void Ataque(float danoAtributos, PersonagemJogavel jogador, Vector3 direcaoAtaque)
    {
        if (municao != null)
        {

        }
    }
}