using UnityEngine;

public class LootHueso : MonoBehaviour
{
    [Header("Configuraci�n del Cofre")]
    [Tooltip("Prefab del cofre que aparecer� al morir")]
    public GameObject cofreHueso;

    [Range(0, 100), Tooltip("Probabilidad de que aparezca un cofre (en %)")]
    public float probabilidadCofre = 10f;

    void OnDestroy()
    {
        // Solo instanciar si el cofre est� asignado y el objeto se destruye normalmente
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