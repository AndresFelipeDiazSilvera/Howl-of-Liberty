using UnityEngine;
using System.Collections;

public class WolfCharacteristics : MonoBehaviour
{
    public float vida;
    public float knockbackForce = 3f;
    public float hurtAnimationTime = 0.5f;

    private Animator anim;
    private Rigidbody2D rb;
    private bool isDefense;
    public PlayerCharacteristics characteristics;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void RecibirVida(float vidaX)
    {
        vida = vidaX;
    }

    IEnumerator TakeHit(Transform enemy)
    {
        isDefense = true;
        characteristics.vida -= 25f;

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
    }
}
