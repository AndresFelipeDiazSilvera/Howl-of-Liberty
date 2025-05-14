using UnityEngine;
using UnityEngine.UI;

public class ControllerOption : MonoBehaviour
{
    private MenuManager menuManager;

    void Start()
    {

        // Buscar el MenuManager en la escena
        menuManager = FindAnyObjectByType<MenuManager>();
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
