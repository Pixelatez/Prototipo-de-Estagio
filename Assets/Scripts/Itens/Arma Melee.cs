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

    public virtual void AtaqueMelee(float danoAtributos, Transform atacante, Vector3 direcaoAtaque, LayerMask tipoDeAlvo)
    {
        Vector3 posiHitboxReal = new(posicaoHitbox.x + larguraHitbox / 2, posicaoHitbox.y);
        posiHitboxReal = Quaternion.AngleAxis(direcaoAtaque.z, Vector3.forward) * posiHitboxReal;
        Vector3 tamanhoHitbox = new(larguraHitbox, alturaHitbox);
        float danoCausado = danoAtributos + dano;

        Collider2D[] alvos = Physics2D.OverlapBoxAll(atacante.transform.position + posiHitboxReal, tamanhoHitbox, direcaoAtaque.z, tipoDeAlvo);

        for (int i = 0; i < alvos.Length; i++)
        {
            if (alvos[i].TryGetComponent<IDamageable>(out IDamageable alvo))
            {
                if (atacante.TryGetComponent<PersonagemJogavel>(out PersonagemJogavel jogador))
                {
                    InimigoBase inimigo = alvos[i].GetComponent<InimigoBase>();
                    bool superEfetivo = false;

                    switch (tipoDeDano)
                    {
                        case TipoDano.Fogo:
                            if (inimigo.TipoDeInimigo == InimigoBase.TipoInimigo.Gelo) superEfetivo = true;
                            break;
                        case TipoDano.Gelo:
                            if (inimigo.TipoDeInimigo == InimigoBase.TipoInimigo.Terra) superEfetivo = true;
                            break;
                        case TipoDano.Terra:
                            if (inimigo.TipoDeInimigo == InimigoBase.TipoInimigo.Raio) superEfetivo = true;
                            break;
                        case TipoDano.Raio:
                            if (inimigo.TipoDeInimigo == InimigoBase.TipoInimigo.Fogo) superEfetivo = true;
                            break;
                    }

                    danoCausado *= superEfetivo ? 1.5f : 1f;

                    if (Mathf.Clamp(inimigo.VidaAtual - danoCausado, 0, inimigo.VidaMaxima) == 0)
                    {
                        jogador.ExpAtual += inimigo.EXPDrop;
                    }
                }

                alvo.LevarDano(danoCausado);
            }
        }
    }
}
