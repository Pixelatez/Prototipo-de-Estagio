using System.Collections;
using UnityEditor;
using UnityEngine;

public class InimigoBase : MonoBehaviour, IDamageable
{
    public float VidaAtual { get { return vidaAtual; } set { vidaAtual = Mathf.Clamp(vidaAtual + value, 0, vidaMaxima); } }
    public float VidaMaxima { get { return vidaMaxima; } }
    public float EXPDrop
    {
        get 
        {
            float aleatorio = Random.Range(ExpAoMorrer.x, ExpAoMorrer.y);
            aleatorio -= aleatorio % 0.01f;
            return aleatorio;
        }
    }

    [Header("Valores de Combate")]

    [SerializeField]
    protected float dano = 15f;
    [SerializeField, Tooltip("Tempo de espera entre ataques em segundos.")]
    protected float cooldownDeAtaque = 1f;

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

    [Header("Valores de Detecção")]

    [SerializeField]
    protected float tamanhoAreaDeteccao = 5f;
    [SerializeField, Range(0, 360)]
    private float FOVDeVisao = 30f;
    [SerializeField, Range(0, 15)]
    private int raycasts = 5;

    [Header("Valores de EXP")]

    [SerializeField, Tooltip("Valor de EXP entre X e Y que o inimigo dropa ao morrer.")]
    private Vector2 ExpAoMorrer = new(1, 3);
   
    [Header("Outros")]

    [SerializeField]
    protected bool modoDebug = false;
    [SerializeField]
    private GameObject UIDoInimigo;

    protected Rigidbody2D rb;
    protected Transform jogador;
    protected bool emCooldown = false;
    protected bool morrendo = false;
    protected bool debug = false;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        GameObject UI = Instantiate(UIDoInimigo, gameObject.transform);
        UI.GetComponent<RectTransform>().localPosition = new(0, 1.25f, 0);
        UI.GetComponent<UIInimigo>().PersonagemDoInimigo = this;
    }

    protected virtual void FixedUpdate()
    {
        rb.gravityScale = gravidade;

        if (Selection.Contains(gameObject) && modoDebug) debug = true;
        else debug = false;

        if (vidaAtual <= 0 && !morrendo)
        {
            morrendo = true;
            StartCoroutine(Morrer());
        }
        else if (morrendo) return;

        jogador = DetectarJogador();
    }

    private Transform DetectarJogador()
    {
        Collider2D[] alvos = Physics2D.OverlapCircleAll(transform.position, tamanhoAreaDeteccao, LayerMask.GetMask("Jogador"));
        Transform alvo = null;

        if (alvos.Length > 0)
        {
            for (int i = 0; i < alvos.Length; i++)
            {
                if (alvos[i].TryGetComponent<PersonagemJogavel>(out _))
                {
                    float FOVAtual = -(FOVDeVisao / 2f);
                    float aumentoDeAngulo = FOVDeVisao / raycasts;

                    for (int j = 0; j < raycasts; j++)
                    {
                        Vector3 direcao = Quaternion.AngleAxis(FOVAtual, Vector3.forward) * (alvos[i].transform.position - transform.position);
                        RaycastHit2D hit;
                        LayerMask jogador = 1 << 3;
                        LayerMask obstaculos = 1 << 6;

                        if (hit = Physics2D.Raycast(transform.position, direcao.normalized, tamanhoAreaDeteccao, jogador | obstaculos))
                        {
                            if (hit.transform == alvos[i].transform)
                            {
                                alvo = alvos[i].transform;
                                if (debug) Debug.DrawRay(transform.position, hit.point - (Vector2)transform.position, Color.green);
                            }
                            else
                            {
                                if (debug) Debug.DrawRay(transform.position, hit.point - (Vector2)transform.position, Color.red);
                            }
                        }

                        FOVAtual += aumentoDeAngulo;  
                    }
                }
            }
        }

        return alvo;
    }

    private IEnumerator Morrer()
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

    protected IEnumerator Cooldown()
    {
        emCooldown = true;
        yield return new WaitForSeconds(cooldownDeAtaque);
        emCooldown = false;
    }

    public void LevarDano(float dano)
    {
        VidaAtual = -dano;
    }

    protected virtual void OnDrawGizmosSelected()
    {
        if (modoDebug)
        {
            Handles.DrawWireDisc(transform.position, Vector3.forward, tamanhoAreaDeteccao);
        }
    }
}