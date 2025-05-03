using UnityEngine;
using UnityEngine.SceneManagement;

public class UIDerrota : MonoBehaviour
{
    public void ReiniciarFase()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}