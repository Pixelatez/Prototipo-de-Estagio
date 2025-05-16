using UnityEngine;

[CreateAssetMenu(fileName = "Arma Base", menuName = "Scriptable Objects/Arma Base")]
public class ArmaBase : ItemBase
{
    public TipoDeDano TipoDeAtaque { get { return tipoDeAtaque; } }
    public float CooldownDeAtaque { get { return cooldownDeAtaque; } }
    public float Knockback { get { return knockback; } }

    [Header("Valores de Arma")]

    [SerializeField]
    protected TipoDeDano tipoDeAtaque;

    [SerializeField]
    protected float dano;

    [SerializeField]
    protected float cooldownDeAtaque;

    [SerializeField]
    protected float knockback;

    [Tooltip("Textura da arma nas mãos do jogador.")]
    public Texture2D texturaEquipado;

    [Tooltip("Animação da arma ao atacar.")]
    public AnimationClip animacaoDeAtaque;

    public virtual void Ataque(float danoAtributos, PersonagemJogavel jogador, Vector3 direcaoAtaque)
    {

    }

    public enum TipoDeDano
    {
        Melee,
        Ranged,
        Magico
    }
}