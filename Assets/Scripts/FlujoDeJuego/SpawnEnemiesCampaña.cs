using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemiesCampaña : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject prefab1;
    public GameObject prefab1MovimientoVertical;
    public GameObject prefab2;
    public GameObject prefab3;
    public GameObject prefab4;

    [Header("Puntos de spawn por mapa")]
    public List<Transform> spawnPointsMapa1;
    public List<Transform> spawnPointsMapa2;
    public List<Transform> spawnPointsMapa3;
    public List<Transform> spawnPointsMapa4;
    public List<Transform> spawnPointsMapa5;

    [Header("GameObjects de los mapas")]
    public GameObject mapa1GO;
    public GameObject mapa2GO;
    public GameObject mapa3GO;
    public GameObject mapa4GO;
    public GameObject mapa5GO;

    private GameObject mapaActivoAnterior;
    private List<GameObject> enemigosInstanciados = new List<GameObject>();

    void Update()
    {
        GameObject mapaActivoActual = ObtenerMapaActivo();

        // Solo hacer algo si el mapa activo cambió
        if (mapaActivoActual != mapaActivoAnterior && mapaActivoActual != null)
        {
            mapaActivoAnterior = mapaActivoActual;
            LimpiarEnemigosExistentes();
            StartCoroutine(InstanciarEnemigosConRetraso(mapaActivoActual)); // Llamamos la coroutine para instanciar con retraso
        }
    }

    GameObject ObtenerMapaActivo()
    {
        if (mapa1GO.activeInHierarchy) return mapa1GO;
        if (mapa2GO.activeInHierarchy) return mapa2GO;
        if (mapa3GO.activeInHierarchy) return mapa3GO;
        if (mapa4GO.activeInHierarchy) return mapa4GO;
        if (mapa5GO.activeInHierarchy) return mapa5GO;
        return null;
    }

    // Coroutine que maneja el retraso de la instanciación
    IEnumerator InstanciarEnemigosConRetraso(GameObject mapa)
    {
        yield return new WaitForSeconds(1f); // Espera de 1 segundo

        // Instanciar enemigos después del retraso
        InstanciarEnemigosDelMapa(mapa);
    }

    void InstanciarEnemigosDelMapa(GameObject mapa)
    {
        if (mapa == mapa1GO)
            Instanciar(spawnPointsMapa1, new List<GameObject> { prefab1, prefab1 });
        else if (mapa == mapa2GO)
            Instanciar(spawnPointsMapa2, new List<GameObject> { prefab1MovimientoVertical, prefab1MovimientoVertical, prefab1, prefab2 });
        else if (mapa == mapa3GO)
            Instanciar(spawnPointsMapa3, new List<GameObject> { prefab1, prefab1, prefab1, prefab2, prefab2 });
        else if (mapa == mapa4GO)
            Instanciar(spawnPointsMapa4, new List<GameObject> { prefab1MovimientoVertical, prefab1, prefab1, prefab3, prefab3 });
        else if (mapa == mapa5GO)
            Instanciar(spawnPointsMapa5, new List<GameObject> { prefab4 });
    }

    void Instanciar(List<Transform> puntos, List<GameObject> prefabsAInstanciar)
    {
        if (puntos == null || puntos.Count == 0)
        {
            Debug.LogWarning("No hay puntos de spawn.");
            return;
        }

        if (prefabsAInstanciar == null || prefabsAInstanciar.Count == 0)
        {
            Debug.LogWarning("No hay prefabs para instanciar.");
            return;
        }

        // Instanciar hasta el mínimo entre la cantidad de puntos y prefabs
        int cantidadAInstanciar = Mathf.Min(puntos.Count, prefabsAInstanciar.Count);

        for (int i = 0; i < cantidadAInstanciar; i++)
        {
            if (puntos[i] != null && prefabsAInstanciar[i] != null)
            {
                GameObject enemigo = Instantiate(prefabsAInstanciar[i], puntos[i].position, puntos[i].rotation);
                enemigosInstanciados.Add(enemigo);
            }
        }

        // Opcional: Mostrar advertencia si las cantidades no coinciden
        if (puntos.Count != prefabsAInstanciar.Count)
        {
            Debug.LogWarning($"Se instanciaron {cantidadAInstanciar} enemigos de {prefabsAInstanciar.Count} prefabs y {puntos.Count} puntos de spawn.");
        }
    }

    void LimpiarEnemigosExistentes()
    {
        foreach (GameObject enemigo in enemigosInstanciados)
        {
            if (enemigo != null)
                Destroy(enemigo);
        }
        enemigosInstanciados.Clear();
    }
}
