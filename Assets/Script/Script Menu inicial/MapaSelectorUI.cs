using UnityEngine;
using UnityEngine.UI;

public class MapaSelectorUI : MonoBehaviour
{
    public Button botonTutorial;
    public Button botonMapa1;
    public Button botonMapa2;
    public Button botonMapa3;
    public Button botonBoss;
    private MenuManager menuManager;

    void Start()
    {
        int progreso = PlayerPrefs.GetInt("Progreso", 1);

        botonTutorial.interactable = progreso >= 1;
        botonMapa1.interactable = progreso >= 2;
        botonMapa2.interactable = progreso >= 3;
        botonMapa3.interactable = progreso >= 4;
        botonBoss.interactable = progreso >= 5;

        // Buscar el MenuManager en la escena
        menuManager = FindAnyObjectByType<MenuManager>();
    }

    public void SeleccionarMapa(string nombreMapa)
    {
        PlayerPrefs.SetString("MapaActual", nombreMapa);
        gameObject.SetActive(false); // Oculta el selector
    }

    public void Cerrar()
    {
        gameObject.SetActive(false); // Solo se oculta el selector

        if (menuManager != null)
        {
            menuManager.MostrarMenu();
        }
    }
}