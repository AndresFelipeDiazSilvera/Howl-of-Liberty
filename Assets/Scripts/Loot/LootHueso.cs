using UnityEngine;

public class LootHueso : MonoBehaviour
{
    [Header("Configuración del Cofre")]
    [Tooltip("Prefab del cofre que aparecerá al morir")]
    public GameObject cofreHueso;

    [Range(0, 100), Tooltip("Probabilidad de que aparezca un cofre (en %)")]
    public float probabilidadCofre = 10f;

    void OnDestroy()
    {
        // Solo instanciar si el cofre está asignado y el objeto se destruye normalmente
        if (cofreHueso != null && gameObject.scene.isLoaded)
        {
            float randomNumber = Random.Range(0f, 100f);
            if (randomNumber < probabilidadCofre)
            {
                Instantiate(cofreHueso, transform.position, Quaternion.identity);
            }
        }
    }
}