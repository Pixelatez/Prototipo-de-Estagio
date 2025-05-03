using System.Collections;
using Unity.VisualScripting;
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
    protected bool emCooldown = false;
    protected float direcaoJogador;

    protected override void Awake()
    {
        base.Awake();
        espada = transform.GetChild(0);
    }

    protected void FixedUpdate()
    {
        if (morrendo) return;
        direcaoJogador = Mathf.Sign(jogador.position.x - transform.position.x);

        if (direcaoJogador >= 0)
        {
            posicaoHitboxReal = new Vector3(posicaoHitbox.x + larguraHitbox / 2, posicaoHitbox.y);
            espada.localPosition = new(1f, 0.25f, 0);
        }
        else
        {
            posicaoHitboxReal = new Vector3(-posicaoHitbox.x - larguraHitbox / 2, posicaoHitbox.y);
            espada.localPosition = new(-1f, 0.25f, 0);
        }

        Vector2 tamanhoHitbox = new(larguraHitbox / 2, alturaHitbox / 2);
        Collider2D[] alvos = Physics2D.OverlapBoxAll(transform.position + posicaoHitboxReal, tamanhoHitbox, 0);

        if (alvos.Length > 0)
        {
            for (int i = 0; i < alvos.Length; i++)
            {
                if (alvos[i].TryGetComponent<IDamageable>(out IDamageable alvo) && alvos[i].gameObject.layer == 3)
                {
                    if (!emCooldown)
                    {
                        alvo.LevarDano(dano);
                        StartCoroutine(Cooldown());
                    }
                }
                else
                {
                    rb.linearVelocityX = velocidade * direcaoJogador;
                }
            }
        }
        else
        {
            rb.linearVelocityX = velocidade * direcaoJogador;
        }
    }

    private IEnumerator Cooldown()
    {
        emCooldown = true;
        yield return new WaitForSeconds(cooldownDeAtaque);
        emCooldown = false;
    }

    private void OnDrawGizmos()
    {
        if (modoDebug)
        {
            Vector3 posicaoHitboxGizmos;

            if (direcaoJogador >= 0) { posicaoHitboxGizmos = new Vector3(posicaoHitbox.x + larguraHitbox / 2, posicaoHitbox.y); }
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