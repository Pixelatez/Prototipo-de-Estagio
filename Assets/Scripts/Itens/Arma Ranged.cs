using UnityEngine;

[CreateAssetMenu(fileName = "Arma Ranged", menuName = "Scriptable Objects/Arma Ranged")]
public class ArmaRanged : ArmaBase
{
    [Header("Valores de Arma Ranged")]

    [SerializeField]
    protected bool usaMunicao;
    [SerializeField, Tooltip("Quanto de munição é usado a cada ataque.")]
    protected int municaoPorUso = 1;
    [SerializeField]
    protected ProjetelBehavior projetelPrefab;

    public override void AtaqueRanged(float danoAtributos, Transform atacante, Vector3 direcaoAtaque, int layerAlvo, ItemColetado municao, ItemAuxiliar tipoMunicao)
    {
        if (tipoMunicao == null && municao != null)
        {
            tipoMunicao = (ItemAuxiliar)municao.ItemInventario;
        }
        
        if (tipoMunicao != null)
        {
            GameObject projetel = Instantiate(projetelPrefab.gameObject, atacante.position, Quaternion.Euler(direcaoAtaque), atacante);
            projetel.transform.localScale = new(0.5f, 0.5f, 0.5f);
            ProjetelBehavior projetelScript = projetel.GetComponent<ProjetelBehavior>();
            ItemAuxiliar tipoProjetel = tipoMunicao;
            projetelScript.Sprite = tipoProjetel.SpriteProjetel;
            projetelScript.Dano = tipoProjetel.Dano + danoAtributos;
            projetelScript.TempoDeVida = tipoProjetel.TempoDeVida;
            projetelScript.Gravidade = tipoProjetel.Gravidade;
            projetelScript.Alvos = layerAlvo;
            projetelScript.enabled = true;

            if (atacante.TryGetComponent<PersonagemJogavel>(out PersonagemJogavel jogador))
            {
                projetelScript.Jogador = jogador;
            }

            projetel.transform.GetComponent<Rigidbody2D>().AddForce(projetel.transform.right * tipoProjetel.VelocidadeProjetel, ForceMode2D.Impulse);

            if (usaMunicao && municao != null) municao.Quantidade -= municaoPorUso;
        }
    }
}