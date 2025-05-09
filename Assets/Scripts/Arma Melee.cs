using UnityEngine;

[CreateAssetMenu(fileName = "ArmaMelee", menuName = "Scriptable Objects/ArmaMelee")]
public class ArmaMelee : ArmaBase
{
    [Header("Valores de Hitbox")]

    [Tooltip("O qu�o distante do jogador o ataque acontecer�.")]
    protected Vector3 posicaoHitbox = new(1, 0, 0);

    protected float larguraHitbox;

    protected float alturaHitbox;

    public override void Ataque(float danoAtributos, Vector3 direcaoAtaque)
    {
        Vector3 posiHitboxReal = new(posicaoHitbox.x + larguraHitbox / 2, posicaoHitbox.y);

    }
}
