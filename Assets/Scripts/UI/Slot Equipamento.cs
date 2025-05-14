using UnityEngine;
using UnityEngine.EventSystems;

public class SlotEquipamento : SlotInventario
{
    public ItemBase TipoDeSlot { get { return tipoDeSlotDeEquipamento; } }

    [SerializeField]
    private ItemBase tipoDeSlotDeEquipamento;

    public override void OnDrop(PointerEventData eventData)
    {
        if (tipoDeSlotDeEquipamento.GetType() == eventData.pointerDrag.GetComponent<ItemColetado>().ItemInventario.GetType())
        {
            base.OnDrop(eventData);
        }
    }
}