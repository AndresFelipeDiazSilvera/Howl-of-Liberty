using UnityEngine;

[RequireComponent(typeof(EnemyScript))]
public class EnemyDefaultHorizontal : MonoBehaviour
{
    [Header("Configuración Patrulla")]
    public float moveRange = 3f;
    public float moveSpeed = 2f;
    public float detectionRange = 5f;

    private Vector2 initialPosition;
    private bool movingRight = true;
    private EnemyScript enemyScript;
    private Transform player;
    private bool returningToPosition = false;

    void Start()
    {
        initialPosition = transform.position;
        enemyScript = GetComponent<EnemyScript>();
        enemyScript.enabled = false;
        FindActivePlayer(); // Buscar jugador activo al inicio
    }

    void Update()
    {
        // Verificar si el jugador actual es nulo o está desactivado
        if (player == null || !player.gameObject.activeInHierarchy)
        {
            FindActivePlayer();
            if (player == null) return; // Si no hay jugador activo, salir
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            if (!enemyScript.enabled)
            {
                enemyScript.enabled = true;
                returningToPosition = false;
            }
            return;
        }
        else
        {
            if (enemyScript.enabled)
            {
                enemyScript.enabled = false;
                returningToPosition = true;
            }
        }

        if (returningToPosition)
        {
            ReturnToInitialPosition();
        }
        else if (!enemyScript.enabled)
        {
            PatrolMovement();
        }
    }

    // Nuevo método para encontrar jugador activo
    void FindActivePlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players)
        {
            if (p.activeInHierarchy)
            {
                player = p.transform;
                return;
            }
        }
        player = null; // Si no encuentra ningún jugador activo
    }

    void PatrolMovement()
    {
        if (movingRight)
        {
            transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
            transform.localScale = new Vector3(-1f, 1f, 1f);
            if (transform.position.x > initialPosition.x + moveRange)
            {
                movingRight = false;
            }
        }
        else
        {
            transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
            transform.localScale = new Vector3(1f, 1f, 1f);
            if (transform.position.x < initialPosition.x - moveRange)
            {
                movingRight = true;
            }
        }
    }

    void ReturnToInitialPosition()
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            initialPosition,
            moveSpeed * Time.deltaTime * 1.5f
        );

        if (Vector2.Distance(transform.position, initialPosition) < 0.05f)
        {
            returningToPosition = false;
            movingRight = initialPosition.x <= transform.position.x;

            transform.position = new Vector3(
                initialPosition.x,
                transform.position.y,
                transform.position.z
            );
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Vector3 rangeStart = Application.isPlaying ? (Vector3)initialPosition : transform.position;
        Gizmos.DrawLine(rangeStart + Vector3.left * moveRange, rangeStart + Vector3.right * moveRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        if (Application.isPlaying)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(initialPosition, Vector3.one * 0.5f);
        }
    }
}