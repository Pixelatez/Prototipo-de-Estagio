using UnityEngine;
using UnityEngine.EventSystems;

public class SlotEquipamento : SlotInventario
{
    public ItemBase TipoDeSlot { get { return tipoDeSlotDeEquipamento; } }

    [SerializeField]
    private ItemBase tipoDeSlotDeEquipamento;

    public override void OnDrop(PointerEventData eventData)
    {
        bool subclasse = eventData.pointerDrag.GetComponent<ItemColetado>().ItemInventario.GetType().IsSubclassOf(tipoDeSlotDeEquipamento.GetType());
        if (subclasse || tipoDeSlotDeEquipamento.GetType() == eventData.pointerDrag.GetComponent<ItemColetado>().ItemInventario.GetType())
        {
            if (tipoDeSlotDeEquipamento.GetType().IsSubclassOf(typeof(ArmaduraBase)) || tipoDeSlotDeEquipamento is ArmaduraBase)
            {
                ArmaduraBase tipoDeSlot = tipoDeSlotDeEquipamento as ArmaduraBase;
                ArmaduraBase tipoDeEquipamento = eventData.pointerDrag.GetComponent<ItemColetado>().ItemInventario as ArmaduraBase;

                if (tipoDeSlot.TipoDeArmadura == tipoDeEquipamento.TipoDeArmadura)
                {
                    base.OnDrop(eventData);
                }
            }
            else
            {
                base.OnDrop(eventData);
            }
        }
    }
}