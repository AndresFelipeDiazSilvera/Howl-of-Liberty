using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerWolfController : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody2D rb;
    private PlayerInput sharedInput;
    private Vector2 inputs;

    private GameObject humanToReactivate;
    private bool isAttacking = false;
    private bool attackInputThisFrame = false;

    public Animator animator;
    public AudioClip walkWolf;
    public AudioClip bite;
    public AudioSource audioSource;
    private bool isMoving = false;
    private SpriteRenderer spriteRenderer;

    public BoxCollider2D attackCollider;
    private WolfCharacteristics wolfCharacteristics;
    private void Start()
    {
        wolfCharacteristics = GetComponent<WolfCharacteristics>();

        if (attackCollider != null)
            attackCollider.enabled = false;
    }

    void OnEnable()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        isAttacking = false;
        attackInputThisFrame = false;

        if (animator != null)
        {
            animator.SetBool("isAttacking", false);
            animator.SetFloat("speed", 0);
        }

        if (attackCollider != null)
            attackCollider.enabled = false;
    }

    public void ReceiveInput(PlayerInput inputReference)
    {
        sharedInput = inputReference;
    }

    public void StartReturnToHuman(GameObject human)
    {
        humanToReactivate = human;
        StartCoroutine(ReturnToHuman());
    }

    IEnumerator ReturnToHuman()
    {
        EnemyScript[] enemigosActivos = FindObjectsByType<EnemyScript>(FindObjectsSortMode.None);

        foreach (EnemyScript enemyScript in enemigosActivos)
        {
            if (enemyScript.enabled)
                enemyScript.BuscarJugador();
        }

        yield return new WaitForSeconds(10f);

        humanToReactivate.transform.position = transform.position;
        humanToReactivate.transform.rotation = transform.rotation;
        humanToReactivate.SetActive(true);

        PlayerController humanScript = humanToReactivate.GetComponent<PlayerController>();
        PlayerCharacteristics playerCharacteristics = humanToReactivate.GetComponent<PlayerCharacteristics>();
        WolfCharacteristics wolfCharacteristics = GetComponent<WolfCharacteristics>();

        playerCharacteristics.RecibirVida(Mathf.Clamp(wolfCharacteristics.vida, 0, playerCharacteristics.vidaMaxima));

        if (humanScript != null)
            humanScript.ResetAttackCount();

        gameObject.SetActive(false);
    }

    void Update()
    {
        if (sharedInput != null)
        {
            inputs = sharedInput.actions["Move"].ReadValue<Vector2>();
            bool currentlyMoving = inputs.magnitude > 0.01f;

            if (currentlyMoving && !isMoving && !isAttacking)
            {
                if (audioSource != null && walkWolf != null && !audioSource.isPlaying)
                {
                    audioSource.clip = walkWolf;
                    audioSource.Play();
                }
                isMoving = true;
            }
            else if ((!currentlyMoving && isMoving) || (currentlyMoving && isMoving && isAttacking))
            {
                if (audioSource != null && audioSource.isPlaying)
                {
                    audioSource.Stop();
                    audioSource.PlayOneShot(bite);
                }
                isMoving = false;
            }

            if (inputs.x > 0.01f && !isAttacking)
                transform.localScale = new Vector3(-1, 1, 1);
            else if (inputs.x < -0.01f && !isAttacking)
                transform.localScale = new Vector3(1, 1, 1);

            if (sharedInput.actions["Attack"].WasPressedThisFrame())
            {
                attackInputThisFrame = true;
            }
        }

        if (animator != null)
            animator.SetFloat("speed", inputs.magnitude);
    }

    void FixedUpdate()
    {
        if (isAttacking) return;

        rb.linearVelocity = inputs.normalized * speed;

        if (attackInputThisFrame && !isAttacking)
        {
            spriteRenderer.sortingOrder = 12;
            isAttacking = true;
            rb.linearVelocity = Vector2.zero;

            if (animator != null)
                animator.SetBool("isAttacking", true);

            if (audioSource != null)
                audioSource.PlayOneShot(bite);

            if (attackCollider != null)
                StartCoroutine(ActivarColliderTemporal());

            Invoke(nameof(EndAttack), 0.6f);
        }

        attackInputThisFrame = false;
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (isAttacking && collision.CompareTag("Enemigo"))
        {
            EnemyCharacteristics enemy = collision.GetComponentInParent<EnemyCharacteristics>();
            if (enemy != null && wolfCharacteristics != null && wolfCharacteristics.characteristics != null)
            {
                // Cura al golpear a un enemigo
                wolfCharacteristics.characteristics.Curar(20f);
            }
        }
    }

    IEnumerator ActivarColliderTemporal()
    {
        attackCollider.enabled = true;

        // ✅ CURACIÓN al atacar
        WolfCharacteristics wolfChar = GetComponent<WolfCharacteristics>();
        if (wolfChar != null && wolfChar.characteristics != null)
        {
            wolfChar.characteristics.Curar(20f);
        }

        yield return new WaitForSeconds(0.1f);
        attackCollider.enabled = false;
    }

    void EndAttack()
    {
        spriteRenderer.sortingOrder = 9;
        isAttacking = false;
        if (animator != null)
            animator.SetBool("isAttacking", false);
    }
}
