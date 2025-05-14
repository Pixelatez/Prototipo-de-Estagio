using UnityEngine;
using UnityEngine.EventSystems;

public class SlotInventario : MonoBehaviour, IDropHandler
{
    public ItemColetado ItemNoSlot { get { return itemNoSlot; } }

    private ItemColetado itemNoSlot;

    private void Update()
    {
        if (transform.childCount == 0) itemNoSlot = null;
        else if (transform.childCount == 1)
        {
            itemNoSlot = transform.GetChild(0).GetComponent<ItemColetado>();
        }
    }

    public virtual void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount == 0)
        {
            ItemColetado itemInventario = eventData.pointerDrag.GetComponent<ItemColetado>();
            itemInventario.ParenteDepoisDeSoltar = transform;
        }
        else
        {
            ItemColetado itemTrocando = eventData.pointerDrag.GetComponent<ItemColetado>();
            
            if (itemNoSlot.ItemInventario == itemTrocando.ItemInventario)
            {
                if (itemNoSlot.Quantidade < itemNoSlot.ItemInventario.StackMaxima)
                {
                    int espacoLivre = itemNoSlot.ItemInventario.StackMaxima - itemNoSlot.Quantidade;

                    itemNoSlot.Quantidade += espacoLivre;
                    itemTrocando.Quantidade -= espacoLivre;
                }
                else
                {
                    TrocarSlots(itemTrocando);
                }
            }
            else
            {
                TrocarSlots(itemTrocando);
            }
        }
    }

    private void TrocarSlots(ItemColetado itemNovo)
    {
        itemNoSlot.transform.SetParent(itemNovo.ParenteDepoisDeSoltar);
        itemNoSlot.ParenteDepoisDeSoltar = itemNovo.transform.parent;
        itemNovo.ParenteDepoisDeSoltar = transform;
    }
}