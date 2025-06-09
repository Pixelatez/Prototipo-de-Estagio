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
            if (!emCooldown && CalcularDirecao(jogador.position, jogador.GetComponent<Rigidbody2D>().linearVelocity, municaoEquipada.VelocidadeProjetel, out Quaternion direcaoQuat))   
            {
                Vector3 direcao = direcaoQuat.eulerAngles;
                armaEquipada.AtaqueRanged(dano, transform, new Vector3(0, 0, direcao.z), 3, null, municaoEquipada);
                StartCoroutine(Cooldown());
            }
        }
    }

    protected bool CalcularDirecao(Vector2 alvo, Vector2 velocidadeAlvo, float velocidadeProjetel, out Quaternion direcaoQuat)
    {
        direcaoQuat = Quaternion.identity;

        Vector2 dir = (Vector2)transform.position - alvo;
        float absX = Mathf.Abs(dir.x);
        float grav = municaoEquipada.Gravidade;
        float veloQuadrado = velocidadeProjetel * velocidadeProjetel;

        float discriminante = veloQuadrado * veloQuadrado - grav * (grav * absX * absX + 2 * dir.y * veloQuadrado);

        if (discriminante < 0) return false; // Alvo fora do alcance.

        float raiz = Mathf.Sqrt(discriminante);

        float angulo = Mathf.Atan((veloQuadrado - raiz) / (grav * absX));

        // Converter ângulo para vetor de direção:
        Vector2 direcao = new Vector2(Mathf.Cos(angulo) * Mathf.Sign(dir.x), Mathf.Sin(angulo)).normalized;

        direcaoQuat = Quaternion.LookRotation(Vector3.forward, direcao) * Quaternion.Euler(0, 0, -90);
        return true; 
    }
}