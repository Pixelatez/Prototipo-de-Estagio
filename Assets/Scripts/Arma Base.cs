using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "Arma Base", menuName = "Scriptable Objects/Arma Base")]
public class ArmaBase : ItemBase
{
    [Header("Valores de Arma")]

    public TipoDeAtaque tipoDeAtaque;

    protected float dano;

    public float cooldownDeAtaque;

    private float knockback;

    [Tooltip("Textura da arma nas mãos do jogador.")]
    public Texture2D texturaEquipado;

    [Tooltip("Animação da arma ao atacar.")]
    public AnimationClip animacaoDeAtaque;

    public virtual void Ataque(float danoAtributos, Vector3 direcaoAtaque)
    {

    }

    public enum TipoDeAtaque
    {
        Melee,
        Ranged,
        Magico
    }
}