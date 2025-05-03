using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PersonagemJogavelMelee : PersonagemJogavel
{
    [Header("Variaveis de Combate")]

    [SerializeField]
    protected float dano = 5f;
    [SerializeField, Tooltip("Tempo de espera entre ataques em segundos.")]
    protected float cooldownDeAtaque = 1f;

    [Header("Valores de Hitbox")]

    [SerializeField]
    private Vector3 posicaoHitbox;
    [SerializeField]
    protected float alturaHitbox;
    [SerializeField]
    protected float larguraHitbox;

    protected Vector3 posicaoHitboxReal;
    protected bool olhandoDireita = true;
    protected bool emCooldown = false;

    #region Input

    protected override void OnEnable()
    {
        base.OnEnable();
        input.Combate.Enable();
        input.Combate.Ataque.performed += AtaqueInput;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        input.Combate.Disable();
        input.Combate.Ataque.performed -= AtaqueInput;
    }

    #endregion

    private void Update()
    {
        Transform espada = transform.GetChild(0);
        Vector2 inputMovimento = input.Movimento.Andar.ReadValue<Vector2>();

        if (olhandoDireita) { posicaoHitboxReal = new Vector3(posicaoHitbox.x + larguraHitbox / 2, posicaoHitbox.y); }
        else { posicaoHitboxReal = new Vector3(-posicaoHitbox.x - larguraHitbox / 2, posicaoHitbox.y); }

        if (inputMovimento.x > 0)
        {
            espada.localPosition = new Vector2(1, 0);
            olhandoDireita = true;
        }
        else if (inputMovimento.x < 0)
        {
            espada.localPosition = new Vector2(-1, 0);
            olhandoDireita = false;
        }
    }

    private void AtaqueInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!emCooldown)
            {
                emCooldown = true;
                Vector2 tamanhoHitbox = new(larguraHitbox / 2, alturaHitbox / 2);
                Collider2D[] alvos = Physics2D.OverlapBoxAll(transform.position + posicaoHitboxReal, tamanhoHitbox, 0, 7);

                for (int i = 0; i < alvos.Length; i++)
                {
                    if (alvos[i].TryGetComponent<IDamageable>(out IDamageable alvo))
                    {
                        alvo.LevarDano(dano);
                    }
                }

                StartCoroutine(Cooldown());
            }
        }
    }

    private IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(cooldownDeAtaque);
        emCooldown = false;
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        if (modoDebug)
        {
            Vector3 posicaoHitboxGizmos;

            if (olhandoDireita) { posicaoHitboxGizmos = new Vector3(posicaoHitbox.x + larguraHitbox / 2, posicaoHitbox.y); }
            else { posicaoHitboxGizmos = new Vector3(-posicaoHitbox.x - larguraHitbox / 2, posicaoHitbox.y); }

            Vector3[] quadradoVertices = new Vector3[4];

            for (int i = 0; i < 4; i++)
            {
                int l = -1;
                int a = -1;

                if (i >= 2) { a = 1; }
                if (i % 3 == 0) { l = 1; }

                quadradoVertices[i] = transform.position + posicaoHitboxGizmos + new Vector3((larguraHitbox / 2) * l, (alturaHitbox / 2) * a);
            }

            Handles.DrawSolidRectangleWithOutline(quadradoVertices, new Color(1, 0, 0, 0.3f), Color.red);
        }
    }
}