using UnityEngine;

public class InimigoTerrestreMagico : InimigoBase
{
    [Header("Valores Magicos")]

    [SerializeField]
    private ArmaMagica armaEquipada;

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (armaEquipada == null) return;

        if (jogador != null)
        {
            if (!emCooldown)
            {
                Vector3 direcao = (jogador.position - transform.position).normalized;
                armaEquipada.AtaqueMagico(dano, transform, direcao, 3);
                StartCoroutine(Cooldown());
            }
        }
    }
}