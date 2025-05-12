using UnityEngine;

public class BossScript : MonoBehaviour
{
    // Referencias
    private Transform playerTransform;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    
    // Parámetros de movimiento
    [Header("Movimiento")]
    public float moveSpeed = 2.5f;
    public float attackRange = 2.0f;
    
    // Parámetros de ataque
    [Header("Ataque")]
    public float attackCooldown = 2.0f;
    private float attackTimer;
    private bool isAttacking = false;
    
    // Constantes para animación
    private const string ANIM_ATTACK = "Attack boss";
    
    void Start()
    {
        // Buscar al jugador por tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("No se encontró objeto con tag 'Player'");
        }
        
        // Obtener componentes necesarios
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Inicializar timer de ataque
        attackTimer = 0f;
    }
    
    void Update()
    {
        if (playerTransform == null) return;
        
        // Reducir timer de ataque
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }
        
        // Calcular distancia al jugador
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        
        // Comprobar si está en rango de ataque
        if (distanceToPlayer <= attackRange)
        {
            // Si no está atacando y el cooldown ha terminado, atacar
            if (!isAttacking && attackTimer <= 0)
            {
                Attack();
            }
        }
        else
        {
            // Mover hacia el jugador
            MoveTowardsPlayer();
        }
    }
    
    void MoveTowardsPlayer()
    {
        // Solo mover si no está atacando
        if (isAttacking) return;
        
        // Obtener dirección hacia el jugador
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        
        // Mover el boss
        transform.position = Vector2.MoveTowards(
            transform.position,
            playerTransform.position,
            moveSpeed * Time.deltaTime
        );
        
        // Voltear sprite según dirección
        FlipSprite(direction.x);
    }
    
    void Attack()
    {
        isAttacking = true;
        attackTimer = attackCooldown;
        
        // Reproducir animación de ataque
        animator.Play(ANIM_ATTACK);
        
        // Llamar al método que finalizará el ataque
        Invoke("FinishAttack", GetAnimationLength(ANIM_ATTACK));
        
        // Aquí podrías añadir lógica adicional de ataque, como daño al jugador
    }
    
    void FinishAttack()
    {
        isAttacking = false;
    }
    
    void FlipSprite(float directionX)
    {
        // Si se mueve a la izquierda, voltear sprite
        if (directionX < 0)
        {
            spriteRenderer.flipX = true;
        }
        // Si se mueve a la derecha, no voltear
        else if (directionX > 0)
        {
            spriteRenderer.flipX = false;
        }
    }
    
    // Método auxiliar para obtener la duración de una animación
    float GetAnimationLength(string animName)
    {
        // Buscar la animación por nombre
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            if (clip.name == animName)
            {
                return clip.length;
            }
        }
        
        // Si no se encuentra, devolver un valor predeterminado
        Debug.LogWarning("No se encontró la animación: " + animName);
        return 1.0f; // Valor predeterminado de 1 segundo
    }
    
    // Método para visualizar el rango de ataque en el editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
