using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlechasPower : MonoBehaviour
{
    [Header("Configuración")]
    public GameObject flechaPrefab;
    public float tiempoDeVida = 1f;
    public float distancia = 1f;
    public int statePower = 1;

    private List<GameObject> flechasActivas = new List<GameObject>();

    void Start()
    {
        StartCoroutine(CicloFlechas());
    }

    private void Update()
    {

    }

    IEnumerator CicloFlechas()
    {
        while (true)
        {
            DestruirFlechas();

            if (statePower == 1)
            {
                CrearFlecha(Vector2.right, 0f);    // →
                CrearFlecha(Vector2.left, 180f);   // ↓
            } else if (statePower ==2)
            {
                CrearFlecha(Vector2.right, 0f);    // →
                CrearFlecha(Vector2.left, 180f);   // ↓
                tiempoDeVida = 0.7f;
            }
            else if(statePower == 3)
            {
                tiempoDeVida = 0.5f;
                CrearFlecha(Vector2.right, 0f);    // →
                CrearFlecha(Vector2.down, -90f);   // ↓
                CrearFlecha(Vector2.up, 90f);      // ↑
                CrearFlecha(Vector2.left, 180f);   // ←
            }


            yield return new WaitForSeconds(tiempoDeVida);
        }
    }

    void CrearFlecha(Vector2 direccion, float rotacionZ)
    {
        Vector3 posicion = new Vector3(
            transform.position.x + direccion.x * distancia,
            transform.position.y + direccion.y * distancia,
            0 // Z siempre en 0 para 2D
        );

        GameObject flecha = Instantiate(
            flechaPrefab,
            posicion,
            Quaternion.Euler(0, 0, rotacionZ),
            transform
        );

        // Asegurar Z = 0 también en el prefab
        flecha.transform.position = new Vector3(
            flecha.transform.position.x,
            flecha.transform.position.y,
            0
        );

        flechasActivas.Add(flecha);
    }

    void DestruirFlechas()
    {
        foreach (var flecha in flechasActivas)
        {
            if (flecha != null) Destroy(flecha);
        }
        flechasActivas.Clear();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 center = new Vector3(transform.position.x, transform.position.y, 0);
        Gizmos.DrawWireSphere(center, 0.1f);
        Gizmos.DrawLine(center, center + Vector3.up * distancia);
        Gizmos.DrawLine(center, center + Vector3.right * distancia);
        Gizmos.DrawLine(center, center + Vector3.down * distancia);
        Gizmos.DrawLine(center, center + Vector3.left * distancia);
    }
}