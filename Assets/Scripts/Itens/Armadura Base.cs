using UnityEngine;

[CreateAssetMenu(fileName = "Armadura Base", menuName = "Scriptable Objects/Armadura Base")]
public class ArmaduraBase : ItemBase
{
    public int Defesa { get { return defesa; } }
    public TipoArmadura TipoDeArmadura { get { return tipoDeArmadura; } }

    [Header("Valores de Armadura")]

    [SerializeField]
    protected int defesa;

    [SerializeField]
    protected TipoArmadura tipoDeArmadura;

    public enum TipoArmadura
    {
        Capacete,
        Peitoral,
        Calcas,
        Botas
    }
}