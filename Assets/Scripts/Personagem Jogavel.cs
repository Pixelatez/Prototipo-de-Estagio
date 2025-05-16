using NUnit.Framework;
using Range = UnityEngine.RangeAttribute;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;
using Unity.VisualScripting;
using System;

public class PersonagemJogavel : MonoBehaviour, IDamageable
{
    public InputJogador Input { set { input = value; } }
    public UIInventario Inventario { get { return UIDeInventario; } }
    public float VidaAtual { get { return vidaAtual; } set { vidaAtual = Mathf.Clamp(vidaAtual + value, 0, vidaMaximaTotal); } }
    public float VidaMaxima { get { return vidaMaximaTotal; } }
    public float VidaMaximaBase { get { return vidaMaximaBase; } }
    public float ExpAtual { get { return expAtual; } set { expAtual = value; } }
    public int PontosRestantes { get { return pontosRestantes; } set { pontosRestantes = value; } }
    public int PontosForca { get { return forcaBase; } set { forcaBase = value; } }
    public int PontosDestreza { get { return destrezaBase; } set { destrezaBase = value; } }
    public int PontosInteligencia { get { return inteligenciaBase; } set { inteligenciaBase = value; } }
    public int PontosVitalidade { get { return vitalidadeBase; } set { vitalidadeBase = value; } }
    public int PontosResistencia { get { return resistenciaBase; } set { resistenciaBase = value; } }
    public int PontosAgilidade { get { return agilidadeBase; } set { agilidadeBase = value; } }

    [Header("Equipamento")]

    [SerializeField]
    protected ArmaBase armaEquipada;
    [SerializeField]
    protected ItemAuxiliar auxiliarEquipado;
    [SerializeField]
    protected ArmaduraBase capaceteEquipado;
    [SerializeField]
    protected ArmaduraBase peitoralEquipado;
    [SerializeField]
    protected ArmaduraBase calcasEquipado;
    [SerializeField]
    protected ArmaduraBase botasEquipado;

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
    private float expAtual = 0;
    [SerializeField]
    protected int nivelAtual = 1;
    [SerializeField, Tooltip("Quantos Pontos de Atributo o jogador receberá por nível.")]
    protected int pontosPorNivel = 4;
    [SerializeField]
    protected int pontosRestantes = 0;

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
    protected Transform arma;
    [SerializeField]
    protected bool modoDebug = false;

    protected InputJogador input;
    protected Collider2D colisor;
    protected Rigidbody2D rb;
    protected Vector3 direcaoMouse;
    protected Vector2 posicaoMouseRaw = Vector2.zero;
    protected bool noChao, emPulo = false, emCoyoteTime = false, coyoteTimeExpirado = false, inventarioAberto = false, morto = false;
    protected bool emCooldown = false;
    protected int forcaTotal, destrezaTotal, inteligenciaTotal, vitalidadeTotal, resistenciaTotal, agilidadeTotal;
    protected float danoMeleeTotal, danoRangedTotal, danoMagicoTotal, vidaAtual, vidaMaximaTotal, defesaTotal, velocidadeTotal;
    protected float expProximoNivel;

    #region Valores de Ataque Desarmado

    private readonly float danoPunhos = 2f, cooldownPunhos = 1f;
    private readonly float alturaHitboxPunhos = 1.5f, larguraHitboxPunhos = 1.5f;
    private readonly Vector3 posiHitboxPunhos = new(1, 0, 0);

    #endregion

    #region Awake e Input

    protected virtual void Awake()
    {
        colisor = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();

        UIDeInventario.transform.GetChild(0).gameObject.SetActive(inventarioAberto);
        UIDeInventario.Jogador = this;
        UIDeDerrota.gameObject.SetActive(false);

        vidaMaximaTotal = vidaMaximaBase * vitalidadeBase;
        vidaAtual = vidaMaximaTotal;
        EquiparArma();
    }

