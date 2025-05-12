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

    [SerializeField, Tooltip("Textura do item no inventário.")]
    protected Sprite spriteItem;

    public Sprite SpriteItem
    {
        get { return spriteItem; }
    }

    [SerializeField, Tooltip("Textura do item largado no chão para coletar.")]
    protected Sprite spriteDropado;

    public Sprite SpriteDropado
    {
        get { return spriteDropado; }
    }
}
