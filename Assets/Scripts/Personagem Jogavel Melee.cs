using UnityEngine;
using UnityEngine.InputSystem;

public class PersonagemJogavelMelee : PersonagemJogavel
{
    [Header("Variaveis de Combate")]

    [SerializeField]
    private float dano = 5f;
    [SerializeField]
    private float cooldownDeAtaque = 1f;

    private bool olhandoDireita = true;

    #region Input

    protected override void OnEnable()
    {
        base.OnEnable();
        input.Combate.Enable();
        input.Combate.Ataque.performed += AtaqueInput;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        input.Combate.Disable();
        input.Combate.Ataque.performed -= AtaqueInput;
    }

    #endregion

    private void Update()
    {
        Transform espada = transform.GetChild(0);
        Vector2 inputMovimento = input.Movimento.Andar.ReadValue<Vector2>();

        if (inputMovimento.x > 0)
        {
            espada.localPosition = new Vector2(1, 0);
            olhandoDireita = true;
        }
        else if (inputMovimento.x < 0)
        {
            espada.localPosition = new Vector2(-1, 0);
            olhandoDireita = false;
        }
    }

    private void AtaqueInput(InputAction.CallbackContext context)
    {

    }
}