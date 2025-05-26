using System;
using System.Collections;
using Unity.Burst.Intrinsics;
using UnityEditor;
using UnityEngine;

public class InimigoTerrestreMelee : InimigoBase
{
    [Header("Valores de Combate")]

    [SerializeField]
    protected float dano = 20f;
    [SerializeField, Tooltip("Tempo de espera entre ataques em segundos.")]
    protected float cooldownDeAtaque = 1f;

    [Header("Valores de Hitbox")]

    [SerializeField]
    private Vector3 posicaoHitbox;
    [SerializeField]
    protected float alturaHitbox;
    [SerializeField]
    protected float larguraHitbox;

    private Transform espada;
    protected Vector3 posicaoHitboxReal;
    protected Vector3 direcaoJogador;
    private Vector3 rotacaoForaDoAlcance = Vector3.zero;
    protected bool emCooldown = false;

    protected override void Awake()
    {
        base.Awake();
        espada = transform.GetChild(0);
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (jogador != null)
        {
            // Movimento
            float direcaoJogadorHori = Mathf.Sign(jogador.position.x - transform.position.x);
            if (Mathf.Abs(jogador.position.x - transform.position.x) > 2f)
            {
                rb.linearVelocityX = velocidade * direcaoJogadorHori;
            }

            // Orientação
            direcaoJogador = (jogador.position - transform.position).normalized;
            espada.rotation = Quaternion.LookRotation(Vector3.forward, direcaoJogador) * Quaternion.Euler(0f, 0f, 90f);

            // Sprites
            bool inverter = direcaoJogadorHori != 1;
            GetComponent<SpriteRenderer>().flipX = inverter;
            espada.GetChild(0).GetComponent<SpriteRenderer>().flipY = inverter;
            rotacaoForaDoAlcance = inverter ? new(0f, 0f, 180f) : rotacaoForaDoAlcance = Vector3.zero;

        }
        else
        {
            espada.rotation = Quaternion.Euler(rotacaoForaDoAlcance);
        }
    }

    protected void Ataque()
    {

    }

    private IEnumerator Cooldown()
    {
        emCooldown = true;
        yield return new WaitForSeconds(cooldownDeAtaque);
        emCooldown = false;
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        if (modoDebug)
        {
            
        }
    }
}