using UnityEngine;

public class ObjetoColetavel : MonoBehaviour
{
    [Header("Valores de Item de Cura")]

    [SerializeField]
    private float curaAoUso = 25f;
    [SerializeField]
    private ItemColetado itemDeCuraInventario;

    private void OnTriggerEnter2D(Collider2D colisor)
    {
        if (colisor.gameObject.layer == 3)
        {
            PersonagemJogavel jogador = colisor.GetComponent<PersonagemJogavel>();
            if (jogador.Inventario.SlotVazio != -1)
            {
                GameObject itemInventario = Instantiate(itemDeCuraInventario.gameObject, jogador.Inventario.transform.GetChild(0).GetChild(0).GetChild(jogador.Inventario.SlotVazio));
                ItemColetado scriptItem = itemInventario.GetComponent<ItemColetado>();
                scriptItem.Inventario = jogador.Inventario;
                scriptItem.CuraAoUso = curaAoUso;
                Destroy(gameObject);
            }
            else return;
        }
    }
}
