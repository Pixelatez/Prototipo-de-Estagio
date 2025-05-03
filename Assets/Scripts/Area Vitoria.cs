using UnityEngine;
using UnityEngine.SceneManagement;

public class AreaVitoria : MonoBehaviour
{
    [Header("Referências")]

    [SerializeField]
    private Transform inimigos;

    private GameObject[] inimigosVivos;
    private int inimigosTotais = 0;

    private void Awake()
    {
        inimigosVivos = new GameObject[inimigos.childCount];
        inimigosTotais = inimigos.childCount;

        for (int i = 0; i < inimigos.childCount; i++)
        {
            inimigosVivos[i] = inimigos.GetChild(i).gameObject;
        }
    }


    public void OnTriggerEnter2D(Collider2D objeto)
    {
        if (objeto.gameObject.layer == 3) // Se for o jogador.
        {
            int inimigosMortos = 0;

            for (int i = 0; i < inimigosTotais; i++)
            {
                if (inimigosVivos[i] == null) // Se o inimigo não existir (estiver "morto").
                {
                    inimigosMortos++;
                }
            }

            if (inimigosMortos == inimigosTotais)
            {
                CompletarFase();   
            }
            else
            {
                Debug.Log("Objetivos ainda não concluidos!");
            }
        }
    }

    private void CompletarFase()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
