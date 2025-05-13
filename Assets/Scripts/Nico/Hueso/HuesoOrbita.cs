using UnityEngine;

public class HuesoOrbita : MonoBehaviour
{
    [Header("Configuración de Órbita")]
    private Transform target;
    public float orbitRadius = 2.5f;
    public float orbitSpeed = 180f;
    public bool clockwise = true;
    public bool rotateOnItsOwnAxis = false;
    public float selfRotationSpeed = 90f;
    public float initialAngle = 0f;
    public GameObject huesoHijo;
    public int stateFlechas = 0;
    private BoxCollider2D boxCol;

    private void Start()
    {
        boxCol = GetComponent<BoxCollider2D>();
        huesoHijo.SetActive(false);
        FindActivePlayer(); // Busca al jugador al inicio
        UpdateOrbitPositions(initialAngle);
    }

    private void Update()
    {
        // Verifica constantemente si el player actual es válido
        if (target == null || !target.gameObject.activeInHierarchy)
        {
            FindActivePlayer();
            if (target == null) return; // Si no hay player activo, no hacer nada
        }

        // Resto de tu lógica Update...
        switch (stateFlechas)
        {
            case 0:
                orbitSpeed = 0f;
                selfRotationSpeed = 0f;
                this.GetComponent<SpriteRenderer>().enabled = false;
                boxCol.enabled = false;
                huesoHijo.SetActive(false);
                break;
            case 1:
                this.GetComponent<SpriteRenderer>().enabled = true;
                boxCol.enabled = true;
                orbitSpeed = 180f;
                selfRotationSpeed = 90f;
                huesoHijo.SetActive(false);
                break;
            case 2:
                orbitSpeed = 270f;
                selfRotationSpeed = 135f;
                huesoHijo.SetActive(false);
                break;
            case 3:
                orbitSpeed = 270f;
                selfRotationSpeed = 135f;
                huesoHijo.SetActive(true);
                break;
        }

        float direction = clockwise ? -1f : 1f;
        float deltaAngle = direction * orbitSpeed * Time.deltaTime;
        initialAngle += deltaAngle;

        UpdateOrbitPositions(initialAngle);

        if (rotateOnItsOwnAxis)
        {
            transform.Rotate(0, 0, selfRotationSpeed * Time.deltaTime);
            if (huesoHijo.activeInHierarchy)
            {
                huesoHijo.transform.rotation = transform.rotation;
            }
        }
    }

    // Nuevo método para encontrar al player activo
    private void FindActivePlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (player.activeInHierarchy)
            {
                target = player.transform;
                return;
            }
        }
        target = null; // Si no encuentra ningún player activo
    }

    private void UpdateOrbitPositions(float angle)
    {
        if (target == null) return;

        // Añadido offset en Y para mejor visualización
        Vector3 targetPosition = target.position - new Vector3(0, 0.5f, 0);

        // Posición para el hueso padre
        float angleRad = angle * Mathf.Deg2Rad;
        Vector2 offset = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad)) * orbitRadius;
        transform.position = targetPosition + new Vector3(offset.x, offset.y, 0);

        // Posición para el hueso hijo (180° opuesto)
        if (huesoHijo.activeSelf)
        {
            float oppositeAngleRad = (angle + 180f) * Mathf.Deg2Rad;
            Vector2 oppositeOffset = new Vector2(Mathf.Cos(oppositeAngleRad), Mathf.Sin(oppositeAngleRad)) * orbitRadius;
            huesoHijo.transform.position = targetPosition + new Vector3(oppositeOffset.x, oppositeOffset.y, 0);
        }
    }

    // Resto de tus métodos...
    private void OnDrawGizmosSelected()
    {
        if (target != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(target.position - new Vector3(0, 0.5f, 0), orbitRadius);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemigo"))
        {
            Debug.Log("Golpe");
        }
    }
}