using NUnit.Framework;
using Range = UnityEngine.RangeAttribute;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;

public class PersonagemJogavel : MonoBehaviour, IDamageable
{
    public InputJogador Input { set { input = value; } }
    public UIInventario Inventario { get { return UIDeInventario; } }
    public float VidaAtual { get { return vidaAtual; } set { vidaAtual = Mathf.Clamp(vidaAtual + value, 0, vidaMaxima); } }
    public float VidaMaxima { get { return vidaMaxima; } }

    [Header("Variaveis de Vida")]

    [SerializeField]
    protected float vidaAtual = 100f;
    [SerializeField]
    protected float vidaMaxima = 100f;

    [Header("Variaveis de Movimento Horizontal")]

    [SerializeField]
    private float velocidade = 8f;

    [Header("Variaveis de Movimento Vertical")]

    [SerializeField]
    private float gravidade = 3f;
    [SerializeField]
    private float tamanhoPulo = 15f;
    [SerializeField, Tooltip("O quão longe do chão o jogador pode ficar e ainda contar como 'está no chão' para coisas como pulo, gravidade, etc."), Range(0.5f, 0.01f)]
    private float distanciaChao = 0.15f;
    [SerializeField, Tooltip("Quais LayerMasks contam como chão.")]
    private LayerMask maskChao;

    [Header("Outros")]

    [SerializeField]
    protected UIInventario UIDeInventario;
    [SerializeField]
    protected UIDerrota UIDeDerrota;
    [SerializeField]
    protected bool modoDebug = false;

    protected InputJogador input;
    protected Collider2D colisor;
    protected Rigidbody2D rb;
    protected bool noChao, emPulo = false, coyoteTime = false, inventarioAberto = false, morto = false;

    #region Start e Input

    protected virtual void Awake()
    {
        colisor = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        UIDeInventario.transform.GetChild(0).gameObject.SetActive(inventarioAberto);
        UIDeInventario.Jogador = this;
        UIDeDerrota.gameObject.SetActive(false);
    }

    protected virtual void OnEnable()
    {
        input.Movimento.Andar.Enable();
        input.Movimento.Pular.Enable();
        input.Outros.Inventario.Enable();
        input.Movimento.Pular.performed += PuloInput;
        input.Outros.Inventario.performed += InventarioInput;
    }

    protected virtual void OnDisable()
    {
        input.Movimento.Andar.Disable();
        input.Movimento.Pular.Disable();
        input.Outros.Inventario.Disable();
        input.Movimento.Pular.performed -= PuloInput;
        input.Outros.Inventario.performed -= InventarioInput;
    }

    #endregion

    protected virtual void FixedUpdate()
    {
        if (vidaAtual == 0f && !morto)
        {
            Morte();
            morto = true;
            return;
        }
        else if (morto) return;

        // Raycasts dependem da física do Unity, que só é processada a cada FixedUpdate.
        noChao = ChecagemChao();

        // Mudar a gravidade do RigidBody2D para a configurada no script.
        rb.gravityScale = gravidade;

        // Pegar input do jogador.
        Vector2 inputMovimento = input.Movimento.Andar.ReadValue<Vector2>();

        if (inputMovimento.magnitude == 0) // Se não tiver input do jogador, o jogador para de se mover.
        {
            rb.linearVelocityX = 0;
        }
        else
        {
            rb.linearVelocityX = inputMovimento.x * velocidade;
        }
    }

    #region Pulo

    private void PuloInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if ((noChao || coyoteTime) && !emPulo) // Se o jogador estiver no chão ou em Coyote Time e já não estiver em pulo.
            {
                Vector2 forcaPulo = new(0f, tamanhoPulo);
                coyoteTime = false;
                StartCoroutine(Pulo()); // Espera o jogador sair do chão ou uma quantidade de tempo passar para poder pular novamente, evitando multiplos pulos em um.
                rb.AddForce(forcaPulo, ForceMode2D.Impulse);
            }
        }
    }

    private IEnumerator Pulo()
    {
        emPulo = true;
        float tempo = 0.15f;
        while (noChao || coyoteTime) // Esperar até 0.15 segundos passarem ou o jogador não estiver no chão e não estiver em Coyote Time.
        {
            yield return null;
            tempo -= Time.deltaTime;
            if (tempo <= 0f) break;
        }
        emPulo = false;
    }

    #endregion

    #region Inventário

    private void InventarioInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            inventarioAberto = !inventarioAberto;
            UIDeInventario.transform.GetChild(0).gameObject.SetActive(inventarioAberto);
        }
    }

    #endregion

    private bool ChecagemChao()
    {
        // Achar a parte mais inferior do jogador (o "pé" dele)
        Vector2 parteInferior = new(transform.position.x, colisor.bounds.min.y - distanciaChao / 2);
        Vector2 tamanho = new(Mathf.Abs(colisor.bounds.min.x - colisor.bounds.max.x), distanciaChao);

        // Criar Raycast para verificar se tiver chão
        RaycastHit2D hit = Physics2D.BoxCast(parteInferior, tamanho, 0, Vector3.down, distanciaChao / 2, maskChao);

        if (hit.collider == null) // Se não tiver chão:
        {
            return false;
        }
        else // Se tiver:
        {
            return true;
        }
    }

    public void LevarDano(float dano)
    {
        VidaAtual = -dano;
    }

    private void Morte()
    {
        Time.timeScale = 0f;
        UIDeDerrota.gameObject.SetActive(true);
        this.enabled = false;
    }

    protected virtual void OnDrawGizmos()
    {
        if (modoDebug)
        {
            colisor = GetComponent<Collider2D>();
            Vector3 parteInferior = new(transform.position.x, colisor.bounds.min.y - distanciaChao / 2);
            Vector3 tamanho = new(Mathf.Abs(colisor.bounds.min.x - colisor.bounds.max.x), distanciaChao);

            Vector3[] quadradoVertices = new Vector3[4];

            for (int i = 0; i < 4; i++)
            {
                int l = -1;
                int a = -1;

                if (i >= 2) { a = 1; }
                if (i % 3 == 0) { l = 1; }

                quadradoVertices[i] = parteInferior + new Vector3((tamanho.x / 2) * l, (tamanho.y / 2) * a);
            }

            Handles.DrawSolidRectangleWithOutline(quadradoVertices, new Color(0, 1, 0, 0.3f), Color.green);
        }
    }
}
