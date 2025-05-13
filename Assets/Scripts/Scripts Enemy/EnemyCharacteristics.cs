using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class EnemyCharacteristics : MonoBehaviour
{
    [Header("Configuración")]
    public Animator anim;
    public float vida = 100f;
    public float knockbackForce = 3f;
    public float immunityTime = 1.5f;
    public float hurtAnimationTime = 0.2f;

    [Header("Detección de daño")]
    public LayerMask capaArmaJugador;

    private bool isHurt = false;
    private bool isDead = false;
    private Rigidbody2D rb;
    private EnemyScript enemyScript; // Referencia al script de movimiento del enemigo
    private EnemyDefaultHorizontal enemyScript2;
    private EnemyDefaultVertical enemyScript3;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (anim == null)
            anim = GetComponentInChildren<Animator>();

        // Obtener los scripts EnemyScript
        enemyScript = GetComponent<EnemyScript>();
        enemyScript2 = GetComponent<EnemyDefaultHorizontal>();
        enemyScript3 = GetComponentInChildren<EnemyDefaultVertical>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isHurt && !isDead && IsInLayerMask(collision.gameObject, capaArmaJugador))
        {
            // Aplica daño
            vida -= 25f;

            // Verifica si el jugador es hombre lobo para curarse
            PlayerCharacteristics player = collision.gameObject.GetComponentInParent<PlayerCharacteristics>();
            if (player != null && player.esHombreLobo) // Asume que tienes una propiedad booleana IsWerewolf
            {
                player.vida += 20f;
                player.vida = Mathf.Min(player.vida, player.vidaMaxima); // Limita la vida al máximo
            }

            if (vida <= 0)
            {
                StartCoroutine(DieRoutine());
            }
            else
            {
                StartCoroutine(HurtRoutine());
                Vector2 knockbackDir = (transform.position - collision.transform.position).normalized;
                ApplyKnockback(knockbackDir);
                
            }
        }
    }

    IEnumerator HurtRoutine()
    {
        isHurt = true;

        // Desactivar el script de movimiento, comprobando si el script existe antes de deshabilitarlo
        if (enemyScript != null)
            enemyScript.enabled = false;
        if (enemyScript2 != null)
            enemyScript2.enabled = false;
        if (enemyScript3 != null)
            enemyScript3.enabled = false;

        if (anim != null)
            anim.SetBool("hurt", true);

        yield return new WaitForSeconds(hurtAnimationTime);
        if (anim != null)
            anim.SetBool("hurt", false);

        yield return new WaitForSeconds(immunityTime - hurtAnimationTime);
        if (enemyScript != null)
            enemyScript.enabled = true;
        // Habilitar los scripts de movimiento si existen
        if (enemyScript2 != null)
            enemyScript2.enabled = true;
        if (enemyScript3 != null)
            enemyScript3.enabled = true;
        




        rb.bodyType = RigidbodyType2D.Dynamic;
        isHurt = false;
    }

    IEnumerator DieRoutine()
    {
        // Detener completamente el Rigidbody
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;

        // Desactivar el script de movimiento, comprobando si el script existe antes de deshabilitarlo
        if (enemyScript != null)
            enemyScript.enabled = false;
        if (enemyScript2 != null)
            enemyScript2.enabled = false;
        if (enemyScript3 != null)
            enemyScript3.enabled = false;

        isDead = true;
        isHurt = true;

        // Activar animación de "hurt" (puede usarse para representar la muerte)
        if (anim != null)
            anim.SetBool("hurt", true);

        // Esperar antes de destruir
        yield return new WaitForSeconds(1f);
        Object.FindAnyObjectByType<FlujoDeCampaña>().IncrementarContadorEnemigos();
        Destroy(gameObject);
    }

    void ApplyKnockback(Vector2 direction)
    {
        if (rb != null)
        {
            //rb.linearVelocity = Vector2.zero; // Detener el movimiento antes de aplicar el knockback
            rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
        }
    }

    bool IsInLayerMask(GameObject obj, LayerMask mask)
    {
        return ((mask.value & (1 << obj.layer)) != 0);
    }

    void OnDisable()
    {
        StopAllCoroutines();
        isHurt = false;
        if (anim != null && !isDead)
            anim.SetBool("hurt", false);
    }
}
