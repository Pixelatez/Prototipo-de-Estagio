using NUnit.Framework;
using Range = UnityEngine.RangeAttribute;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PersonagemJogavel : MonoBehaviour
{
    [Header("Variaveis de Movimento Horizontal")]

    [SerializeField]
    private float velocidade = 8f;

    [Header("Variaveis de Movimento Vertical")]

    [SerializeField]
    private float gravidade = 3f;
    [SerializeField]
    private float tamanhoPulo = 15f;
    [SerializeField, Tooltip("O qu�o longe do ch�o o jogador pode ficar e ainda contar como 'est� no ch�o' para coisas como pulo, gravidade, etc."), Range(0.5f, 0.01f)]
    private float distanciaChao = 0.15f;
    [SerializeField, Tooltip("Quais LayerMasks contam como ch�o.")]
    private LayerMask maskChao;

    [Header("Outros")]

    [SerializeField]
    protected bool modoDebug = false;

    protected InputJogador input;
    private Collider2D colisor;
    private Rigidbody2D rb;
    private bool noChao;
    private bool emPulo = false;
    private bool coyoteTime = false;

    #region Start e Input

    protected virtual void Awake()
    {
        input = new InputJogador();
        colisor = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void OnEnable()
    {
        input.Enable();
        input.Movimento.Andar.Enable();
        input.Movimento.Pular.Enable();
        input.Movimento.Pular.performed += PuloInput;
    }

    protected virtual void OnDisable()
    {
        input.Disable();
        input.Movimento.Andar.Disable();
        input.Movimento.Pular.Disable();
        input.Movimento.Pular.performed -= PuloInput;
    }

    #endregion

    protected virtual void FixedUpdate()
    {
        // Raycasts dependem da f�sica do Unity, que s� � processada a cada FixedUpdate.
        noChao = ChecagemChao();

        // Mudar a gravidade do RigidBody2D para a configurada no script.
        rb.gravityScale = gravidade;

        // Pegar input do jogador.
        Vector2 inputMovimento = input.Movimento.Andar.ReadValue<Vector2>();

        if (inputMovimento.magnitude == 0) // Se n�o tiver input do jogador, o jogador para de se mover.
        {
            rb.linearVelocityX = 0;
        }
        else
        {
            rb.linearVelocityX = inputMovimento.x * velocidade;
        }
    }

    private void PuloInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if ((noChao || coyoteTime) && !emPulo) // Se o jogador estiver no ch�o ou em Coyote Time e j� n�o estiver em pulo.
            {
                Vector2 forcaPulo = new Vector2(0f, tamanhoPulo);
                coyoteTime = false;
                StartCoroutine(Pulo()); // Espera o jogador sair do ch�o ou uma quantidade de tempo passar para poder pular novamente, evitando multiplos pulos em um.
                rb.AddForce(forcaPulo, ForceMode2D.Impulse);
            }
        }
    }

    private IEnumerator Pulo()
    {
        emPulo = true;
        float tempo = 0.15f;
        while (noChao || coyoteTime) // Esperar at� 0.15 segundos passarem ou o jogador n�o estiver no ch�o e n�o estiver em Coyote Time.
        {
            yield return null;
            tempo -= Time.deltaTime;
            if (tempo <= 0f) break;
        }
        emPulo = false;
    }

    private bool ChecagemChao()
    {
        // Achar a parte mais inferior do jogador (o "p�" dele)
        Vector2 parteInferior = new Vector2(transform.position.x, colisor.bounds.min.y);

        // Criar Raycast para verificar se tiver ch�o
        RaycastHit2D hit = Physics2D.Raycast(parteInferior, Vector3.down, distanciaChao, maskChao);

        if (hit.collider == null) // Se n�o tiver ch�o:
        {
            if (modoDebug) Debug.DrawRay(parteInferior, Vector3.down * distanciaChao, Color.red);
            return false;
        }
        else // Se tiver:
        {
            if (modoDebug) Debug.DrawRay(parteInferior, hit.point - parteInferior, Color.green);
            return true;
        }
    }
}
