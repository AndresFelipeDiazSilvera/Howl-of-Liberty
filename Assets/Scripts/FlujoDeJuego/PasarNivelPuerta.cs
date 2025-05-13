using UnityEngine;

public class PasarNivelPuerta : MonoBehaviour
{
    [Header("Referencias")]
    public FlujoDeCampaña flujoDeCampaña;
    private GameObject canvasVictoria;
    private void Start()
    {
        canvasVictoria = GameObject.Find("CanvasVictoria");
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        int mapaActual = flujoDeCampaña.GetMapaActual();

        if (mapaActual >= 0 && mapaActual <= 4)
        {
            
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            rb.simulated = false;
            flujoDeCampaña.ProcesarToquePuerta(mapaActual, rb);
        }
        else if (mapaActual ==5){
            canvasVictoria.SetActive(true);
            Time.timeScale = 0;
        }
        else
        {
            Debug.LogWarning("Índice de mapa actual fuera de rango: " + mapaActual);
        }
    }
}
