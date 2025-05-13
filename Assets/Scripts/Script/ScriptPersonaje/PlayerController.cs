using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D playerRb2D;
    public Animator animator;
    public float speed;

    public PlayerInput sharedInput;
    public GameObject wolfObject;

    public Vector2 inputs;
    public bool isAttacking = false;
    private bool attackInputThisFrame = false;
    private int attackCount = 0;

    public GameObject smokeEffectPrefab;
    public AudioClip howlSound;
    public AudioClip hitSword;
    public AudioClip walkHuman;
    public AudioSource audioSource;
    private bool isMoving = false;
    private SpriteRenderer spriteRenderer;
    private bool isHurt;
    public BoxCollider2D boxColliderArma;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerRb2D = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        isAttacking = false;
        attackInputThisFrame = false;
        animator.SetBool("isAttacking", false);

        if (playerRb2D != null)
            playerRb2D.linearVelocity = Vector2.zero;
    }

    void Update()
    {
        if (sharedInput != null)
        {
            inputs = sharedInput.actions["Move"].ReadValue<Vector2>();
            animator.SetFloat("movement", inputs.magnitude);

            bool currentlyMoving = inputs.magnitude > 0.01f;

            if (currentlyMoving && !isMoving && !isAttacking)
            {
                if (audioSource != null && walkHuman != null && !audioSource.isPlaying)
                {
                    audioSource.clip = walkHuman; // Asigna el clip a la AudioSource
                    audioSource.Play();
                }
                isMoving = true; // Marca que ahora se está moviendo
            }
            else if ((!currentlyMoving && isMoving) || (currentlyMoving && isMoving && isAttacking))
            {
                if (audioSource != null && audioSource.isPlaying)
                {
                    audioSource.Stop();
                    audioSource.PlayOneShot(hitSword);
                }
                isMoving = false; // Marca que ahora no se está moviendo
            }

            if (sharedInput.actions["Attack"].WasPressedThisFrame())
            {
                attackInputThisFrame = true;
            }

            if (inputs.x > 0.01f && !isAttacking)
                transform.localScale = new Vector3(1, 1, 1);
            else if (inputs.x < -0.01f && !isAttacking)
                transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    void FixedUpdate()
    {
        Movement();
        Attack();
        attackInputThisFrame = false;
    }

    void Attack()
    {
        if (attackInputThisFrame && !isAttacking)
        {
            isAttacking = true;
            animator.SetBool("isAttacking", true);
            playerRb2D.linearVelocity = Vector2.zero;
            spriteRenderer.sortingOrder = 12;

            audioSource.PlayOneShot(hitSword);

            attackCount++;
            if (attackCount >= 5)
            {
                StartCoroutine(TransformToWolf());
            }

            // Activar el collider 0.3 segundos después de comenzar la animación de ataque
            StartCoroutine(ActivarColliderAtaquePorTiempo(0.3f));
        }
    }

    void Movement()
    {
        if (isAttacking) return;
        if (animator.GetBool("hurt") == true) return;
        Vector2 moveVelocity = inputs.normalized * speed;
        playerRb2D.linearVelocity = moveVelocity;
        spriteRenderer.sortingOrder = 9;
    }

    // Activar el collider después de un retraso (0.3 segundos)
    IEnumerator ActivarColliderAtaquePorTiempo(float tiempo)
    {
        yield return new WaitForSeconds(tiempo);  // Espera 0.3 segundos antes de activar el collider

        // Solo activa el collider si no ha sido golpeado
        if (!isHurt)
        {
            boxColliderArma.enabled = true;
            // Desactiva el collider después de un tiempo
            StartCoroutine(DesactivarColliderAtaque());
        }
    }

    // Desactiva el collider después de un tiempo
    IEnumerator DesactivarColliderAtaque()
    {
        yield return new WaitForSeconds(0.2f);  // El tiempo durante el cual el collider está activo (0.2 segundos)
        boxColliderArma.enabled = false;
        isAttacking = false;
        animator.SetBool("isAttacking", false);
    }

    public void DesactivarAtaque()
    {
        if (isHurt) return;
        boxColliderArma.enabled = false;
        isAttacking = false;
        animator.SetBool("isAttacking", false);
    }

    public void ResetAttackCount()
    {
        attackCount = 0;
    }

    IEnumerator TransformToWolf()
    {
        if (smokeEffectPrefab != null)
            Instantiate(smokeEffectPrefab, transform.position, Quaternion.identity);

        if (audioSource != null && howlSound != null)
            audioSource.PlayOneShot(howlSound);

        yield return new WaitForSeconds(1f);

        wolfObject.transform.position = transform.position;
        wolfObject.transform.rotation = transform.rotation;
        wolfObject.SetActive(true);
        PlayerWolfController wolf = wolfObject.GetComponent<PlayerWolfController>();
        wolf.ReceiveInput(sharedInput);
        wolf.StartReturnToHuman(this.gameObject);
        WolfCharacteristics wolfCharacteristics = wolfObject.GetComponent<WolfCharacteristics>();
        PlayerCharacteristics playerCharacteristics = GetComponent<PlayerCharacteristics>();
        wolfCharacteristics.RecibirVida(playerCharacteristics.vida);
        gameObject.SetActive(false);
        yield return null;
    }
}
