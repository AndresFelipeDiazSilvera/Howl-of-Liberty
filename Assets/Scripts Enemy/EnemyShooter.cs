using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    [Header("Detección y Objetivo")]
    public float rangoDeteccion = 10f;        // Distancia a la que detecta al jugador
    public LayerMask capaJugador;             // Capa del jugador para detectarlo
    public Transform puntoDisparo;            // Punto desde donde salen los proyectiles
    public bool usarRaycast = true;           // Usar raycast para detección (más preciso)

    [Header("Proyectil")]
    public GameObject prefabProyectil;        // Prefab del proyectil a disparar
    public float velocidadProyectil = 10f;    // Velocidad del proyectil
    public float daño = 10f;                  // Daño que causa el proyectil

    [Header("Configuración de Disparo")]
    public int cantidadProyectiles = 3;       // Cantidad de proyectiles por ráfaga
    public float tiempoEntreProyectiles = 0.2f; // Tiempo entre proyectiles de una misma ráfaga
    public float tiempoRecarga = 10f;         // Tiempo de recarga entre ráfagas (10 segundos)

    [Header("Referencias")]
    public Animator animator;                 // Referencia al Animator
    public Transform barraCarga;              // Barra visual que muestra la recarga (opcional)
    public SpriteRenderer indicadorCarga;     // Indicador visual de carga (opcional)

    // Variables privadas
    private Transform jugador;                // Referencia al transform del jugador
    private bool puedeDisparar = true;        // Estado de disparo
    private bool estaRecargando = false;      // Estado de recarga
    private float tiempoActualRecarga = 0f;   // Contador de tiempo de recarga
    private bool jugadorDetectado = false;    // ¿Se ha detectado al jugador?
    private Vector2 direccionAlJugador;       // Dirección hacia el jugador

    private void Start()
    {
        // Buscar jugador si no se ha asignado
        EncontrarJugador();

        // Si no se asignó un punto de disparo, usar la posición del objeto
        if (puntoDisparo == null)
            puntoDisparo = transform;

        // Si hay una barra de carga, ocultarla al inicio
        if (barraCarga != null)
            barraCarga.localScale = new Vector3(0, 1, 1);

        // Si hay un indicador de carga, ocultarlo al inicio
        if (indicadorCarga != null)
            indicadorCarga.enabled = false;
    }

    private void Update()
    {
        // Buscar al jugador si no lo tenemos
        if (jugador == null)
        {
            EncontrarJugador();
            if (jugador == null) return; // Si no hay jugador, no hacer nada
        }

        // Verificar si el jugador está en rango
        DetectarJugador();

        // Actualizar la recarga si está activa
        if (estaRecargando)
        {
            ActualizarRecarga();
        }

        // Si el jugador está en rango y podemos disparar, hacerlo
        if (jugadorDetectado && puedeDisparar)
        {
            StartCoroutine(DisparoEnRafaga());
        }

        // Voltear el sprite según la dirección (sin mover al enemigo)
        ActualizarOrientacion();
    }

    // Método para encontrar al jugador
    private void EncontrarJugador()
    {
        GameObject objetoJugador = GameObject.FindGameObjectWithTag("Player");
        if (objetoJugador != null)
            jugador = objetoJugador.transform;
    }

    // Método para detectar al jugador usando distancia o raycast
    private void DetectarJugador()
    {
        float distanciaAlJugador = Vector2.Distance(transform.position, jugador.position);
        direccionAlJugador = (jugador.position - transform.position).normalized;

        if (distanciaAlJugador <= rangoDeteccion)
        {
            if (usarRaycast)
            {
                // Usar raycast para verificar que no haya obstáculos entre el arquero y el jugador
                RaycastHit2D hit = Physics2D.Raycast(
                    puntoDisparo.position, 
                    direccionAlJugador, 
                    rangoDeteccion, 
                    capaJugador | LayerMask.GetMask("Default") // Detecta jugador y obstáculos
                );

                // Si el raycast golpea al jugador primero, entonces tenemos línea de visión
                jugadorDetectado = hit.collider != null && hit.collider.CompareTag("Player");
            }
            else
            {
                // Solo usar comprobación de distancia
                jugadorDetectado = true;
            }
        }
        else
        {
            jugadorDetectado = false;
        }

        // Actualizar animador si existe
        if (animator != null)
        {
            animator.SetBool("PlayerDetected", jugadorDetectado);
        }
    }

    // Método para actualizar la orientación (solo voltear sprite, no mover)
    private void ActualizarOrientacion()
    {
        if (jugadorDetectado)
        {
            // Voltear el sprite según la dirección
            if (jugador.position.x > transform.position.x)
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            else if (jugador.position.x < transform.position.x)
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    // Método para actualizar la recarga
    private void ActualizarRecarga()
    {
        tiempoActualRecarga += Time.deltaTime;
        ActualizarIndicadorCarga();

        // Comprobar si ha terminado la recarga
        if (tiempoActualRecarga >= tiempoRecarga)
        {
            estaRecargando = false;
            puedeDisparar = true;
            
            // Resetear indicadores visuales
            if (barraCarga != null)
                barraCarga.localScale = new Vector3(0, 1, 1);
            
            if (indicadorCarga != null)
                indicadorCarga.enabled = false;
            
            // Posible animación de listo para disparar
            if (animator != null)
                animator.SetTrigger("ReadyToShoot");
        }
    }

    // Corrutina para disparar proyectiles en ráfaga
    private IEnumerator DisparoEnRafaga()
    {
        // Evitar disparos múltiples
        puedeDisparar = false;
        
        // Activar animación de ataque si hay un animator
        if (animator != null)
            animator.SetTrigger("Attack");

        // Disparar la cantidad de proyectiles indicada
        for (int i = 0; i < cantidadProyectiles; i++)
        {
            if (jugadorDetectado) // Verificar que el jugador siga siendo detectable
            {
                DispararProyectil();
            }
            
            // Esperar entre proyectiles
            yield return new WaitForSeconds(tiempoEntreProyectiles);
        }

        // Comenzar recarga
        IniciarRecarga();
    }

    [Header("Configuración de la Flecha")]
    public Vector3 escalaFlecha = new Vector3(0.5f, 0.5f, 0.5f); // Escala personalizada para la flecha
    
    // Método para disparar un proyectil
    private void DispararProyectil()
    {
        // Calcular dirección hacia el jugador (actualizada)
        Vector2 direccion = (jugador.position - puntoDisparo.position).normalized;
        
        // Instanciar proyectil
        GameObject proyectil = Instantiate(prefabProyectil, puntoDisparo.position, Quaternion.identity);
        
        // Aplicar escala personalizada a la flecha
        proyectil.transform.localScale = escalaFlecha;
        
        // Configurar el proyectil
        Rigidbody2D rbProyectil = proyectil.GetComponent<Rigidbody2D>();
        if (rbProyectil != null)
        {
            // Aplicar velocidad al proyectil
            rbProyectil.linearVelocity = direccion * velocidadProyectil;
            
            // Calcular la rotación para que apunte en la dirección del movimiento
            float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
            proyectil.transform.rotation = Quaternion.Euler(0, 0, angulo);
        }
        
        // Configurar daño y escala del proyectil si tiene el script de Projectile
        Projectile scriptProyectil = proyectil.GetComponent<Projectile>();
        if (scriptProyectil != null)
        {
            scriptProyectil.daño = daño;
            scriptProyectil.escalaPersonalizada = escalaFlecha; // Pasar la escala al script de proyectil
        }
        
        // Destruir el proyectil después de un tiempo si no tiene su propio sistema
        if (!proyectil.GetComponent<Projectile>() && !proyectil.GetComponent<Rigidbody2D>())
        {
            Destroy(proyectil, 5f);
        }
    }

    // Iniciar el proceso de recarga
    private void IniciarRecarga()
    {
        estaRecargando = true;
        tiempoActualRecarga = 0f;
        
        // Activar animación de recarga si hay animator
        if (animator != null)
            animator.SetTrigger("Reloading");
        
        // Mostrar indicador de carga
        if (indicadorCarga != null)
            indicadorCarga.enabled = true;
    }

    // Actualizar indicador visual de recarga
    private void ActualizarIndicadorCarga()
    {
        // Calcular porcentaje de recarga
        float porcentajeRecarga = tiempoActualRecarga / tiempoRecarga;
        
        // Actualizar barra de carga si existe
        if (barraCarga != null)
        {
            barraCarga.localScale = new Vector3(porcentajeRecarga, 1, 1);
        }
        
        // Cambiar color del indicador según el progreso (opcional)
        if (indicadorCarga != null)
        {
            // Color: rojo -> amarillo -> verde
            indicadorCarga.color = Color.Lerp(Color.red, Color.green, porcentajeRecarga);
        }
    }

    // Visualizar el rango de detección en el editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangoDeteccion);
        
        // Dibujar una línea que muestra la dirección de disparo
        if (jugador != null && jugadorDetectado)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(puntoDisparo.position, puntoDisparo.position + (Vector3)(direccionAlJugador * 2f));
            
            // Si está usando raycast, dibujar el raycast
            if (usarRaycast)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(puntoDisparo.position, direccionAlJugador * rangoDeteccion);
            }
        }
    }
}

public class Projectile : MonoBehaviour
{
    public float daño = 10f;           // Daño que causa el proyectil
    public float tiempoVida = 5f;      // Tiempo de vida del proyectil
    public Vector3 escalaPersonalizada; // Escala personalizada para el proyectil
    
    private void Start()
    {
        // Aplicar escala personalizada si se ha configurado
        if (escalaPersonalizada != Vector3.zero)
        {
            transform.localScale = escalaPersonalizada;
        }
        
        // Destruir el proyectil después de un tiempo
        Destroy(gameObject, tiempoVida);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Si golpea al jugador, aplicar daño
        if (other.CompareTag("Player"))
        {
            // Puedes llamar a un método para dañar al jugador
            // other.GetComponent<PlayerHealth>()?.RecibirDaño(daño);
            
            // Destruir el proyectil al impactar
            Destroy(gameObject);
        }
        else if (!other.CompareTag("Enemy") && !other.CompareTag("Projectile"))
        {
            // Si golpea algo que no sea un enemigo u otro proyectil, destruirlo
            Destroy(gameObject);
        }
    }
}