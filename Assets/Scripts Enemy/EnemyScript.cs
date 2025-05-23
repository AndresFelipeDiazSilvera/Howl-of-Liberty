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
    
    private Animator anim;

    private CircleCollider2D collider;
    // Variables privadas
    private Transform jugador;                   // Referencia al transform del jugador
    private float tiempoUltimoAtaque;            // Tiempo en que se realizó el último ataque
    private bool puedeAtacar = true;             // Bandera para controlar si puede atacar
    private Rigidbody2D rb;                      // Componente Rigidbody2D del enemigo
   
    
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
