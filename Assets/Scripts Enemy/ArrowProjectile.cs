using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
    [Header("Configuración Básica")]
    public float daño = 10f;
    public float tiempoVida = 5f;
    public float velocidad = 10f;

    [Header("Configuración Visual")]
    public Vector3 escala = new Vector3(0.5f, 0.5f, 0.5f);
    public GameObject efectoImpacto; // Prefab de efecto de impacto (opcional)
    
    [Header("Configuración de Física")]
    public float masa = 0.1f;          // Masa para física de la flecha
    public bool usarGravedad = false; // Si la flecha tendrá gravedad
    public float gravedadPersonalizada = 0.5f; // Valor bajo para simular arco
    
    // Referencias privadas
    private Rigidbody2D rb;
    private Vector2 direccionMovimiento;
    private bool impactado = false;
    
    private void Awake()
    {
        // Aplicar escala al inicio
        transform.localScale = escala;
        
        // Obtener o agregar Rigidbody2D
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        
        // Configurar Rigidbody2D para la flecha
        rb.mass = masa;
        rb.gravityScale = usarGravedad ? gravedadPersonalizada : 0;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.freezeRotation = true; // Para que la rotación sea controlada por el script
    }
    
    private void Start()
    {
        // Destruir después del tiempo de vida
        Destroy(gameObject, tiempoVida);
    }
    
    public void Lanzar(Vector2 direccion)
    {
        // Guardar dirección para actualizar la rotación
        direccionMovimiento = direccion.normalized;
        
        // Aplicar velocidad inicial
        rb.linearVelocity = direccionMovimiento * velocidad;
        
        // Rotar la flecha en la dirección del movimiento
        ActualizarRotacion();
    }
    
    private void Update()
    {
        if (!impactado && rb.linearVelocity.magnitude > 0.1f)
        {
            // Actualizar rotación en cada frame para que siga la trayectoria
            ActualizarRotacion();
        }
    }
    
    private void ActualizarRotacion()
    {
        // Si usamos gravedad, la dirección de movimiento viene de la velocidad actual
        if (usarGravedad && rb.linearVelocity.magnitude > 0.1f)
        {
            direccionMovimiento = rb.linearVelocity.normalized;
        }
        
        // Calcular ángulo para la rotación en Z
        float angulo = Mathf.Atan2(direccionMovimiento.y, direccionMovimiento.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angulo);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (impactado) return; // Evitar múltiples colisiones
        
        // Si golpea al jugador
        if (other.CompareTag("Player"))
        {
            // Aplicar daño (descomentar cuando tengas el componente de salud del jugador)
            // var playerHealth = other.GetComponent<PlayerHealth>();
            // if (playerHealth != null) playerHealth.RecibirDaño(daño);
            
            CrearEfectoImpacto();
            Destroy(gameObject);
        }
        // Si golpea cualquier otro objeto que no sea un enemigo u otra flecha
        else if (!other.CompareTag("Enemy") && !other.CompareTag("Projectile"))
        {
            // Marcar como impactado
            impactado = true;
            
            // Detener movimiento
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.isKinematic = true;
            }
            
            // Crear efecto visual si existe
            CrearEfectoImpacto();
            
            // Si queremos que la flecha quede clavada en la superficie:
            if (other.CompareTag("Ground") || other.CompareTag("Wall"))
            {
                // Desactivar collider para evitar más interacciones
                Collider2D col = GetComponent<Collider2D>();
                if (col != null) col.enabled = false;
                
                // Destruir después de un tiempo
                Destroy(gameObject, 5f);
            }
            else
            {
                // Si golpea otro objeto, destruir inmediatamente
                Destroy(gameObject);
            }
        }
    }
    
    private void CrearEfectoImpacto()
    {
        // Crear efecto de impacto si tiene asignado uno
        if (efectoImpacto != null)
        {
            Instantiate(efectoImpacto, transform.position, Quaternion.identity);
        }
    }
}