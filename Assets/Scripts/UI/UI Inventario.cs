using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class UIInventario : MonoBehaviour
{
    public PersonagemJogavel Jogador { get { return jogador; } set { jogador = value; } }
    public InputJogador Input { get { return input; } set { input = value; } }
    public int SlotVazio
    {
        get
        {
            for (int i = 0; i < slotsDeInventario.Length; i++)
            {
                if (slotsDeInventario[i].transform.childCount == 0)
                {
                    return i;
                }
            }

            return -1;
        }
    }

    private PersonagemJogavel jogador;
    private InputJogador input;
    private SlotInventario[] slotsDeInventario;

    #region Inicio e Input

    private void Awake()
    {
        slotsDeInventario = new SlotInventario[transform.GetChild(0).GetChild(0).childCount];

        for (int i = 0; i < transform.GetChild(0).GetChild(0).childCount; i++)
        {
            slotsDeInventario[i] = transform.GetChild(0).GetChild(0).GetChild(i).GetComponent<SlotInventario>();
        }
    }

    private void OnEnable()
    {
        input.Outros.Interagir.Enable();
        input.Outros.PosicaoMouse.Enable();
    }

    private void OnDisable()
    {
        input.Outros.Interagir.Disable();
        input.Outros.PosicaoMouse.Disable();
    }

    #endregion

}