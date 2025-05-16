using UnityEngine;

public class ObjetoColetavel : MonoBehaviour
{
    [Header("Valores de Coletagem")]

    public ItemBase item;

    [SerializeField]
    private ItemColetado itemNoInventario;

    [SerializeField, HideInInspector]
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

    private SpriteRenderer sRenderer;

    private void Awake()
    {
        sRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (item.SpriteItem != null)
        {
            if (item.SpriteDropado == null) sRenderer.sprite = item.SpriteItem;
            else sRenderer.sprite = item.SpriteDropado;
        }
    }

    private void OnTriggerEnter2D(Collider2D colisor)
    {
        if (colisor.TryGetComponent<PersonagemJogavel>(out PersonagemJogavel jogador) && colisor.gameObject.layer == 3)
        {
            int quantidadeAtual = Quantidade;

            for (int i = 0; i < jogador.Inventario.ItensColetados.Length; i++)
            {
                if (jogador.Inventario.ItensColetados[i] != null) // Se tiver um item no inventário.
                {
                    if (jogador.Inventario.ItensColetados[i].ItemInventario == item) // Se o item for do mesmo tipo do coletavel.
                    {
                        ItemColetado itemAtual = jogador.Inventario.ItensColetados[i];
                        int espacoLivre = itemAtual.ItemInventario.StackMaxima - itemAtual.Quantidade;

                        if (espacoLivre - quantidadeAtual < 0)
                        {
                            itemAtual.Quantidade += espacoLivre;
                            quantidadeAtual -= espacoLivre;
                        }
                        else
                        {
                            itemAtual.Quantidade += quantidadeAtual;
                            quantidadeAtual = 0;
                        }
                    }
                }
            }

            if (quantidadeAtual > 0 && jogador.Inventario.SlotVazio != -1)
            {
                GameObject novoItem = Instantiate(itemNoInventario.gameObject, jogador.Inventario.SlotsDeInventario.GetChild(jogador.Inventario.SlotVazio));
                ItemColetado scriptItem = novoItem.GetComponent<ItemColetado>();
                scriptItem.Inventario = jogador.Inventario;
                scriptItem.ParenteDepoisDeSoltar = novoItem.transform.parent;
                scriptItem.ItemInventario = item;
                scriptItem.Quantidade = quantidadeAtual;
            }
            else
            {
                return;
            }

            Destroy(gameObject);
        }
    }
}
