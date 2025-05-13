using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyScript : MonoBehaviour
{
    // Variables públicas que se pueden ajustar desde el Inspector
    public float velocidadMovimiento = 2.0f;     // Velocidad a la que se mueve el enemigo
    public float distanciaDeteccion = 5.0f;      // Distancia a la que el enemigo detecta al jugador
    public float distanciaAtaque = 0.5f;         // Distancia a la que el enemigo ataca al jugador
    public float tiempoEntreAtaques = 1.0f;      // Tiempo entre ataques
    public float fuerzaAtaque = 10.0f;           // Fuerza del ataque
    public float tiempoAnimacionDaño = 0.5f;     // Duración de la animación de daño
    
    private Animator anim;
    private CircleCollider2D collider;
    
    // Variables privadas
    private Transform jugador;                   // Referencia al transform del jugador
    private float tiempoUltimoAtaque;            // Tiempo en que se realizó el último ataque
    private bool puedeAtacar = true;             // Bandera para controlar si puede atacar
    private Rigidbody2D rb;                      // Componente Rigidbody2D del enemigo
    
    // Variables para el sistema de daño
    private bool recibiendoDaño = false;         // Bandera para saber si está recibiendo daño
    private float tiempoFinAnimacionDaño;        // Tiempo en que terminará la animación de daño
    
    void Start()
    {
        // Buscar al jugador en la escena
        jugador = GameObject.FindGameObjectWithTag("Player").transform;
        
        // Obtener los componentes necesarios
        rb = GetComponent<Rigidbody2D>();
        
        // Inicializar el tiempo del último ataque
        tiempoUltimoAtaque = -tiempoEntreAtaques;  // Para permitir atacar inmediatamente

        anim = GetComponentInChildren<Animator>();
        collider = GetComponent<CircleCollider2D>();
    }
    
    void Update()
    {
        // Si está recibiendo daño, verificamos si la animación ha terminado
        if (recibiendoDaño)
        {
            if (Time.time >= tiempoFinAnimacionDaño)
            {
                // La animación de daño ha terminado, volvemos al comportamiento normal
                recibiendoDaño = false;
            }
            else
            {
                // Mientras recibe daño, no hace nada más
                return;
            }
        }
        
        // Si no encontramos al jugador, no hacemos nada
        if (jugador == null)
            return;
            
        // Calculamos la distancia al jugador
        float distanciaAlJugador = Vector2.Distance(transform.position, jugador.position);
        
        // Decidimos si debemos perseguir al jugador
        if (distanciaAlJugador <= distanciaDeteccion)
        {
            // Si estamos a distancia de ataque
            if (distanciaAlJugador <= distanciaAtaque)
            {
                // Detenemos el movimiento
                DetenerMovimiento();
                
                // Intentamos atacar
                IntentarAtacar();
            }
            else
            {
                // Si no estamos a distancia de ataque, perseguimos al jugador
                PerseguirJugador();
            }
        }
        else
        {
            // Si el jugador está fuera del rango de detección, detenemos al enemigo
            DetenerMovimiento();
        }
    }
    
    void PerseguirJugador()
    {
        // Si está recibiendo daño, no persigue
        if (recibiendoDaño)
            return;
            
        // Calculamos la dirección hacia el jugador
        Vector2 direccion = (jugador.position - transform.position).normalized;
        collider.enabled = false;
        // Movemos al enemigo hacia el jugador
        rb.linearVelocity = direccion * velocidadMovimiento;
        
        // Volteamos el sprite según la dirección del movimiento
        if (jugador.position.x > transform.position.x)
            transform.localScale = new Vector3(-1, 1, 1);
        else if (jugador.position.x < transform.position.x)
            transform.localScale = new Vector3(1, 1, 1);
    }
    
    void DetenerMovimiento()
    {
        // Detenemos el movimiento del enemigo
        rb.linearVelocity = Vector2.zero;
    }
    
    void IntentarAtacar()
    {
        // Si está recibiendo daño, no ataca
        if (recibiendoDaño)
            return;
            
        // Verificamos si ha pasado suficiente tiempo desde el último ataque
        if (Time.time >= tiempoUltimoAtaque + tiempoEntreAtaques)
        {
            // Realizamos el ataque
            Atacar();
            
            // Actualizamos el tiempo del último ataque
            tiempoUltimoAtaque = Time.time;
        }
    }
    
    void Atacar()
    {
        // Aquí iría la lógica de ataque
        Debug.Log("¡Enemigo atacando al jugador!");
        anim.SetTrigger("2_Attack");
        collider.enabled = true;
        
        // Aquí puedes enviar un mensaje al jugador para que reciba daño
        // Por ejemplo:
        // jugador.GetComponent<JugadorScript>().RecibirDanio(fuerzaAtaque);
    }
    
    // Método para recibir daño
    public void RecibirDaño(float cantidad)
    {
        // Si ya está recibiendo daño, ignoramos
        if (recibiendoDaño)
            return;
            
        // Activamos el estado de recibiendo daño
        recibiendoDaño = true;
        
        // Detenemos el movimiento
        DetenerMovimiento();
        
        // Reproducimos la animación de daño
        anim.SetTrigger("Damage");
        
        // Calculamos cuando terminará la animación
        tiempoFinAnimacionDaño = Time.time + tiempoAnimacionDaño;
        
        // añadir lógica para reducir la vida del enemigo
        // Por ejemplo:
        // vidaActual -= cantidad;
        // if (vidaActual <= 0) Morir();
        
        Debug.Log("Enemigo recibió daño: " + cantidad);
    }
    
    // Detector de colisiones con trigger
    void OnTriggerEnter2D(Collider2D other)
    {
        // Verificamos si colisionó con un ataque
        if (other.CompareTag("ataque"))
        {
            // Calculamos la cantidad de daño (puedes personalizar esto)
            float cantidadDaño = 10f; // Daño fijo por ahora
            
            // También podrías obtener el daño desde el objeto de ataque:
            // float cantidadDaño = other.GetComponent<AtaqueScript>().daño;
            
            // Llamamos al método para recibir daño
            RecibirDaño(cantidadDaño);
        }
    }
    
    // Método para dibujar gizmos en el editor (ayuda visual)
    void OnDrawGizmosSelected()
    {
        // Dibujamos el radio de detección
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaDeteccion);
        
        // Dibujamos el radio de ataque
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaAtaque);
    }
}