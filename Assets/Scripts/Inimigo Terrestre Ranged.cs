using UnityEngine;

public class InimigoTerrestreRanged : InimigoBase
{
    [Header("Valores de Combate")]

    [SerializeField]
    protected ArmaRanged armaEquipada;
    [SerializeField]
    protected ItemAuxiliar municaoEquipada;

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (armaEquipada != null)
        {
            
        }
    }

    protected void EncontrarTrajeto()
    {
        float veloProjetel = municaoEquipada.VelocidadeProjetel;
        float gravidade = municaoEquipada.Gravidade;

        Vector2 teste = transform.position;
    }
}