using UnityEngine;
using UnityEngine.UI;

public class GUIGameplay : MonoBehaviour
{
    [Header("ReferÍncias")]

    [SerializeField]
    private Slider vidaDoJogadorUI;
    [SerializeField]
    private PersonagemJogavel personagemDoJogador;

    private void Update()
    {
        vidaDoJogadorUI.value = Mathf.Clamp(personagemDoJogador.VidaAtual / personagemDoJogador.VidaMaxima, 0, 1);
    }
}
