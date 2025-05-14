using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject selectorDeMapasPrefab;
    public GameObject guiaPrefab;
    public Transform canvasPadre; // El canvas donde aparecerá el UI
    public GameObject menuPrincipal;
    public GameObject titulo;         // El título del juego (texto o imagen)
    public GameObject square;         // El objeto decorativo "Square"
    private GameObject instanciaSelector;
    private GameObject instanciaGuia;
    public void NuevoJuego()
    {


        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetString("MapaActual", "MapaTuto");
        PlayerPrefs.SetInt("Progreso", 1);
        SceneManager.LoadScene("PruebaEscenario"); // Aquí es donde tienes el GameManager

    }

    public void Continuar()
    {
        if (instanciaSelector == null)
        {
            instanciaSelector = Instantiate(selectorDeMapasPrefab, canvasPadre);
        }
        else
        {
            instanciaSelector.SetActive(true); // Lo reactivamos si ya existe
        }

        OcultarElementosMenu();
    }

    public void Opciones()
    {
        if (instanciaGuia == null)
        {
            instanciaGuia = Instantiate(guiaPrefab, canvasPadre);
        }
        else
        {
            instanciaGuia.SetActive(true);
        }

        OcultarElementosMenu();
    }

    public void Salir()
    {
        Application.Quit();
        Debug.Log("Estas saliendo del Juego...");
    }

    private void OcultarElementosMenu()
    {
        menuPrincipal.SetActive(false);

        if (titulo != null)
            titulo.SetActive(false);

        if (square != null)
            square.SetActive(false);
    }
    public void MostrarMenu()
    {
        menuPrincipal.SetActive(true);

        if (titulo != null)
            titulo.SetActive(true);

        if (square != null)
            square.SetActive(true);

        // NO destruir la instancia, solo ocultarla
        if (instanciaSelector != null)
        {
            instanciaSelector.SetActive(false);
            // No pongas instanciaSelector = null;
        }
        if (instanciaGuia != null)
        {
            instanciaGuia.SetActive(false);
            // No pongas instanciaSelector = null;
        }
    }
}