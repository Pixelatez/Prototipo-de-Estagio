using UnityEngine;

[CreateAssetMenu(fileName = "ItemArma", menuName = "Scriptable Objects/ItemArma")]
public class ItemArma : ItemBase
{
    public TipoDeAtaque tipoDeAtaque;

    public enum TipoDeAtaque
    {
        Corpo_A_Corpo,
        Distancia
    }
}