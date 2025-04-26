using Unity.VisualScripting;
using UnityEngine;

public class Camera : MonoBehaviour
{
    [SerializeField]
    private Transform objetoParaSeguir;

    void Update()
    {
        transform.position = new Vector3(objetoParaSeguir.position.x, objetoParaSeguir.position.y, transform.position.z);
    }
}
