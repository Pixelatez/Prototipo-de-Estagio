using UnityEngine;

[CreateAssetMenu(fileName = "Arma Ranged", menuName = "Scriptable Objects/Arma Ranged")]
public class ArmaRanged : ArmaBase
{
    public ItemColetado Municao { set { municao = value; } }

    [Header("Valores de Arma Ranged")]

    [SerializeField]
    protected bool usaMunicao;
    [SerializeField, Tooltip("Quanto de munição é usado a cada ataque.")]
    protected int municaoPorUso = 1;
    [SerializeField]
    protected ProjetelBehavior projetelPrefab;

    private ItemColetado municao;

    public override void Ataque(float danoAtributos, PersonagemJogavel jogador, Vector3 direcaoAtaque)
    {
        if (municao != null)
        {
            GameObject projetel = Instantiate(projetelPrefab.gameObject, jogador.transform.position, Quaternion.Euler(direcaoAtaque), jogador.transform);
            ProjetelBehavior projetelScript = projetel.GetComponent<ProjetelBehavior>();
            ItemAuxiliar tipoProjetel = (ItemAuxiliar)municao.ItemInventario;
            projetelScript.Sprite = tipoProjetel.SpriteProjetel;
            projetelScript.Dano = tipoProjetel.Dano + danoAtributos;
            projetelScript.TempoDeVida = tipoProjetel.TempoDeVida;
            projetelScript.Gravidade = tipoProjetel.Gravidade;
            projetelScript.Alvos = tipoProjetel.Alvos;
            projetelScript.enabled = true;
            projetel.transform.GetComponent<Rigidbody2D>().AddForce(projetel.transform.right * tipoProjetel.VelocidadeProjetel, ForceMode2D.Impulse);

            if (usaMunicao) municao.Quantidade -= municaoPorUso;
        }
    }
}