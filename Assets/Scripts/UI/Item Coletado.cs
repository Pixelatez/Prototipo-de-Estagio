using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemColetado : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public Transform ParenteDepoisDeSoltar { set { parenteDepoisDeSoltar = value; } }
    public UIInventario Inventario { set { inventario = value; } }
    public float CuraAoUso { set { curaAoUso = value; } }

    [SerializeField]
    private UIInventario inventario;

    private Image imagem;
    private Transform parenteDepoisDeSoltar;
    private float curaAoUso = 0f;

    private void Awake()
    {
        imagem = GetComponent<Image>();
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
        inventario.Jogador.VidaAtual = Mathf.Clamp(inventario.Jogador.VidaAtual + curaAoUso, 0, inventario.Jogador.VidaMaxima);
        Destroy(gameObject);
    }
}
