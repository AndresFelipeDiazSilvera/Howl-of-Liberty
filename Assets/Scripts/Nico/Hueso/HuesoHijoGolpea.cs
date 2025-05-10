using UnityEngine;

public class HuesoHijoGolpea : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemigo"))
        {
            Debug.Log("Hijo Golpea");
        }
    }
}
