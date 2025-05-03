using System.Collections;
using UnityEngine;

public class InimigoBase : MonoBehaviour, IDamageable
{
    public float VidaAtual { get { return vidaAtual; } }
    public float VidaMaxima { get { return vidaMaxima; } }

    [Header("Valores de Vida")]

    [SerializeField]
    protected float vidaAtual = 10f;
    [SerializeField]
    protected float vidaMaxima = 10f;

    [Header("Valores de Movimento")]

    [SerializeField]
    protected float velocidade = 4f;
    [SerializeField]
    protected float gravidade = 3f;

    [Header("Outros")]

    [SerializeField]
    protected bool modoDebug = false;
    [SerializeField]
    protected Transform jogador;
    [SerializeField]
    private GameObject UIDoInimigo;

    protected bool morrendo = false;
    protected Rigidbody2D rb;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        GameObject UI = Instantiate(UIDoInimigo, gameObject.transform);
        UI.GetComponent<RectTransform>().localPosition = new(0, 1.25f, 0);
        UI.GetComponent<UIInimigo>().PersonagemDoInimigo = this;
    }

    protected virtual void Update()
    {
        rb.gravityScale = gravidade;

        if (vidaAtual <= 0 && !morrendo)
        {
            morrendo = true;
            StartCoroutine(Morrer());
        }
    }

    private IEnumerator Morrer()
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

    public void LevarDano(float dano)
    {
        vidaAtual = Mathf.Clamp(vidaAtual - dano, 0, vidaMaxima);
    }
}