    protected virtual void OnEnable()
    {
        input.Movimento.Andar.Enable();
        input.Movimento.Pular.Enable();
        input.Combate.Ataque.Enable();
        input.Outros.Inventario.Enable();
        input.Outros.PosicaoMouse.Enable();
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
        input.Outros.PosicaoMouse.Disable();
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

        int defesaArmaduras = 0;
        if (auxiliarEquipado != null) defesaArmaduras += auxiliarEquipado.Defesa;
        if (capaceteEquipado != null) defesaArmaduras += capaceteEquipado.Defesa;
        if (peitoralEquipado != null) defesaArmaduras += peitoralEquipado.Defesa;
        if (calcasEquipado != null) defesaArmaduras += calcasEquipado.Defesa;
        if (botasEquipado != null) defesaArmaduras += botasEquipado.Defesa;

        forcaTotal = forcaBase;
        destrezaTotal = destrezaBase;
        inteligenciaTotal = inteligenciaBase;
        vitalidadeTotal = vitalidadeBase;
        resistenciaTotal = resistenciaBase + defesaArmaduras;
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

        expProximoNivel = 25 * Mathf.Pow(1.3f, Mathf.Clamp(nivelAtual - 2, 0, nivelAtual));
        expProximoNivel -= expProximoNivel % 0.01f; // Remover decimais depois do segundo (Ex: 12,57 ao invés de 12,5795483)

        // Se a exp atual ultrapassar o necessário para o próximo nível
        if (expAtual >= expProximoNivel)
        {
            expAtual = Mathf.Clamp(expAtual - expProximoNivel, 0, expAtual); // Diminuir exp atual pelo necessário para subir de nível, com o minimo de 0.
            nivelAtual++;
            pontosRestantes += pontosPorNivel;
        }

        noChao = ChecagemChao(); // Raycasts dependem da física do Unity, que só é processada a cada FixedUpdate.
        EquiparArma();

        rb.gravityScale = gravidade;

        Vector2 inputMovimento = input.Movimento.Andar.ReadValue<Vector2>(); // Pegar input do mouse se o jogo estiver em foco.

        Vector2 posicaoMouse = Camera.main.ScreenToWorldPoint(posicaoMouseRaw); // Traduzir a posição do mouse na tela para o jogo.
        direcaoMouse = (transform.position - (Vector3)posicaoMouse); // Pegar direção do mouse em relação ao jogador.
        arma.rotation = Quaternion.LookRotation(Vector3.forward, direcaoMouse.normalized) * Quaternion.Euler(0, 0, -90);
        arma.GetChild(0).transform.localPosition = posiHitboxPunhos;

        // Se não tiver input do jogador, o jogador para de se mover.
        if (inputMovimento.magnitude == 0)
        {
            rb.linearVelocityX = 0;
        }
        else
        {
            rb.linearVelocityX = inputMovimento.x * velocidadeTotal;
        }

        // Inverter o sprite dependendo de onde olhar.
        bool inverter = Mathf.Sign(transform.position.x - posicaoMouse.x) != 1;
        GetComponent<SpriteRenderer>().flipX = inverter;
        arma.GetChild(0).GetComponent<SpriteRenderer>().flipY = inverter;

        if (noChao && coyoteTimeExpirado) coyoteTimeExpirado = false; // Resetar coyote time para poder acontecer outra vez

        if (!noChao && !emCoyoteTime && !coyoteTimeExpirado) // Se não estiver no chão, em pulo, em Coyote Time e já não ter perdido o Coyote Time
        {
            StartCoroutine(CoyoteTime());
        }
    }

