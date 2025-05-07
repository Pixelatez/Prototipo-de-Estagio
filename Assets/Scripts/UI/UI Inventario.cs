using UnityEngine;

public class UIInventario : MonoBehaviour
{
    public PersonagemJogavel Jogador { get { return jogador; } set { jogador = value; } }
    public InputJogador Input { get { return input; } set { input = value; } }
    public int SlotVazio
    {
        get
        {
            for (int i = 0; i < numSlotsNoInventario.Length; i++)
            {
                if (numSlotsNoInventario[i].transform.childCount == 0)
                {
                    return i;
                }
            }

            return -1;
        }
    }

    public Transform SlotsDeInventario { get { return slotsDeInventario; } }

    [Header("Referências")]

    [SerializeField]
    private Transform slotsDeInventario;

    private PersonagemJogavel jogador;
    private InputJogador input;
    private SlotInventario[] numSlotsNoInventario;

    #region Inicio e Input

    private void Awake()
    {
        numSlotsNoInventario = new SlotInventario[slotsDeInventario.childCount];

        for (int i = 0; i < slotsDeInventario.childCount; i++)
        {
            numSlotsNoInventario[i] = slotsDeInventario.GetChild(i).GetComponent<SlotInventario>();
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