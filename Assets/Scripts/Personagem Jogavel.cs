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
    public float VidaAtual { get { return vidaAtual; } set { vidaAtual = Mathf.Clamp(vidaAtual + value, 0, vidaMaximaTotal); } }
    public float VidaMaxima { get { return vidaMaximaTotal; } }

    [Header("Equipamento")]

    [SerializeField]
    protected ItemArma armaEquipada;

    [Header("Valores de Movimento")]

    [SerializeField]
    private float gravidade = 3f;
    [SerializeField]
    protected float tamanhoPulo = 15f;
    [SerializeField, Tooltip("O quão longe do chão o jogador pode ficar e ainda contar como 'está no chão' para coisas como pulo, gravidade, etc."), Range(0.5f, 0.01f)]
    private float distanciaChao = 0.15f;
    [SerializeField, Tooltip("Fração de segundo onde o jogador pode pular logo depois de cair de uma plataforma sem pular.")]
    private float coyoteTime = 0.1f;
    [SerializeField, Tooltip("Quais LayerMasks contam como chão.")]
    private LayerMask maskChao;

    [Header("Valores de Nível")]

    [SerializeField, Tooltip("Formula de Exp para próximo nível: 25 * 1.3^(nívelAtual - 2)")]
    protected float expAtual = 0;
    [SerializeField]
    protected int nivelAtual = 1;
    [SerializeField, Tooltip("Quantos pontos de atributo o jogador receberá por nível.")]
    protected int pontosPorNivel = 4;

    [Header("Valores de Atributos Iniciais")]

    [SerializeField, Tooltip("Cada ponto aumenta o dano causado com Armas de Corpo-a-Corpo. (Melee)")]
    protected int forcaBase = 5;
    [SerializeField, Tooltip("Cada ponto aumenta o dano causado com Armas de Projéteis/Longo Alcance. (Ranged)")]
    protected int destrezaBase = 5;
    [SerializeField, Tooltip("Cada ponto aumenta o dano causado com Armas Mágicas. (Mágico)")]
    protected int inteligenciaBase = 5;
    [SerializeField, Tooltip("Cada ponto aumenta a vida do jogador.")]
    protected int vitalidadeBase = 5;
    [SerializeField, Tooltip("Cada ponto diminui o dano total levado por ataques, até o minimo de 1 de dano levado.")]
    protected int resistenciaBase = 5;
    [SerializeField, Tooltip("Cada ponto aumenta a velocidade do jogador.")]
    protected int agilidadeBase = 5;

    [Header("Valores de Status por Atributo")]

    [SerializeField, Tooltip("Quantidade de dano Melee que o jogador causa para cada ponto de Força.")]
    protected float danoMeleeBase = 1f;
    [SerializeField, Tooltip("Quantidade de dano Ranged que o jogador causa para cada ponto de Destreza.")]
    protected float danoRangedBase = 1f;
    [SerializeField, Tooltip("Quantidade de dano Mágico que o jogador causa para cada ponto de Inteligência.")]
    protected float danoMagicoBase = 1f;
    [SerializeField, Tooltip("Quantidade de vida maxima que o jogador ganha para cada ponto em Vitalidade.")]
    protected float vidaMaximaBase = 20f;
    [SerializeField, Tooltip("Quantidade de dano recebido que o jogador diminui para cada ponto em Resistência.")]
    protected float defesaBase = 2f;
    [SerializeField, Tooltip("Quantidade de velocidade que o jogador ganha para cada ponto em Agilidade.")]
    protected float velocidadeBase = 2f;

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
    protected bool noChao, emPulo = false, emCoyoteTime = false, coyoteTimeExpirado = false, inventarioAberto = false, morto = false;
    protected bool emCooldown = false, olhandoDireita = true;
    protected int forcaTotal, destrezaTotal, inteligenciaTotal, vitalidadeTotal, resistenciaTotal, agilidadeTotal;
    protected float danoMeleeTotal, danoRangedTotal, danoMagicoTotal, vidaAtual, vidaMaximaTotal, defesaTotal, velocidadeTotal;

    #region Valores de Ataque Desarmado

    private float danoPunhos = 2f, cooldownPunhos = 1f;
    private float alturaHitboxPunhos = 1.5f, larguraHitboxPunhos = 1.5f;
    private Vector3 posiHitboxPunhos = new(1, 0, 0);

    #endregion

    #region Start e Input

    protected virtual void Awake()
    {
        colisor = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();

        UIDeInventario.transform.GetChild(0).gameObject.SetActive(inventarioAberto);
        UIDeInventario.Jogador = this;
        UIDeDerrota.gameObject.SetActive(false);

        vidaMaximaTotal = vidaMaximaBase * vitalidadeBase;
        vidaAtual = vidaMaximaTotal;
    }

    protected virtual void OnEnable()
    {
        input.Movimento.Andar.Enable();
        input.Movimento.Pular.Enable();
        input.Combate.Ataque.Enable();
        input.Outros.Inventario.Enable();
        input.Movimento.Pular.performed += PuloInput;
        input.Combate.Ataque.performed += AtaqueInput;
        input.Outros.Inventario.performed += InventarioInput;
    }

    protected virtual void OnDisable()
    {
        input.Movimento.Andar.Disable();
        input.Movimento.Pular.Disable();
        input.Combate.Ataque.Disable();
        input.Outros.Inventario.Disable();
        input.Movimento.Pular.performed -= PuloInput;
        input.Combate.Ataque.performed -= AtaqueInput;
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

        #region Atributos Totais

        forcaTotal = forcaBase;
        destrezaTotal = destrezaBase;
        inteligenciaTotal = inteligenciaBase;
        vitalidadeTotal = vitalidadeBase;
        resistenciaTotal = resistenciaBase;
        agilidadeTotal = agilidadeBase;

        #endregion

        #region Status Totais

        danoMeleeTotal = danoMeleeBase * forcaTotal;
        danoRangedTotal = danoRangedBase * destrezaTotal;
        danoMagicoTotal = danoMagicoBase * inteligenciaTotal;
        defesaTotal = defesaBase * resistenciaTotal;
        vidaMaximaTotal = vidaMaximaBase * vitalidadeTotal;
        velocidadeTotal = velocidadeBase * agilidadeTotal;

        #endregion

        noChao = ChecagemChao(); // Raycasts dependem da física do Unity, que só é processada a cada FixedUpdate.
        rb.gravityScale = gravidade;
        Vector2 inputMovimento = input.Movimento.Andar.ReadValue<Vector2>(); // Pegar input do jogador.
        Transform arma = transform.GetChild(0);

        if (inputMovimento.magnitude == 0) // Se não tiver input do jogador, o jogador para de se mover.
        {
            rb.linearVelocityX = 0;
        }
        else
        {
            rb.linearVelocityX = inputMovimento.x * velocidadeTotal;
        }

        if (inputMovimento.x > 0)
        {
            arma.localPosition = new Vector2(1, 0);
            olhandoDireita = true;
        }
        else if (inputMovimento.x < 0)
        {
            arma.localPosition = new Vector2(-1, 0);
            olhandoDireita = false;
        }

        if (noChao && coyoteTimeExpirado) coyoteTimeExpirado = false; // Resetar coyote time para poder acontecer outra vez

        if (!noChao && !emCoyoteTime && !coyoteTimeExpirado) // Se não estiver no chão, em pulo, em Coyote Time e já não ter perdido o Coyote Time
        {
            StartCoroutine(CoyoteTime());
        }
    }

    #region Pulo

    private void PuloInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if ((noChao || emCoyoteTime) && !emPulo) // Se o jogador estiver no chão ou em Coyote Time e já não estiver em pulo.
            {
                Vector2 forcaPulo = new(0f, tamanhoPulo);
                emCoyoteTime = false;
                coyoteTimeExpirado = true;
                StartCoroutine(Pulo()); // Espera o jogador sair do chão ou uma quantidade de tempo passar para poder pular novamente, evitando multiplos pulos em um.
                rb.AddForce(forcaPulo, ForceMode2D.Impulse);
            }
        }
    }

    private IEnumerator Pulo()
    {
        emPulo = true;
        float tempo = 0.15f;
        while (noChao || emCoyoteTime) // Esperar até 0.15 segundos passarem ou o jogador não estiver no chão e não estiver em Coyote Time.
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

    #region Ataque

    private void AtaqueInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!emCooldown)
            {
                emCooldown = true;

                bool ataqueCorpo_a_Corpo = false;
                if (armaEquipada == null) ataqueCorpo_a_Corpo = true;
                else
                {
                    switch (armaEquipada.tipoDeAtaque)
                    {
                        case ItemArma.TipoDeAtaque.Corpo_A_Corpo:
                            ataqueCorpo_a_Corpo = true;
                            break;
                        case ItemArma.TipoDeAtaque.Distancia:
                            ataqueCorpo_a_Corpo = false;
                            break;
                    }
                }

                if (ataqueCorpo_a_Corpo)
                {
                    Vector3 posicaoHitboxReal = Vector3.zero;
                    Vector2 tamanhoHitbox = Vector2.zero;
                    float danoCausado = 0f;

                    if (olhandoDireita)
                    {
                        if (armaEquipada == null)
                        {
                            posicaoHitboxReal = new Vector3(posiHitboxPunhos.x + larguraHitboxPunhos / 2, posiHitboxPunhos.y);
                            tamanhoHitbox = new(larguraHitboxPunhos / 2, alturaHitboxPunhos / 2);
                            danoCausado = danoPunhos + danoMeleeTotal;
                        }
                        else
                        {

                        }
                    }
                    else
                    {
                        if (armaEquipada == null)
                        {
                            posicaoHitboxReal = new Vector3(-posiHitboxPunhos.x - larguraHitboxPunhos / 2, posiHitboxPunhos.y);
                            tamanhoHitbox = new(larguraHitboxPunhos / 2, alturaHitboxPunhos / 2);
                            danoCausado = danoPunhos + danoMeleeTotal;
                        }
                        else
                        {

                        }
                    }

                    Collider2D[] alvos = Physics2D.OverlapBoxAll(transform.position + posicaoHitboxReal, tamanhoHitbox, 0, 7);

                    for (int i = 0; i < alvos.Length; i++)
                    {
                        if (alvos[i].TryGetComponent<IDamageable>(out IDamageable alvo))
                        {
                            alvo.LevarDano(danoCausado);
                        }
                    }
                }
                else
                {

                }

                StartCoroutine(Cooldown());
            }
        }
    }

    private IEnumerator Cooldown()
    {
        if (armaEquipada == null) yield return new WaitForSeconds(cooldownPunhos);
        else if (armaEquipada.tipoDeAtaque == ItemArma.TipoDeAtaque.Corpo_A_Corpo)
        {

        }
        emCooldown = false;
    }

    #endregion

    #region Detecção de Chão e Pulo

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

    private IEnumerator CoyoteTime()
    {
        emCoyoteTime = true;
        yield return new WaitForSeconds(coyoteTime);
        emCoyoteTime = false;
        coyoteTimeExpirado = true;
    }

    #endregion

    public void LevarDano(float dano)
    {
        VidaAtual = -Mathf.Clamp(dano + Mathf.Clamp(dano - resistenciaTotal * defesaBase, 0, dano), 1, dano);
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

            Vector3[] quadradoVerticesChao = new Vector3[4];

            for (int i = 0; i < 4; i++)
            {
                int l = -1;
                int a = -1;

                if (i >= 2) { a = 1; }
                if (i % 3 == 0) { l = 1; }

                quadradoVerticesChao[i] = parteInferior + new Vector3((tamanho.x / 2) * l, (tamanho.y / 2) * a);
            }

            Handles.DrawSolidRectangleWithOutline(quadradoVerticesChao, new Color(0, 1, 0, 0.3f), Color.green);

            
            Vector3 posicaoHitboxGizmos = Vector3.zero;

            if (armaEquipada == null)
            {
                if (olhandoDireita) { posicaoHitboxGizmos = new Vector3(posiHitboxPunhos.x + larguraHitboxPunhos / 2, posiHitboxPunhos.y); }
                else { posicaoHitboxGizmos = new Vector3(-posiHitboxPunhos.x - larguraHitboxPunhos / 2, posiHitboxPunhos.y); }
            }
            else if (armaEquipada.tipoDeAtaque == ItemArma.TipoDeAtaque.Corpo_A_Corpo)
            {

            }
            else
            {
                return;
            }

            Vector3[] quadradoVerticesMelee = new Vector3[4];

            for (int i = 0; i < 4; i++)
            {
                int l = -1;
                int a = -1;

                if (i >= 2) { a = 1; }
                if (i % 3 == 0) { l = 1; }

                quadradoVerticesMelee[i] = transform.position + posicaoHitboxGizmos + new Vector3((larguraHitboxPunhos / 2) * l, (alturaHitboxPunhos / 2) * a);
            }

            Handles.DrawSolidRectangleWithOutline(quadradoVerticesMelee, new Color(1, 0, 0, 0.3f), Color.red);
        }
    }
}