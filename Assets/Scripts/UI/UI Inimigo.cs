using UnityEngine;
using UnityEngine.UI;

public class UIInimigo : MonoBehaviour
{
    public InimigoBase PersonagemDoInimigo { set { personagemDoInimigo = value; } }

    private Slider vidaDoInimigoUI;
    private InimigoBase personagemDoInimigo;

    private void Awake()
    {
        vidaDoInimigoUI = transform.GetChild(0).GetChild(0).GetComponent<Slider>();
    }

    private void Update()
    {
        vidaDoInimigoUI.value = Mathf.Clamp(personagemDoInimigo.VidaAtual / personagemDoInimigo.VidaMaxima, 0, 1);
    }
}
