using UnityEditor;
using UnityEngine;

public class InimigoTerrestreMelee : InimigoBase
{
    [Header("Valores de Melee")]

    [SerializeField]
    protected ArmaMelee armaEquipada;

    protected Transform arma;
    protected Vector3 posicaoHitboxReal;
    protected Vector3 direcaoJogador;
    private Vector3 rotacaoForaDoAlcance = Vector3.zero;
    private bool erro = false;
    protected bool jogadorAlcanceDeAtaque = false;

    protected override void Awake()
    {
        base.Awake();
        arma = transform.GetChild(0);
        arma.GetChild(0).GetComponent<SpriteRenderer>().sprite = armaEquipada.SpriteEquipado;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (armaEquipada == null && !erro)
        {
            Debug.LogError("Inimigo " + gameObject.name + " não contem uma Arma Melee!");
            erro = true;
            return;
        }
        else if (erro) return;

        if (jogador != null)
        {
            jogadorAlcanceDeAtaque = ChecarAtaque();

            // Movimento e Ataque:
            float direcaoJogadorHori = Mathf.Sign(jogador.position.x - transform.position.x);
            if (!jogadorAlcanceDeAtaque) // Se o jogador não pode ser atacado, se mover em sua direção.
            {
                if (Mathf.Abs(jogador.position.x - transform.position.x) > 2f)
                {
                    rb.linearVelocityX = velocidade * direcaoJogadorHori;
                }
            }
            else if (!emCooldown) // Se o jogador pode ser atacado e não estiver em cooldown de ataque.
            {
                armaEquipada.AtaqueMelee(dano, transform, arma.eulerAngles, 1 << 3);
                StartCoroutine(Cooldown());
            }

            // Orientação
            direcaoJogador = (jogador.position - transform.position).normalized;
            arma.rotation = Quaternion.LookRotation(Vector3.forward, direcaoJogador) * Quaternion.Euler(0f, 0f, 90f);

            // Sprites
            bool inverter = direcaoJogadorHori != 1;
            GetComponent<SpriteRenderer>().flipX = inverter;
            arma.GetChild(0).GetComponent<SpriteRenderer>().flipY = inverter;
            arma.GetChild(0).GetComponent<SpriteRenderer>().flipX = inverter;
            rotacaoForaDoAlcance = inverter ? new(0f, 0f, 180f) : rotacaoForaDoAlcance = Vector3.zero;
        }
        else
        {
            arma.rotation = Quaternion.Euler(rotacaoForaDoAlcance);
        }
    }

    protected bool ChecarAtaque()
    {
        Vector3 posicaoHitbox = armaEquipada.PosicaoHitbox;
        float larguraHitbox = armaEquipada.LarguraHitbox;
        float alturaHitbox = armaEquipada.AlturaHitbox;

        Vector3 posiHitboxReal = new(posicaoHitbox.x + larguraHitbox / 2, posicaoHitbox.y);
        posiHitboxReal = Quaternion.AngleAxis(arma.eulerAngles.z, Vector3.forward) * posiHitboxReal;
        Vector3 tamanhoHitbox = new(larguraHitbox, alturaHitbox);

        Collider2D[] alvos = Physics2D.OverlapBoxAll(transform.position + posiHitboxReal, tamanhoHitbox, arma.eulerAngles.z, 1 << 3);

        for (int i = 0; i < alvos.Length; i++)
        {
            if (alvos[i].TryGetComponent<IDamageable>(out _))
            {
                return true;
            }
        }

        return false;
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        if (modoDebug && armaEquipada != null && transform.childCount > 0)
        {
            arma = transform.GetChild(0);

            Vector3 posicaoHitbox = armaEquipada.PosicaoHitbox;
            float larguraHitbox = armaEquipada.LarguraHitbox;
            float alturaHitbox = armaEquipada.AlturaHitbox;

            Vector3 posicaoHitboxGizmos = new(posicaoHitbox.x + larguraHitbox / 2, posicaoHitbox.y);
            Vector3[] quadradoVerticesMelee = new Vector3[4];

            for (int i = 0; i < 4; i++)
            {
                int l = -1;
                int a = -1;

                if (i >= 2) { a = 1; }
                if (i % 3 == 0) { l = 1; }

                Vector3 offset = Quaternion.AngleAxis(arma.eulerAngles.z, Vector3.forward) * (posicaoHitboxGizmos + new Vector3((larguraHitbox / 2) * l, (alturaHitbox / 2) * a));
                quadradoVerticesMelee[i] = transform.position + offset;
            }

            Handles.DrawSolidRectangleWithOutline(quadradoVerticesMelee, new Color(1, 0, 0, 0.3f), Color.red);
        }
    }
}