using UnityEngine;

[CreateAssetMenu(fileName = "Item Consumivel", menuName = "Scriptable Objects/Item Consumivel")]
public class ItemConsumivel : ItemBase
{
    [Header("Valores De Item Consumivel")]
    public TipoDeConsumivel tipoDeConsumivel;
    public Texture2D texturaItem;

    [HideInInspector]
    public int curaAoUso;

    [HideInInspector]
    public float duracaoBuff;

    public enum TipoDeConsumivel
    {
        Cura,
        Buff
    }
}
