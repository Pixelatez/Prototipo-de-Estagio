using UnityEngine;

public class InimigoTerrestreRanged : InimigoBase
{
    [Header("Valores de Ranged")]

    [SerializeField]
    protected ArmaRanged armaEquipada;
    [SerializeField]
    protected ItemAuxiliar municaoEquipada;

    protected bool erro = false;

    protected override void Awake()
    {
        base.Awake();
        if (armaEquipada == null || municaoEquipada == null) erro = true;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (erro) return;

        if (jogador != null)
        {
            if (EncontrarTrajeto(jogador.position, jogador.GetComponent<Rigidbody2D>().linearVelocity, out Vector2 trajeto) && !emCooldown)
            {
                armaEquipada.AtaqueRanged(dano, transform, trajeto, 3, null, municaoEquipada);
                StartCoroutine(Cooldown());
            }
        }
    }

    protected bool EncontrarTrajeto(Vector2 alvo, Vector2 velocidadeAlvo, out Vector2 trajeto)
    {
        Vector2 delta = alvo - (Vector2)transform.position;

        // Desbalanceamento entre a velocidade do jogador e o projétil.
        float a = Vector2.Dot(velocidadeAlvo, velocidadeAlvo) - municaoEquipada.VelocidadeProjetel * municaoEquipada.VelocidadeProjetel;
        // Produto escalar entre a distância inicial e a velocidade do jogador.
        float b = 2 * Vector2.Dot(delta, velocidadeAlvo);
        // O quadrado da distância entre o inimigo e o jogador no início.
        float c = Vector3.Dot(delta, delta);

        float discriminante = b * b - 4 * a * c;

        if (discriminante < 0)
        {
            trajeto = Vector2.zero;
            return false;
        }

        float t1 = (-b + Mathf.Sqrt(discriminante)) / (2 * a);
        float t2 = (-b - Mathf.Sqrt(discriminante)) / (2 * a);

        float t = Mathf.Max(t1, t2);

        if (t <= 0)
        {
            trajeto = Vector2.zero;
            return false;
        }

        // Posição futura do jogador.
        Vector2 posiFutura = alvo + velocidadeAlvo * t;

        // Calcular velocidade com componente vertical afetado pela gravidade.
        Vector2 distAlvo = posiFutura - (Vector2)transform.position;

        float trajetoY = (distAlvo.y + 0.5f * municaoEquipada.Gravidade * t * t) / t;
        float trajetoX = distAlvo.x / t;

        Debug.Log("Trajeto da flecha: " + trajetoX + " : " + trajetoY);
        trajeto = new Vector2(trajetoX, trajetoY);
        return true; 
    }
}