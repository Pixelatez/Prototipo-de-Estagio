using UnityEngine;

[CreateAssetMenu(fileName = "Arma Magica", menuName = "Scriptable Objects/Arma Magica")]
public class ArmaMagica : ArmaBase
{
    [Header("Valores de Arma Magica")]

    [SerializeField]
    private float tempoDeVida;
    [SerializeField]
    private float velocidadeDeProjetel;
    [SerializeField]
    private ProjetelBehavior projetelPrefab;
    [SerializeField]
    private Sprite projetelSprite;

    public virtual void AtaqueMagico(float danoAtributos, Transform atacante, Vector3 direcaoAtaque, int layerAlvo)
    {
        GameObject projetel = Instantiate(projetelPrefab.gameObject, atacante.position, Quaternion.identity, atacante);
        projetel.transform.localScale = new(0.5f, 0.5f, 0.5f);
        ProjetelBehavior projetelScript = projetel.GetComponent<ProjetelBehavior>();
        projetelScript.Sprite = projetelSprite;
        projetelScript.Dano = danoAtributos + dano;
        projetelScript.TempoDeVida = tempoDeVida;
        projetelScript.Gravidade = 0f;
        projetelScript.TipoDeDano = tipoDeDano;
        projetelScript.Alvos = layerAlvo;
        projetelScript.enabled = true;

        if (atacante.TryGetComponent<PersonagemJogavel>(out PersonagemJogavel jogador))
        {
            projetelScript.Jogador = jogador;
        }

        projetel.transform.GetComponent<Rigidbody2D>().AddForce(direcaoAtaque * velocidadeDeProjetel, ForceMode2D.Impulse);
    }
}