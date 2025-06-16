using UnityEngine;

[CreateAssetMenu(fileName = "Arma Base", menuName = "Scriptable Objects/Arma Base")]
public class ArmaBase : ItemBase
{
    public TipoDano TipoDeDano { get { return tipoDeDano; } }
    public float CooldownDeAtaque { get { return cooldownDeAtaque; } }
    public Sprite SpriteEquipado { get {  return spriteEquipado; } }
    public float Knockback { get { return knockback; } }

    [Header("Valores de Arma")]

    [SerializeField]
    protected TipoDano tipoDeDano;

    [SerializeField]
    protected float dano;

    [SerializeField]
    protected float cooldownDeAtaque;

    [SerializeField]
    protected float knockback;

    [SerializeField, Tooltip("Textura da arma nas mãos do jogador.")]
    protected Sprite spriteEquipado;

    [Tooltip("Animação da arma ao atacar.")]
    public AnimationClip animacaoDeAtaque;

    public enum TipoDano
    {
        None,
        Fogo,
        Gelo,
        Terra,
        Raio
    }
}