using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public Transform playerWolf;
    public float smoothSpeed = 15f;
    public Vector2 offset = new Vector2(0f, 0f);
    public LayerMask collisionMask;
    private Camera cam;
    private float cameraHalfWidth, cameraHalfHeight;

    void Start()
    {
        cam = GetComponent<Camera>();
        cameraHalfHeight = cam.orthographicSize;
        cameraHalfWidth = cameraHalfHeight * cam.aspect;
    }

    void LateUpdate()
    {
        // 👉 Elegir automáticamente al personaje activo
        Transform target = player.gameObject.activeInHierarchy ? player : playerWolf;

        Vector3 desiredPosition = target.position + (Vector3)offset;
        desiredPosition.z = -10f;

        // Bordes de la cámara para detección de colisión
        Vector2 leftEdge = new Vector2(desiredPosition.x - cameraHalfWidth, desiredPosition.y);
        Vector2 rightEdge = new Vector2(desiredPosition.x + cameraHalfWidth, desiredPosition.y);
        Vector2 topEdge = new Vector2(desiredPosition.x, desiredPosition.y + cameraHalfHeight);
        Vector2 bottomEdge = new Vector2(desiredPosition.x, desiredPosition.y - cameraHalfHeight);

        bool canMoveX = !Physics2D.OverlapCircle(leftEdge, 0.1f, collisionMask) &&
                        !Physics2D.OverlapCircle(rightEdge, 0.1f, collisionMask);

        bool canMoveY = !Physics2D.OverlapCircle(topEdge, 0.1f, collisionMask) &&
                        !Physics2D.OverlapCircle(bottomEdge, 0.1f, collisionMask);

        Vector3 currentPos = transform.position;
        float newX = canMoveX ? desiredPosition.x : currentPos.x;
        float newY = canMoveY ? desiredPosition.y : currentPos.y;

        Vector3 smoothedPosition = Vector3.Lerp(
            currentPos,
            new Vector3(newX, newY, -10f),
            smoothSpeed * Time.deltaTime
        );

        transform.position = smoothedPosition;
    }
}
