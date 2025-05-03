using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlotInventario : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount == 0)
        {
            ItemColetado itemInventario = eventData.pointerDrag.GetComponent<ItemColetado>();
            itemInventario.ParenteDepoisDeSoltar = transform;
        }
    }
}
