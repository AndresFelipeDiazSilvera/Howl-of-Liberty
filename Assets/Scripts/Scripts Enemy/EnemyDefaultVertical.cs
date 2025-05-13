using UnityEngine;

[RequireComponent(typeof(EnemyScript))]
public class EnemyDefaultVertical : MonoBehaviour
{
    [Header("Configuración Patrulla")]
    public float moveRange = 3f;
    public float moveSpeed = 2f;
    public float detectionRange = 5f;

    private Vector2 initialPosition;
    private bool movingUp = true;
    private EnemyScript enemyScript;
    private Transform player;
    private bool returningToPosition = false;

    void Start()
    {
        initialPosition = transform.position;
        enemyScript = GetComponent<EnemyScript>();
        enemyScript.enabled = false;

        GameObject jugadorGO = GameObject.FindGameObjectWithTag("Player");
        if (jugadorGO != null)
            player = jugadorGO.transform;
        else
            Debug.LogWarning("No se encontró un objeto con la etiqueta 'Player'");
    }

    void Update()
    {
        if (player == null) return;

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

    void PatrolMovement()
    {
        if (movingUp)
        {
            transform.Translate(Vector2.up * moveSpeed * Time.deltaTime);
            if (transform.position.y > initialPosition.y + moveRange)
            {
                movingUp = false;
            }
        }
        else
        {
            transform.Translate(Vector2.down * moveSpeed * Time.deltaTime);
            if (transform.position.y < initialPosition.y - moveRange)
            {
                movingUp = true;
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
            movingUp = initialPosition.y <= transform.position.y;

            transform.position = new Vector3(
                transform.position.x,
                initialPosition.y,
                transform.position.z
            );
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 rangeStart = Application.isPlaying ? (Vector3)initialPosition : transform.position;
        Gizmos.DrawLine(rangeStart + Vector3.down * moveRange, rangeStart + Vector3.up * moveRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        if (Application.isPlaying)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(initialPosition, Vector3.one * 0.5f);
        }
    }
}
