using UnityEngine;

[CreateAssetMenu(fileName = "Arma Melee", menuName = "Scriptable Objects/Arma Melee")]
public class ArmaMelee : ArmaBase
{
    public Vector3 PosicaoHitbox { get { return posicaoHitbox; } }
    public float LarguraHitbox { get { return larguraHitbox; } }
    public float AlturaHitbox { get { return alturaHitbox; } }

    [Header("Valores de Hitbox")]

    [SerializeField, Tooltip("O quão distante do jogador o ataque acontecerá.")]
    protected Vector3 posicaoHitbox = new(1, 0, 0);

    [SerializeField]
    protected float larguraHitbox;

    [SerializeField]
    protected float alturaHitbox;

    public override void Ataque(float danoAtributos, PersonagemJogavel jogador, Vector3 direcaoAtaque)
    {
        Vector3 posiHitboxReal = new(posicaoHitbox.x + larguraHitbox / 2, posicaoHitbox.y);
        posiHitboxReal = Quaternion.AngleAxis(direcaoAtaque.z, Vector3.forward) * posiHitboxReal;
        Vector3 tamanhoHitbox = new(larguraHitbox, alturaHitbox);
        float danoCausado = danoAtributos + dano;

        Collider2D[] alvos = Physics2D.OverlapBoxAll(jogador.transform.position + posiHitboxReal, tamanhoHitbox, direcaoAtaque.z, 7);

        for (int i = 0; i < alvos.Length; i++)
        {
            if (alvos[i].TryGetComponent<IDamageable>(out IDamageable alvo))
            {
                InimigoBase inimigo = alvos[i].GetComponent<InimigoBase>();
                if (Mathf.Clamp(inimigo.VidaAtual - danoCausado, 0, inimigo.VidaMaxima) == 0)
                {
                    jogador.ExpAtual += inimigo.EXPDrop;
                }
                alvo.LevarDano(danoCausado);
            }
        }
    }
}
