using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Configuración de Spawn")]
    public GameObject enemyPrefab;          // El prefab del enemigo que vamos a generar
    public int maxEnemies = 5;              // Número máximo de enemigos simultáneos
    public float spawnDelay = 2f;           // Tiempo entre generaciones de enemigos
    
    [Header("Área de Spawn")]
    public float spawnRadius = 10f;         // Radio donde aparecerán los enemigos (alrededor del spawner)
    public bool useSpawnPoints;             // Si es true, usa puntos específicos en lugar del radio
    public Transform[] spawnPoints;         // Puntos específicos donde pueden aparecer los enemigos
    
    // Variables privadas
    private List<GameObject> activeEnemies = new List<GameObject>();
    private Transform playerTransform;
    
    void Start()
    {
        // Buscar al jugador
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        
        if (playerTransform == null)
        {
            Debug.LogError("¡No se encontró ningún objeto con la etiqueta 'Player'!");
            return;
        }
        
        // Comenzar la rutina de generación de enemigos
        StartCoroutine(SpawnEnemies());
    }
    
    IEnumerator SpawnEnemies()
    {
        while (true)
        {
            // Limpiar la lista de enemigos (eliminar los nulos/destruidos)
            activeEnemies.RemoveAll(enemy => enemy == null);
            
            // Si no hemos alcanzado el límite máximo de enemigos, generamos uno nuevo
            if (activeEnemies.Count < maxEnemies)
            {
                SpawnEnemy();
            }
            
            // Esperar antes de la siguiente generación
            yield return new WaitForSeconds(spawnDelay);
        }
    }
    
    void SpawnEnemy()
    {
        Vector3 spawnPosition;
        
        if (useSpawnPoints && spawnPoints.Length > 0)
        {
            // Elegir un punto de spawn aleatorio de los disponibles
            int randomIndex = Random.Range(0, spawnPoints.Length);
            spawnPosition = spawnPoints[randomIndex].position;
        }
        else
        {
            // Generar una posición aleatoria dentro del radio
            Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
            spawnPosition = transform.position + new Vector3(randomCircle.x, randomCircle.y, 0);
        }
        
        // Instanciar el enemigo en la posición calculada
        GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        
        // Agregar el enemigo a la lista de activos
        activeEnemies.Add(newEnemy);
        
        // Mostrar mensaje en consola
        Debug.Log("Enemigo generado en: " + spawnPosition);
    }
    
    // Método para dibujar gizmos en el editor (ayuda visual)
    void OnDrawGizmosSelected()
    {
        // Dibujar el área de spawn
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
        
        // Dibujar los puntos de spawn si están configurados
        if (useSpawnPoints && spawnPoints != null)
        {
            Gizmos.color = Color.red;
            foreach (Transform point in spawnPoints)
            {
                if (point != null)
                    Gizmos.DrawSphere(point.position, 0.3f);
            }
        }
    }
}