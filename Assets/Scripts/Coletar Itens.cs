using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using static UnityEditor.Progress;

[RequireComponent(typeof(PersonagemJogavel))]
public class ColetarItens : MonoBehaviour
{
    [Header("Prefab do Item no Inventário")]

    [SerializeField]
    private ItemColetado itemNoInventario;

    private PersonagemJogavel jogador;
    private UIInventario inventario;
    private bool coletando = false;
    private readonly List<ObjetoColetavel> objetosParaColetar = new();

    public ObjetoColetavel ColetarObjeto { set { objetosParaColetar.Add(value); } }

    private void Awake()
    {
        jogador = transform.GetComponent<PersonagemJogavel>();
        inventario = jogador.Inventario;
    }

    private void Update()
    {
        if (objetosParaColetar.Count > 0 && !coletando) // Se tiver um objeto para coletar:
        {
            coletando = true;

            for (int i = objetosParaColetar.Count -1; i >= 0; i--) // Para cada objeto:
            {
                int quantidadeAtual = objetosParaColetar[i].Quantidade;

                for (int j = 0; j < inventario.ItensColetados.Length; j++)
                {
                    if (inventario.ItensColetados[j] != null) // Se tiver um item no inventário.
                    {
                        if (inventario.ItensColetados[j].ItemInventario == objetosParaColetar[i].item) // Se o item for do mesmo tipo do coletavel.
                        {
                            ItemColetado itemAtual = inventario.ItensColetados[j];
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

                if (quantidadeAtual > 0 && inventario.SlotVazio != -1)
                {
                    GameObject novoItem = Instantiate(itemNoInventario.gameObject, inventario.SlotsDeInventario.GetChild(inventario.SlotVazio));
                    inventario.AtualizarInventario();
                    ItemColetado scriptItem = novoItem.GetComponent<ItemColetado>();
                    scriptItem.Inventario = inventario;
                    scriptItem.ParenteDepoisDeSoltar = novoItem.transform.parent;
                    scriptItem.ItemInventario = objetosParaColetar[i].item;
                    scriptItem.Quantidade = quantidadeAtual;
                }
                else
                {
                    return;
                }

                objetosParaColetar.RemoveAt(i);
            }

            coletando = false;
        }
    }
}