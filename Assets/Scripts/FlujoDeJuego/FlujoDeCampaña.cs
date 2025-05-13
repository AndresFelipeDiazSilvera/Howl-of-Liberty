using UnityEngine;

public class FlujoDeCampaña : MonoBehaviour
{
    [Header("Mapas")]
    public GameObject mapaTuto, mapaUno, mapaDos, mapaTres, mapaCuatro;

    [Header("Puertas")]
    public SpriteRenderer puertaTuto, puertaUno, puertaDos, puertaTres, puertaCuatro;
    public Sprite puertaAbierta, puertaCerrada;

    [Header("Jugador")]
    public Transform jugador;
    public Vector3[] posicionesIniciales = new Vector3[5];

    [Header("Otros")]
    public FadeController fadeController;

    public int enemigosCount = 0;
    private int mapaActual = 0;
    private int[] enemigosRequeridos = new int[5] { 2, 4, 5, 5, 1 }; // Mapa 4 no requiere enemigos

    void Start()
    {
        fadeController = GetComponent<FadeController>();

        DesactivarTodosLosMapas();
        mapaTuto.SetActive(true);
        TeleportarJugador(0);
    }

    void DesactivarTodosLosMapas()
    {
        mapaTuto.SetActive(false);
        mapaUno.SetActive(false);
        mapaDos.SetActive(false);
        mapaTres.SetActive(false);
        mapaCuatro.SetActive(false);
    }

    public void IncrementarContadorEnemigos()
    {
        enemigosCount++;
        VerificarProgresoMapa();
    }

    public void VerificarProgresoMapa()
    {
        if (enemigosCount >= enemigosRequeridos[mapaActual])
        {
            AbrirPuerta(GetPuertaPorMapa(mapaActual));
        }
    }

    public void ProcesarToquePuerta(int puerta, Rigidbody2D rb)
    {
        if (puerta < 0 || puerta >= enemigosRequeridos.Length) return;

        if (enemigosCount >= enemigosRequeridos[puerta])
        {
            CambiarMapa(puerta + 1); // Pasa al siguiente mapa
            rb.simulated = true;
        }
        else
        {
            Debug.Log($"Faltan enemigos por derrotar en el mapa {puerta}");
        }
    }

    public void CambiarMapa(int nuevoMapa)
    {
        if (nuevoMapa < 0 || nuevoMapa >= posicionesIniciales.Length)
        {
            Debug.LogError("Índice de mapa fuera de rango: " + nuevoMapa);
            return;
        }

        fadeController.StartFadeOutThenIn(() =>
        {
            DesactivarTodosLosMapas();
            DestruirEnemigosExistentes();

            // Activar mapa correspondiente
            switch (nuevoMapa)
            {
                case 0: mapaTuto.SetActive(true); break;
                case 1: mapaUno.SetActive(true); break;
                case 2: mapaDos.SetActive(true); break;
                case 3: mapaTres.SetActive(true); break;
                case 4: mapaCuatro.SetActive(true); break;
            }

            // Teletransportar jugador a la posición inicial correspondiente
            TeleportarJugador(nuevoMapa);

            enemigosCount = 0;
            mapaActual = nuevoMapa;

            if (nuevoMapa > 0)
                CerrarPuerta(GetPuertaPorMapa(nuevoMapa - 1));
        });
    }

    private void TeleportarJugador(int indice)
    {
        if (jugador != null && indice >= 0 && indice < posicionesIniciales.Length)
        {
            jugador.position = posicionesIniciales[indice];
            jugador.gameObject.SetActive(false); // Desactivar el jugador temporalmente
            jugador.gameObject.SetActive(true);  // Reactivar el jugador para actualizar la posición en el Editor
            Debug.Log($"Jugador teletransportado a mapa {indice} en posición {posicionesIniciales[indice]}");
        }
        else
        {
            Debug.LogWarning($"Posición inicial no válida para índice {indice}");
        }
    }

    private void DestruirEnemigosExistentes()
    {
        foreach (var enemigo in GameObject.FindGameObjectsWithTag("Enemigo"))
        {
            Destroy(enemigo);
        }
    }

    public int GetMapaActual()
    {
        return mapaActual;
    }

    private SpriteRenderer GetPuertaPorMapa(int mapa)
    {
        switch (mapa)
        {
            case 0: return puertaTuto;
            case 1: return puertaUno;
            case 2: return puertaDos;
            case 3: return puertaTres;
            case 4: return puertaCuatro;
            default: return null;
        }
    }

    public void AbrirPuerta(SpriteRenderer puerta)
    {
        if (puerta != null)
            puerta.sprite = puertaAbierta;
        BoxCollider2D bcollider = puerta.gameObject.GetComponent<BoxCollider2D>();
        bcollider.enabled = true;
    }

    public void CerrarPuerta(SpriteRenderer puerta)
    {
        if (puerta != null)
            puerta.sprite = puertaCerrada;
    }
}
