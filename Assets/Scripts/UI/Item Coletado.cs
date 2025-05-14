using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ItemColetado : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public ItemBase ItemInventario { get { return item; } set { item = value; } }
    public Transform ParenteDepoisDeSoltar { get { return parenteDepoisDeSoltar; } set { parenteDepoisDeSoltar = value; } }
    public UIInventario Inventario { get { return Inventario; } set { inventario = value; } }

    [Header("Outros")]

    [SerializeField]
    private UIInventario inventario;

    private Image imagem;
    private TextMeshProUGUI texto;
    private Transform parenteDepoisDeSoltar;
    private ItemBase item;
    private int m_Quantidade;

    public int Quantidade
    {
        get { return m_Quantidade; }
        set
        {
            if (item == null) m_Quantidade = 1;
            else m_Quantidade = Mathf.Clamp(value, 1, item.StackMaxima);
        }
    }

    private void Awake()
    {
        imagem = GetComponent<Image>();
        texto = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        imagem.sprite = item.SpriteItem;
        if (item.itemStackavel) texto.text = Quantidade.ToString();
        else texto.text = string.Empty;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        parenteDepoisDeSoltar = transform.parent;
        transform.SetParent(transform.root);
        imagem.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = inventario.Input.Outros.PosicaoMouse.ReadValue<Vector2>();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(parenteDepoisDeSoltar);
        imagem.raycastTarget = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (item is ItemConsumivel itemConsumivel)
        {
            ItemConsumivel.TipoDeConsumivel tipoDeConsumivel = itemConsumivel.tipoDeConsumivel;

            switch (tipoDeConsumivel)
            {
                case ItemConsumivel.TipoDeConsumivel.Cura:
                    inventario.Jogador.VidaAtual = 25f;
                    break;
                case ItemConsumivel.TipoDeConsumivel.Buff:
                    break;
            }

            if (!itemConsumivel.itemStackavel || (itemConsumivel.itemStackavel && Quantidade <= 1)) Destroy(gameObject);
            else Quantidade--;
        }
    }
}
