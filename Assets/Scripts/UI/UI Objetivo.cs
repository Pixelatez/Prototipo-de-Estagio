using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class UIObjeto : MonoBehaviour
{
    [Header("Referências")]

    [SerializeField]
    private Transform inimigos;

    private TextMeshProUGUI numDisplay;
    private GameObject[] inimigosVivos;
    private int inimigosTotais;

    private void Awake()
    {
        numDisplay = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        inimigosVivos = new GameObject[inimigos.childCount];
        inimigosTotais = inimigos.childCount;

        for (int i = 0; i < inimigos.childCount; i++)
        {
            inimigosVivos[i] = inimigos.GetChild(i).gameObject;
        }
    }

    private void Update()
    {
        int numInimigosVivos = inimigosTotais;

        for (int i = 0; i < inimigosTotais; i++)
        {
            if (inimigosVivos[i] == null) numInimigosVivos--;
        }

        numDisplay.text = numInimigosVivos.ToString();
    }
}