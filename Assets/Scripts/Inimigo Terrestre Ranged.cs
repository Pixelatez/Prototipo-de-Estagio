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
            if (!emCooldown)    
            {
                Vector2 direcaoRad = CalcularDirecao(jogador.position, jogador.GetComponent<Rigidbody2D>().linearVelocity, municaoEquipada.VelocidadeProjetel);
                float direcaoDeg = Mathf.Atan2(direcaoRad.x, direcaoRad.y) * Mathf.Rad2Deg;
                armaEquipada.AtaqueRanged(dano, transform, new Vector3(0, 0, direcaoDeg), 3, null, municaoEquipada);
                StartCoroutine(Cooldown());
            }
        }
    }

    protected Vector2 CalcularDirecao(Vector2 alvo, Vector2 velocidadeAlvo, float velocidadeProjetel)
    {
        Vector2 distancia = alvo - (Vector2)transform.position;

        // Desbalanceamento entre a velocidade do jogador e o projétil.
        float a = Vector2.Dot(velocidadeAlvo, velocidadeAlvo) - velocidadeProjetel * velocidadeProjetel;
        // Produto escalar entre a distância inicial e a velocidade do jogador.
        float b = 2 * Vector2.Dot(distancia, velocidadeAlvo);
        // O quadrado da distância entre o inimigo e o jogador no início.
        float c = Vector3.Dot(distancia, distancia);

        float discriminante = b * b - 4 * a * c;

        if (discriminante < 0)
        {
            return (alvo - (Vector2)transform.position).normalized;
        }

        float t1 = (-b + Mathf.Sqrt(discriminante)) / (2 * a);
        float t2 = (-b - Mathf.Sqrt(discriminante)) / (2 * a);

        float t = Mathf.Min(t1, t2);
        if (t < 0) t = Mathf.Max(t1, t2);   
        if (t < 0)
        {
            return (alvo - (Vector2)transform.position).normalized;
        }

        // Posição futura do jogador.
        Vector2 posiFutura = alvo + velocidadeAlvo * t;

        // Compensar pela gravidade.
        float offsetVertical = 0.5f + municaoEquipada.Gravidade * t * t;
        posiFutura.y += offsetVertical;

        Vector2 direcao = (posiFutura - (Vector2)transform.position).normalized;
        return direcao; 
    }
}