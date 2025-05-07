using UnityEngine;

[CreateAssetMenu(fileName = "Item Arma", menuName = "Scriptable Objects/Item Arma")]
public class ItemArma : ItemBase
{
    [Header("Valores de Arma")]
    public TipoDeAtaque tipoDeAtaque;
    public TipoDeDano tipoDeDano;
    public float dano;

    [Header("Valores de Hitbox")]
    public Vector3 posicaoHitbox;
    public float larguraHitbox;
    public float alturaHitbox;

    public enum TipoDeAtaque
    {
        Corpo_A_Corpo,
        Distancia
    }

    public enum TipoDeDano
    {
        Melee,
        Ranged,
        Magico
    }
}