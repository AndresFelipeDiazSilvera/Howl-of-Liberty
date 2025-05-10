using UnityEngine;

public class HuesoOrbita : MonoBehaviour
{
    [Header("Configuración de Órbita")]
    public Transform target;
    public float orbitRadius = 2.5f;
    public float orbitSpeed = 180f;
    public bool clockwise = true;
    public bool rotateOnItsOwnAxis = false;
    public float selfRotationSpeed = 90f;
    public float initialAngle = 0f;
    public GameObject huesoHijo;
    public int stateFlechas = 0;

    private void Start()
    {
        huesoHijo.SetActive(false);
        if (target == null)
        {
            Debug.LogError("¡No hay objetivo asignado!");
            enabled = false;
            return;
        }
        UpdateOrbitPositions(initialAngle);
    }

    private void Update()
    {
        switch (stateFlechas)
        {
            case 1:
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

    private void UpdateOrbitPositions(float angle)
    {
        if (target == null) return;

        // Posición para el hueso padre
        float angleRad = angle * Mathf.Deg2Rad;
        Vector2 offset = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad)) * orbitRadius;
        transform.position = new Vector3(target.position.x + offset.x, target.position.y + 0.5f + offset.y, target.position.z);

        // Posición para el hueso hijo (180° opuesto)
        if (huesoHijo.activeSelf)
        {
            float oppositeAngleRad = (angle + 180f) * Mathf.Deg2Rad;
            Vector2 oppositeOffset = new Vector2(Mathf.Cos(oppositeAngleRad), Mathf.Sin(oppositeAngleRad)) * orbitRadius;
            huesoHijo.transform.position = new Vector3(target.position.x + oppositeOffset.x, target.position.y + 0.5f + oppositeOffset.y, target.position.z);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (target != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(new Vector3(target.position.x, target.position.y + 0.5f, target.position.z), orbitRadius);
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