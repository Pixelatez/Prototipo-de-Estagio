using UnityEngine;

[CreateAssetMenu(fileName = "Item Base", menuName = "Scriptable Objects/Item Base")]
public class ItemBase : ScriptableObject
{
    [HideInInspector]
    public bool itemStackavel = false;

    private int m_StackMaxima;

    public int StackMaxima
    {
        get { return m_StackMaxima; }
        set { m_StackMaxima = Mathf.Clamp(value, 1, value); }
    }

    private int m_Quantidade;

    public int Quantidade
    {
        get { return m_Quantidade; }
        set { m_Quantidade = Mathf.Clamp(value, 0, StackMaxima); }
    }
}
