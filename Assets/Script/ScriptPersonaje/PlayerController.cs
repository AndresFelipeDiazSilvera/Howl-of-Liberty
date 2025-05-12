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
    private bool isAttacking = false;
    private bool attackInputThisFrame = false;
    private int attackCount = 0;

    public GameObject smokeEffectPrefab;
    public AudioClip howlSound;
    public AudioClip hitSword;
    public AudioClip walkHuman;
    public AudioSource audioSource;
    private bool isMoving = false;

    void Start()
    {
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

            // Si el personaje comienza a moverse y no se estaba moviendo antes, inicia la reproducción
            if (currentlyMoving && !isMoving && !isAttacking)
            {
                if (audioSource != null && walkHuman != null && !audioSource.isPlaying)
                {
                    audioSource.clip = walkHuman; // Asigna el clip a la AudioSource
                    audioSource.Play();
                }
                isMoving = true; // Marca que ahora se está moviendo
            }
            // Si el personaje deja de moverse y estaba en movimiento, detiene la reproducción
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

            if (inputs.x > 0.01f)
                transform.localScale = new Vector3(1, 1, 1);
            else if (inputs.x < -0.01f)
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
            audioSource.PlayOneShot(hitSword);

            attackCount++;
            if (attackCount >= 5)
            {
                StartCoroutine(TransformToWolf());
            }
        }
    }

    void Movement()
    {
        if (isAttacking) return;
        Vector2 moveVelocity = inputs.normalized * speed;
        playerRb2D.linearVelocity = moveVelocity;

    }

    public void DesactivarAtaque()
    {
        isAttacking = false;
        animator.SetBool("isAttacking", false);
    }
    public void ResetAttackCount()
    {
        attackCount = 0;
    }

    IEnumerator TransformToWolf()
    {
        // Efecto de humo
        if (smokeEffectPrefab != null)
            Instantiate(smokeEffectPrefab, transform.position, Quaternion.identity);

        // Sonido de aullido
        if (audioSource != null && howlSound != null)
            audioSource.PlayOneShot(howlSound);
        // Esperar un segundo para que se vea el humo
        yield return new WaitForSeconds(1f);

        // Mover al Wolf a la posición actual del Human
        wolfObject.transform.position = transform.position;
        wolfObject.transform.rotation = transform.rotation;
        // Activar Wolf y pasar input
        wolfObject.SetActive(true);
        PlayerWolfController wolf = wolfObject.GetComponent<PlayerWolfController>();
        wolf.ReceiveInput(sharedInput);
        wolf.StartReturnToHuman(this.gameObject);

        // Desactivar este personaje
        gameObject.SetActive(false);
        yield return null;
    }

}