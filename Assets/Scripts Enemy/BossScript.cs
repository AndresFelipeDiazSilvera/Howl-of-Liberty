using UnityEngine;

public class BossScript : MonoBehaviour
{
  // Referencias
    private Transform playerTransform;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    
    // Componentes de audio
    private AudioSource audioSource;
    
    // Clips de audio
    [Header("Sonidos")]
    public AudioClip attackSound;
    public AudioClip footstepSound;
    public float footstepInterval = 0.5f;  // Intervalo entre sonidos de pasos
    private float footstepTimer = 0f;
    
    // Rangos de audio
    [Header("Rangos de Audio")]
    public float footstepAudibleRange = 5.0f;  // Distancia a la que se escuchan los pasos
    
    // Parámetros de movimiento
    [Header("Movimiento")]
    public float moveSpeed = 2.5f;
    public float detectionRange = 8.0f;  // Rango para detectar al jugador
    public float attackRange = 2.0f;     // Rango para atacar
    
    // Parámetros de ataque
    [Header("Ataque")]
    public float attackCooldown = 2.0f;
    private float attackTimer;
    private bool isAttacking = false;
    
    // Constantes para animación
    private const string ANIM_ATTACK = "Attack boss";
    private const string ANIM_IDLE = "espada efecto";
    
    // Estado del jefe
    private enum BossState { Idle, Chase, Attack }
    private BossState currentState = BossState.Idle;
    private BossState previousState = BossState.Idle;
    
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
        
        // Configurar componente de audio
        SetupAudio();
        
        // Inicializar timer de ataque
        attackTimer = 0f;
        
        // Iniciar con la animación idle
        PlayIdleAnimation();
    }
    
    void SetupAudio()
    {
        // Obtener o agregar componente AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            
            // Configuración básica del AudioSource
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1.0f;  // Sonido completamente 3D
            audioSource.minDistance = 1.0f;
            audioSource.maxDistance = 20.0f;
            audioSource.rolloffMode = AudioRolloffMode.Linear;
        }
        
        // Verificar si se asignaron los clips de audio
        if (attackSound == null)
        {
            Debug.LogWarning("¡No se ha asignado un sonido de ataque!");
        }
        
        if (footstepSound == null)
        {
            Debug.LogWarning("¡No se ha asignado un sonido de pasos!");
        }
    }
    
    void Update()
    {
        if (playerTransform == null) return;
        
        // Guardar el estado anterior
        previousState = currentState;
        
        // Reducir timer de ataque
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }
        
        // Calcular distancia al jugador
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        
        // Actualizar estado según la distancia
        UpdateBossState(distanceToPlayer);
        
        // Ejecutar comportamiento según el estado actual
        switch (currentState)
        {
            case BossState.Idle:
                // En idle solo se queda quieto con su animación
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName(ANIM_IDLE))
                {
                    PlayIdleAnimation();
                }
                break;
                
            case BossState.Chase:
                // Perseguir al jugador
                MoveTowardsPlayer();
                
                // Gestionar sonidos de pasos solo si está suficientemente cerca
                if (distanceToPlayer <= footstepAudibleRange)
                {
                    ManageFootstepSounds();
                }
                break;
                
            case BossState.Attack:
                // Atacar si puede
                if (!isAttacking && attackTimer <= 0)
                {
                    Attack();
                }
                // Orientarse hacia el jugador durante el ataque
                FlipTowardsPlayer();
                break;
        }
    }
    
    void ManageFootstepSounds()
    {
        // Reproducir sonido de pasos a intervalos regulares mientras se mueve
        if (footstepSound != null)
        {
            footstepTimer -= Time.deltaTime;
            
            if (footstepTimer <= 0)
            {
                // Calcular volumen basado en la distancia (opcional)
                float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
                float volumeMultiplier = Mathf.Clamp01(1.0f - (distanceToPlayer / footstepAudibleRange));
                float finalVolume = 0.6f * volumeMultiplier;
                
                PlaySound(footstepSound, finalVolume);
                footstepTimer = footstepInterval;
            }
        }
    }
    
    void PlaySound(AudioClip clip, float volume = 1.0f)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip, volume);
        }
    }
    
    void UpdateBossState(float distanceToPlayer)
    {
        // Si está atacando, mantener ese estado hasta que termine
        if (isAttacking) return;
        
        // Cambiar el estado según la distancia
        if (distanceToPlayer <= attackRange)
        {
            // En rango de ataque
            currentState = BossState.Attack;
        }
        else if (distanceToPlayer <= detectionRange)
        {
            // En rango de detección pero no de ataque
            currentState = BossState.Chase;
        }
        else
        {
            // Fuera de rango de detección
            currentState = BossState.Idle;
        }
    }
    
    void MoveTowardsPlayer()
    {
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
        
        // Reproducir sonido de ataque
        PlaySound(attackSound, 1.0f);
        
        // Llamar al método que finalizará el ataque
        Invoke("FinishAttack", GetAnimationLength(ANIM_ATTACK));
        
        // Aquí podrías añadir lógica adicional de ataque, como daño al jugador
    }
    
    void FinishAttack()
    {
        isAttacking = false;
    }
    
    void PlayIdleAnimation()
    {
        animator = GetComponent<Animator>(); // Asegurarse de que el animator no sea nulo
        if (animator != null) 
        {
            animator.Play(ANIM_IDLE);
        }
    }
    
    void FlipTowardsPlayer()
    {
        if (playerTransform != null)
        {
            // Determinar si el jugador está a la izquierda o a la derecha
            bool isPlayerToLeft = playerTransform.position.x < transform.position.x;
            spriteRenderer.flipX = isPlayerToLeft;
        }
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
    
    // Método para visualizar los rangos en el editor
    private void OnDrawGizmosSelected()
    {
        // Rango de ataque (rojo)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        // Rango de detección (amarillo)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // Rango de audio para pasos (azul)
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, footstepAudibleRange);
    }
}
