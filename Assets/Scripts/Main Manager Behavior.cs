using UnityEngine;

[DefaultExecutionOrder(-5)]
public class MainManagerBehavior : MonoBehaviour
{
    [SerializeField]
    private PersonagemJogavel jogador;

    private InputJogador input;

    private void Awake()
    {
        Time.timeScale = 1;
        input = new InputJogador();
        jogador.Input = input;
        jogador.Inventario.Input = input;
    }

    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }
}
