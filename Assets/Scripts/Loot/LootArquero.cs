using UnityEngine;

public class LootArquero : MonoBehaviour
{
    [Header("Configuraci�n del Cofre")]
    [Tooltip("Prefab del cofre que aparecer� al morir")]
    public GameObject cofreArco;

    [Range(0, 100), Tooltip("Probabilidad de que aparezca un cofre (en %)")]
    public float probabilidadCofre = 10f;

    void OnDestroy()
    {
        // Solo instanciar si el cofre est� asignado y el objeto se destruye normalmente
        if (cofreArco != null && gameObject.scene.isLoaded)
        {
            float randomNumber = Random.Range(0f, 100f);
            if (randomNumber < probabilidadCofre)
            {
                Instantiate(cofreArco, transform.position, Quaternion.identity);
            }
        }
    }
}