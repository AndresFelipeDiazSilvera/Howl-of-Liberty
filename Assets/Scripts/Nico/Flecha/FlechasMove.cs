using UnityEngine;

public class FlechasMove : MonoBehaviour
{
    [Header("Configuración")]
    public float velocidadNormal = 5f;
    public float distanciaPersecucion = 3f;

    private Transform enemigoCercano;
    public FlechasPower flechasPower;

    void Start()
    {
        flechasPower = FindAnyObjectByType<FlechasPower>();
    }
    void Update()
    {
        if(flechasPower.statePower == 2){
            velocidadNormal = 8f;
            distanciaPersecucion = 4f;
        }else if (flechasPower.statePower == 3){
            velocidadNormal = 12f;
            distanciaPersecucion = 5f;
        }
        BuscarEnemigoCercano();
    }

    void FixedUpdate()
    {
        if(enemigoCercano != null)
        {
            // Perseguir enemigo
            Vector2 direccion = (enemigoCercano.position - transform.position).normalized;
            transform.position += (Vector3)direccion * velocidadNormal * Time.fixedDeltaTime;
            
            // Rotar flecha hacia el enemigo
            float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angulo, Vector3.forward);
        }
        else
        {
            // Movimiento normal hacia adelante
            transform.Translate(Vector2.right * velocidadNormal * Time.fixedDeltaTime, Space.Self);
        }
    }

    void BuscarEnemigoCercano()
    {
        enemigoCercano = null;
        float distanciaMasCorta = Mathf.Infinity;

        // Buscar todos los enemigos por tag (sin layers)
        foreach(GameObject enemigo in GameObject.FindGameObjectsWithTag("Enemigo"))
        {
            float distancia = Vector2.Distance(transform.position, enemigo.transform.position);
            if(distancia < distanciaPersecucion && distancia < distanciaMasCorta)
            {
                distanciaMasCorta = distancia;
                enemigoCercano = enemigo.transform;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D otro)
    {
        if(otro.CompareTag("Enemigo"))
        {
            Debug.Log("Golpeado");
            Destroy(gameObject); // Destruye al enemigo
        }
        Destroy(gameObject); // Siempre destruye la flecha
    }

    // Opcional: Ver rango en el Editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaPersecucion);
    }
}