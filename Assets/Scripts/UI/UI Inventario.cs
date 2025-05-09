using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

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
    [SerializeField]
    private Transform slotsDeEquipamento;    
    [SerializeField]
    private Transform pontosDeAtributo;

    private PersonagemJogavel jogador;
    private InputJogador input;
    private SlotInventario[] numSlotsNoInventario;
    private Transform[] atributos;
    private TextMeshProUGUI[] valoresDeAtributo;

    private void Awake()
    {
        numSlotsNoInventario = new SlotInventario[slotsDeInventario.childCount];

        for (int i = 0; i < slotsDeInventario.childCount; i++)
        {
            numSlotsNoInventario[i] = slotsDeInventario.GetChild(i).GetComponent<SlotInventario>();
        }

        atributos = new Transform[pontosDeAtributo.childCount - 2];
        valoresDeAtributo = new TextMeshProUGUI[pontosDeAtributo.childCount - 2];

        for (int i = 0; i < pontosDeAtributo.childCount - 2; i++)
        {
            atributos[i] = pontosDeAtributo.GetChild(i + 2).transform;

            if (i == 0) valoresDeAtributo[0] = atributos[i].GetChild(0).GetComponent<TextMeshProUGUI>();
            else valoresDeAtributo[i] = atributos[i].GetChild(1).GetComponent<TextMeshProUGUI>();
        }
    }

    private void Update()
    {
        valoresDeAtributo[0].text = jogador.PontosRestantes.ToString();
        valoresDeAtributo[1].text = jogador.PontosForca.ToString();
        valoresDeAtributo[2].text = jogador.PontosDestreza.ToString();
        valoresDeAtributo[3].text = jogador.PontosInteligencia.ToString();
        valoresDeAtributo[4].text = jogador.PontosVitalidade.ToString();
        valoresDeAtributo[5].text = jogador.PontosResistencia.ToString();
        valoresDeAtributo[6].text = jogador.PontosAgilidade.ToString();
    }

    public void AdicionarForca()
    {
        if (jogador.PontosRestantes > 0)
        {
            jogador.PontosForca += 1;
            jogador.PontosRestantes -= 1;
        }
    }

    public void AdicionarDestreza()
    {
        if (jogador.PontosRestantes > 0)
        {
            jogador.PontosDestreza += 1;
            jogador.PontosRestantes -= 1;
        }
    }

    public void AdicionarInteligencia()
    {
        if (jogador.PontosRestantes > 0)
        {
            jogador.PontosInteligencia += 1;
            jogador.PontosRestantes -= 1;
        }
    }

    public void AdicionarVitalidade()
    {
        if (jogador.PontosRestantes > 0)
        {
            jogador.PontosVitalidade += 1;
            jogador.VidaAtual += jogador.VidaMaximaBase;
            jogador.PontosRestantes -= 1;
        }
    }

    public void AdicionarResistencia()
    {
        if (jogador.PontosRestantes > 0)
        {
            jogador.PontosResistencia += 1;
            jogador.PontosRestantes -= 1;
        }
    }

    public void AdicionarAgilidade()
    {
        if (jogador.PontosRestantes > 0)
        {
            jogador.PontosAgilidade += 1;
            jogador.PontosRestantes -= 1;
        }
    }
}