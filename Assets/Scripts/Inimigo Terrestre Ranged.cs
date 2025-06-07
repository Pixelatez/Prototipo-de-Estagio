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
        Vector2 direcao = alvo - (Vector2)transform.position;
        return direcao; 
    }
}