    private void LateUpdate()
    {
        if (Application.isFocused)
        {
            posicaoMouseRaw = input.Outros.PosicaoMouse.ReadValue<Vector2>(); // Pegar posicao do mouse na tela.
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

                if (armaEquipada != null)
                {
                    float danoAtributos = 0;

                    switch (armaEquipada.TipoDeAtaque)
                    {
                        case ArmaBase.TipoDeDano.Melee:
                            danoAtributos = danoMeleeTotal;
                            break;
                        case ArmaBase.TipoDeDano.Ranged:
                            danoAtributos = danoRangedTotal;
                            break;
                        case ArmaBase.TipoDeDano.Magico:
                            danoAtributos = danoMagicoTotal;
                            break;
                    }

                    armaEquipada.Ataque(danoAtributos, this, arma.eulerAngles);
                }
                else
                {
                    Vector3 posicaoHitboxReal = new(posiHitboxPunhos.x + larguraHitboxPunhos / 2, posiHitboxPunhos.y);
                    posicaoHitboxReal = Quaternion.AngleAxis(arma.eulerAngles.z, Vector3.forward) * posicaoHitboxReal;
                    Vector2 tamanhoHitbox = new(larguraHitboxPunhos, alturaHitboxPunhos);
                    float danoCausado = danoMeleeTotal + danoPunhos;

                    Collider2D[] alvos = Physics2D.OverlapBoxAll(transform.position + posicaoHitboxReal, tamanhoHitbox, arma.eulerAngles.z, 7);

                    for (int i = 0; i < alvos.Length; i++)
                    {
                        if (alvos[i].TryGetComponent<IDamageable>(out IDamageable alvo))
                        {
                            InimigoBase inimigo = alvos[i].GetComponent<InimigoBase>();
                            if (Mathf.Clamp(inimigo.VidaAtual - danoCausado, 0, inimigo.VidaMaxima) == 0)
                            {
                                expAtual += inimigo.EXPDrop;
                            }
                            alvo.LevarDano(danoCausado);
                        }
                    }
                }

                StartCoroutine(Cooldown());
            }
        }
    }

    private IEnumerator Cooldown()
    {
        if (armaEquipada == null) yield return new WaitForSeconds(cooldownPunhos);
        else
        {
            yield return new WaitForSeconds(armaEquipada.CooldownDeAtaque);
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
        // Diminuir dano recebido pela resistência total do jogador, até o minimo de 1 de dano.
        VidaAtual = -Mathf.Clamp(dano + Mathf.Clamp(dano - resistenciaTotal * defesaBase, 0, dano), 1, dano);
    }

    private void Morte()
    {
        Time.timeScale = 0f;
        UIDeDerrota.gameObject.SetActive(true);
        this.enabled = false;
    }

    public void EquiparArma()
    {
        for (int i = 0; i < Inventario.SlotsDeEquipamento.childCount; i++)
        {
            SlotEquipamento slotDeEquipamento = Inventario.SlotsDeEquipamento.GetChild(i).GetComponent<SlotEquipamento>();

            if (slotDeEquipamento.TipoDeSlot != null)
            {
                if (slotDeEquipamento.TipoDeSlot.GetType().IsSubclassOf(typeof(ArmaBase)))
                {
                    if (slotDeEquipamento.ItemNoSlot == null) armaEquipada = null;
                    else armaEquipada = (ArmaBase)slotDeEquipamento.ItemNoSlot.ItemInventario;
                }
                else if (slotDeEquipamento.TipoDeSlot.GetType().IsSubclassOf(typeof(ArmaduraBase)) || slotDeEquipamento.TipoDeSlot is ArmaduraBase)
                {
                    ArmaduraBase slotDeArmadura = (ArmaduraBase)slotDeEquipamento.TipoDeSlot;

                    switch (slotDeArmadura.TipoDeArmadura)
                    {
                        case ArmaduraBase.TipoArmadura.Capacete:
                            if (slotDeEquipamento.ItemNoSlot == null) capaceteEquipado = null;
                            else capaceteEquipado = (ArmaduraBase)slotDeEquipamento.ItemNoSlot.ItemInventario;
                            break;
                        case ArmaduraBase.TipoArmadura.Peitoral:
                            if (slotDeEquipamento.ItemNoSlot == null) peitoralEquipado = null;
                            else peitoralEquipado = (ArmaduraBase)slotDeEquipamento.ItemNoSlot.ItemInventario;
                            break;
                        case ArmaduraBase.TipoArmadura.Calcas:
                            if (slotDeEquipamento.ItemNoSlot == null) calcasEquipado = null;
                            else calcasEquipado = (ArmaduraBase)slotDeEquipamento.ItemNoSlot.ItemInventario;
                            break;
                        case ArmaduraBase.TipoArmadura.Botas:
                            if (slotDeEquipamento.ItemNoSlot == null) botasEquipado = null;
                            else botasEquipado = (ArmaduraBase)slotDeEquipamento.ItemNoSlot.ItemInventario;
                            break;
                    }
                }
                else if (slotDeEquipamento.TipoDeSlot.GetType().IsSubclassOf(typeof(ItemAuxiliar)) || slotDeEquipamento.TipoDeSlot is ItemAuxiliar)
                {
                    if (slotDeEquipamento.ItemNoSlot == null) auxiliarEquipado = null;
                    else auxiliarEquipado = (ItemAuxiliar)slotDeEquipamento.ItemNoSlot.ItemInventario;

                    if (armaEquipada != null)
                    {
                        if (armaEquipada.GetType().IsSubclassOf(typeof(ArmaRanged)))
                        {
                            ArmaRanged ranged = armaEquipada as ArmaRanged;
                            ranged.Municao = auxiliarEquipado;
                        }
                    }
                }
            }
        }
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

            Vector3 posicaoHitboxGizmos;

            if (armaEquipada is ArmaMelee armaMelee)
            {
                posicaoHitboxGizmos = new Vector3(armaMelee.PosicaoHitbox.x + armaMelee.LarguraHitbox / 2, armaMelee.PosicaoHitbox.y);

                Vector3[] quadradoVerticesMelee = new Vector3[4];

                for (int i = 0; i < 4; i++)
                {
                    int l = -1;
                    int a = -1;

                    if (i >= 2) { a = 1; }
                    if (i % 3 == 0) { l = 1; }

                    Vector3 teste = Quaternion.AngleAxis(arma.eulerAngles.z, Vector3.forward) * (posicaoHitboxGizmos + new Vector3((armaMelee.LarguraHitbox / 2) * l, (armaMelee.AlturaHitbox / 2) * a));
                    quadradoVerticesMelee[i] = transform.position + teste;
                }

                Handles.DrawSolidRectangleWithOutline(quadradoVerticesMelee, new Color(1, 0, 0, 0.3f), Color.red);
            }
            else
            {
                posicaoHitboxGizmos = new Vector3(posiHitboxPunhos.x + larguraHitboxPunhos / 2, posiHitboxPunhos.y);

                Vector3[] quadradoVerticesMelee = new Vector3[4];

                for (int i = 0; i < 4; i++)
                {
                    int l = -1;
                    int a = -1;

                    if (i >= 2) { a = 1; }
                    if (i % 3 == 0) { l = 1; }

                    Vector3 teste = Quaternion.AngleAxis(arma.eulerAngles.z, Vector3.forward) * (posicaoHitboxGizmos + new Vector3((larguraHitboxPunhos / 2) * l, (alturaHitboxPunhos / 2) * a));
                    quadradoVerticesMelee[i] = transform.position + teste;
                }

                Handles.DrawSolidRectangleWithOutline(quadradoVerticesMelee, new Color(1, 0, 0, 0.3f), Color.red);
            }
        }
    }
}