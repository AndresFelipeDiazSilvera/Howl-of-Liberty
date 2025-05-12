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



    void OnEnable()
    {
        rb = GetComponent<Rigidbody2D>();
        isAttacking = false;
        attackInputThisFrame = false;

        if (animator != null)
        {
            animator.SetBool("isAttacking", false);
            animator.SetFloat("speed", 0);
        }
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

        yield return new WaitForSeconds(10f);

        // Mover al Human a la posición actual del Wolf
        humanToReactivate.transform.position = transform.position;
        humanToReactivate.transform.rotation = transform.rotation;

        humanToReactivate.SetActive(true);

        PlayerController humanScript = humanToReactivate.GetComponent<PlayerController>();
        if (humanScript != null)
        {
            humanScript.ResetAttackCount();
        }

        gameObject.SetActive(false);
    }

    void Update()
    {
        if (sharedInput != null)
        {
            inputs = sharedInput.actions["Move"].ReadValue<Vector2>();

            bool currentlyMoving = inputs.magnitude > 0.01f;

            // Si el personaje comienza a moverse y no se estaba moviendo antes, inicia la reproducción
            if (currentlyMoving && !isMoving && !isAttacking)
            {
                if (audioSource != null && walkWolf != null && !audioSource.isPlaying)
                {
                    audioSource.clip = walkWolf; // Asigna el clip a la AudioSource
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
                    audioSource.PlayOneShot(bite);
                }
                isMoving = false; // Marca que ahora no se está moviendo
            }

            if (inputs.x > 0.01f)
                transform.localScale = new Vector3(-1, 1, 1);
            else if (inputs.x < -0.01f)
                transform.localScale = new Vector3(1, 1, 1);

            if (sharedInput.actions["Attack"].WasPressedThisFrame())
            {
                attackInputThisFrame = true;
            }
        }

        // Animación de movimiento
        if (animator != null)
        {
            animator.SetFloat("speed", inputs.magnitude);
        }
    }

    void FixedUpdate()
    {
        if (isAttacking) return;

        rb.linearVelocity = inputs.normalized * speed;

        if (attackInputThisFrame && !isAttacking)
        {
            isAttacking = true;
            rb.linearVelocity = Vector2.zero;

            if (animator != null)
                animator.SetBool("isAttacking", true);
                audioSource.PlayOneShot(bite);

            // Puedes llamar una animación evento para desactivar después
            Invoke(nameof(EndAttack), 0.6f); // Asumiendo que la animación dura 0.6 segundos
        }

        attackInputThisFrame = false;
    }

    void EndAttack()
    {
        isAttacking = false;
        if (animator != null)
            animator.SetBool("isAttacking", false);
    }
}