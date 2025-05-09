using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Componente Rigidbody 2D
    Rigidbody2D playerRb2D;
    public Animator animator;

    // Velocidad de movimiento del jugador
    public float speed;

    // Inputs del jugador (sistema de input de Unity)
    PlayerInput playerInput;

    // Valores del input (x: izquierda/derecha, y: arriba/abajo)
    public Vector2 inputs;
    private bool isAttacking = false;
    private bool attackInputThisFrame = false; // Variable para almacenar si se presionó el ataque en el Update

    void Start()
    {
        // Obtenemos el Rigidbody 2D y el sistema de input al iniciar
        playerRb2D = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
    }

    void Update()
    {
        // Leer los inputs del jugador desde el sistema de Input
        inputs = playerInput.actions["Move"].ReadValue<Vector2>();
        animator.SetFloat("movement", inputs.magnitude);

        // Detectar la entrada de ataque en Update y guardar el estado
        if (playerInput.actions["Attack"].WasPressedThisFrame())
        {
            attackInputThisFrame = true;
        }

        // Mirar a la dirección del movimiento
        if (inputs.x > 0.01f)
            transform.localScale = new Vector3(1, 1, 1);
        else if (inputs.x < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    void FixedUpdate()
    {
        // Mover al jugador según el input (solo si no está atacando)
        Movement();
        // Llamar a Attack en FixedUpdate
        Attack();
        // Resetear la variable de entrada después de procesarla en FixedUpdate
        attackInputThisFrame = false;
    }

    void Attack()
    {
        // Usar la variable guardada del Update para activar el ataque
        if (attackInputThisFrame && !isAttacking)
        {
            isAttacking = true;
            animator.SetBool("isAttacking", true);
            playerRb2D.linearVelocity = Vector2.zero; // Detener el movimiento al atacar
        }
    }

    void Movement()
    {
        if (isAttacking)
        {
            return;
        }
        // Crear un vector de movimiento 2D a partir del input
        Vector2 moveVelocity = inputs.normalized * speed;

        // Mover el Rigidbody 2D
        playerRb2D.linearVelocity = moveVelocity;
    }

    public void DesactivarAtaque()
    {
        isAttacking = false;
        animator.SetBool("isAttacking", false);
    }
}