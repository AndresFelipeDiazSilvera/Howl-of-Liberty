using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Transform contenedorDeMapa;

    public GameObject tutorialPrefab;
    public GameObject mapa1Prefab;
    public GameObject mapa2Prefab;
    public GameObject mapa3Prefab;
    public GameObject mapa4Prefab;
    

    private GameObject mapaActual;

    void Start()
    {
        string mapa = PlayerPrefs.GetString("MapaActual", "MapaTuto");
        CargarMapa(mapa);
    }

    public void CargarMapa(string nombre)
    {
        if (mapaActual != null)
            Destroy(mapaActual);

        switch (nombre)
        {
            case "MapaTuto":
                mapaActual = Instantiate(tutorialPrefab, contenedorDeMapa);
                break;
            case "MapFrist":
                mapaActual = Instantiate(mapa1Prefab, contenedorDeMapa);
                break;
            case "MapSecond":
                mapaActual = Instantiate(mapa2Prefab, contenedorDeMapa);
                break;
            case "MapaThrid":
                mapaActual = Instantiate(mapa3Prefab, contenedorDeMapa);
                break;
            case "MapFourth":
                mapaActual = Instantiate(mapa4Prefab, contenedorDeMapa);
                break;
            default:
                Debug.LogError("Mapa no reconocido: " + nombre);
                break;
        }

        PlayerPrefs.SetString("MapaActual", nombre);
        PlayerPrefs.Save();
    }

    public void IrAlSiguienteMapa()
    {
        string actual = PlayerPrefs.GetString("MapaActual", "MapaTuto");
        string siguiente = ObtenerSiguienteMapa(actual);
        CargarMapa(siguiente);
    }

    private string ObtenerSiguienteMapa(string actual)
    {
        switch (actual)
        {
            case "MapaTuto": return "MapFrist";
            case "MapFrist": return "MapSecond";
            case "MapSecond": return "MapaThrid";
            case "MapaThrid": return "MapFourth";
            default: return "MapFourth";
        }
    }
}
