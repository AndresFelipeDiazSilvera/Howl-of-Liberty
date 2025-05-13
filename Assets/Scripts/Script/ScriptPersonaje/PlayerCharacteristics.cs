using UnityEngine;
using System.Collections;

public class PlayerCharacteristics : MonoBehaviour
{
    public float vida = 100f;
    public float vidaMaxima = 100f;
    public bool esHombreLobo;
    public float knockbackForce = 3f;
    public float hurtAnimationTime = 0.5f;

    private Animator anim;
    private Rigidbody2D rb;
    private bool isDefense;

    void Start()
    {
        vida = Mathf.Clamp(vida, 0, vidaMaxima);
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        esHombreLobo = true;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isDefense && collision.CompareTag("Enemigo"))
        {
            CircleCollider2D enemy = collision.GetComponent<CircleCollider2D>();
            if (enemy != null && enemy.enabled)
            {
                StartCoroutine(TakeHit(enemy.transform));
            }
        }
    }

    public void Curar(float cantidad)
    {
        vida = Mathf.Min(vida + cantidad, vidaMaxima);
    }

    public void RecibirVida(float nuevaVida)
    {
        vida = Mathf.Clamp(nuevaVida, 0, vidaMaxima);
    }

    IEnumerator TakeHit(Transform enemy)
    {
        isDefense = true;
        vida -= 25f;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce((transform.position - enemy.position).normalized * knockbackForce, ForceMode2D.Impulse);

        anim.SetBool("hurt", true);
        yield return new WaitForSeconds(hurtAnimationTime);
        anim.SetBool("hurt", false);

        isDefense = false;
    }

    void OnDisable()
    {
        if (anim) anim.SetBool("hurt", false);
        esHombreLobo = false;
    }
}
