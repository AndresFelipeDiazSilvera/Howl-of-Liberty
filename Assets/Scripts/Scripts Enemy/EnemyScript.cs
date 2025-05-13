using UnityEngine;
using System.Collections;

public class EnemyScript : MonoBehaviour
{
    private bool esperandoParaAtacar = false;
    [Header("Movimiento")]
    public float velocidadMovimiento = 2.0f;
    public float distanciaDeteccion = 5.0f;
    public float distanciaAtaque = 1f;
    public float tiempoEntreAtaques = 1.0f;
    public float tiempoInmovilizacionAtaque = 2.0f;
    public float ajusteVertical = -0.4f;

    [Header("Estados")]
    public bool estaAtacando { get; private set; }

    private Animator anim;
    private CircleCollider2D attackCollider;
    private Transform jugador;
    private Rigidbody2D rb;
    private float tiempoUltimoAtaque;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        attackCollider = GetComponent<CircleCollider2D>();

        if (attackCollider != null)
            attackCollider.enabled = false;

        tiempoUltimoAtaque = -tiempoEntreAtaques;
    }

    void Update()
    {
        if (jugador == null || !jugador.gameObject.activeInHierarchy || estaAtacando)
        {
            BuscarJugador(); // Intenta actualizar si el actual está desactivado o nulo
            return;
        }

        float distanciaAlJugador = Vector2.Distance(transform.position, jugador.position);
        if(jugador.gameObject.name == "Player_Human")
        {
            distanciaAtaque = 0.8f;
        }
        else
        {
            distanciaAtaque = 1.2f;
        }
        if (distanciaAlJugador <= distanciaDeteccion)
        {
            if (distanciaAlJugador > distanciaAtaque)
            {
                PerseguirJugador();
            }
            else
            {
                AlinearHorizontalmente();

                if (!esperandoParaAtacar)
                    StartCoroutine(EsperarAntesDeAtacar());
            }
        }
        else
        {
            DetenerMovimiento();
        }
    }
    public void BuscarJugador()
    {
        GameObject jugadorGO = GameObject.FindGameObjectWithTag("Player");
        if (jugadorGO != null)
        {
            jugador = jugadorGO.transform;
        }
        else
        {
            Debug.LogWarning("No se encontró un objeto con la etiqueta 'Player'");
        }
    }
    void AlinearHorizontalmente()
    {
        // Ajuste en Y para alinear horizontalmente sin moverse en X
        float desplazamientoY = (jugador.position.y - 0.4f) + ajusteVertical - transform.position.y;
        rb.linearVelocity = new Vector2(0, desplazamientoY) * velocidadMovimiento;

        // Voltear sprite hacia el jugador
        transform.localScale = new Vector3(jugador.position.x > transform.position.x ? -1 : 1, 1, 1);
    }

    void PerseguirJugador()
    {
        Vector2 direccion = (new Vector3(
            jugador.position.x,
            jugador.position.y + ajusteVertical,
            jugador.position.z
        ) - transform.position).normalized;

        rb.linearVelocity = direccion * velocidadMovimiento;
        transform.localScale = new Vector3(direccion.x > 0 ? -1 : 1, 1, 1);
    }

    void DetenerMovimiento()
    {
        rb.linearVelocity = Vector2.zero;
    }

    void IntentarAtacar()
    {
        if (Time.time >= tiempoUltimoAtaque + tiempoEntreAtaques && !estaAtacando)
        {
            StartCoroutine(EjecutarAtaque());
        }
    }

    IEnumerator EjecutarAtaque()
    {
        estaAtacando = true;
        DetenerMovimiento();
        anim.SetTrigger("2_Attack");

        yield return new WaitForSeconds(0.1f);
        attackCollider.enabled = true;

        yield return new WaitForSeconds(0.1f);
        attackCollider.enabled = false;

        yield return new WaitForSeconds(tiempoInmovilizacionAtaque - 0.2f);
        tiempoUltimoAtaque = Time.time;
        estaAtacando = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaDeteccion);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaAtaque);
    }
    IEnumerator EsperarAntesDeAtacar()
    {
        esperandoParaAtacar = true;
        DetenerMovimiento(); // se queda quieto mientras espera

        yield return new WaitForSeconds(0.3f); // espera 1 segundo

        // Solo ataca si sigue cerca del jugador y no está atacando
        if (!estaAtacando && Vector2.Distance(transform.position, jugador.position) <= distanciaAtaque)
        {
            IntentarAtacar();
        }

        esperandoParaAtacar = false;
    }

}
