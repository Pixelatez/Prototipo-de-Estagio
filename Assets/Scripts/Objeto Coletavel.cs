using UnityEngine;

public class ObjetoColetavel : MonoBehaviour
{
    [Header("Valores de Coletagem")]

    public ItemBase item;

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
        if (colisor.TryGetComponent<ColetarItens>(out ColetarItens jogador) && colisor.gameObject.layer == 3)
        {
            jogador.ColetarObjeto = this;
            Destroy(gameObject);
        }
    }
}
