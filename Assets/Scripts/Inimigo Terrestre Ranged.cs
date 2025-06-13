using UnityEngine;

public class InimigoTerrestreRanged : InimigoBase
{
    [Header("Valores de Ranged")]

    [SerializeField]
    protected ArmaRanged armaEquipada;
    [SerializeField]
    protected ItemAuxiliar municaoEquipada;

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (armaEquipada == null || municaoEquipada == null) return;

        if (jogador != null)
        {
            if (!emCooldown && CalcularDirecao(jogador.position, municaoEquipada.VelocidadeProjetel, municaoEquipada.Gravidade, out float angulo))
            {
                Vector2 direcao = new(Mathf.Cos(angulo * Mathf.Deg2Rad), Mathf.Sin(angulo * Mathf.Deg2Rad));
                direcao *= jogador.position.x >= transform.position.x ? 1f : -1f;

                armaEquipada.AtaqueRanged(dano, transform, direcao, 3, null, municaoEquipada);
                StartCoroutine(Cooldown());
            }
        }
    }

    protected bool CalcularDirecao(Vector2 alvo, float velocidadeProjetel, float graviFlecha, out float angulo)
    {
        angulo = 0f;

        Vector2 distancia = alvo - (Vector2)transform.position;
        float gravidade = Mathf.Abs(Physics2D.gravity.y * graviFlecha);

        float veloQuadrado = velocidadeProjetel * velocidadeProjetel;
        float raizBaixa = veloQuadrado * veloQuadrado - gravidade * (gravidade * distancia.x * distancia.x + 2 * distancia.y * veloQuadrado);

        if (raizBaixa < 0) return false;

        float raiz = Mathf.Sqrt(raizBaixa);

        float tan = (veloQuadrado - raiz) / (gravidade * distancia.x);
        float anguloRad = Mathf.Atan(tan);

        angulo = anguloRad * Mathf.Rad2Deg;
        return true; 
    }
}