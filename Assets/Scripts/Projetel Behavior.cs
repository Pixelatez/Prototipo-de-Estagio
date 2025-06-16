using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static ArmaBase;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(SpriteRenderer))]
public class ProjetelBehavior : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer spriteRend;
    private PersonagemJogavel m_jogador;

    public PersonagemJogavel Jogador { set { m_jogador = value; } }

    private int m_alvos;

    public int Alvos { set { m_alvos = value; } }

    private Sprite m_sprite;

    public Sprite Sprite { set { m_sprite = value; } }

    private float m_dano = 0;

    public float Dano { set { m_dano = value; } }

    private float m_tempoDeVida = 0;

    public float TempoDeVida { set { m_tempoDeVida = value; } }

    public float Gravidade { set { rb.gravityScale = value; } }

    private TipoDano m_tipoDeDano;

    public TipoDano TipoDeDano { set { m_tipoDeDano = value; } }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRend = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        StartCoroutine(TemporizadorDeVida());
        spriteRend.sprite = m_sprite;
    }

    private void Update()
    {
        float angulo = Mathf.Atan2(rb.linearVelocityY, rb.linearVelocityX) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angulo, Vector3.forward);
    }

    private void OnTriggerEnter2D(Collider2D colisor)
    {
        if (colisor.gameObject.layer == m_alvos && colisor.TryGetComponent(out IDamageable alvo))
        {
            if (m_jogador != null)
            {
                InimigoBase inimigo = colisor.GetComponent<InimigoBase>();
                bool superEfetivo = false;

                switch (m_tipoDeDano)
                {
                    case TipoDano.Fogo:
                        if (inimigo.TipoDeInimigo == InimigoBase.TipoInimigo.Gelo) superEfetivo = true;
                        break;
                    case TipoDano.Gelo:
                        if (inimigo.TipoDeInimigo == InimigoBase.TipoInimigo.Terra) superEfetivo = true;
                        break;
                    case TipoDano.Terra:
                        if (inimigo.TipoDeInimigo == InimigoBase.TipoInimigo.Raio) superEfetivo = true;
                        break;
                    case TipoDano.Raio:
                        if (inimigo.TipoDeInimigo == InimigoBase.TipoInimigo.Fogo) superEfetivo = true;
                        break;
                }

                m_dano *= superEfetivo ? 1.5f : 1f;

                if (Mathf.Clamp(inimigo.VidaAtual - m_dano, 0, inimigo.VidaMaxima) == 0)
                {
                    m_jogador.ExpAtual += inimigo.EXPDrop;
                }
            }

            alvo.LevarDano(m_dano);
            Destroy(gameObject);
        }
        else if (colisor.gameObject.layer == 6)
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator TemporizadorDeVida()
    {
        yield return new WaitForSeconds(m_tempoDeVida);
        Destroy(gameObject);
    }
}
