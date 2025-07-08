using UnityEngine;

public class Silk : MonoBehaviour
{
    [Header("Rope Settings")]
    public int nodeCount = 20;
    public float ropeLength = 10f;
    public float gravity = 9.81f;
    public float damping = 0.99f;
    public int constraintIterations = 3;

    [Header("Player Sync Settings")]
    public float maxDistanceFromPlayer = 0.2f; 
    public float playerSyncSpeed = 1000f; 

    [Header("References")]
    public Transform player;
    public Transform wall;
    public Vector2 firstPointPos;

    [Header("Rendering")]
    //public LineRenderer lineRenderer;

    private RopeNode[] nodes;
    private float segmentLength;

    private class RopeNode
    {
        public Vector3 position;
        public Vector3 oldPosition;
        public bool isFixed;
    }

    void Start()
    {
        SetRope();
    }
    public void SetRope()
    {
        InitializeRope();
        SetupLineRenderer();
    }

    void InitializeRope()
    {
        nodes = new RopeNode[nodeCount];
        segmentLength = ropeLength / (nodeCount - 1);

        Vector3 startPos = player.position;
        Vector3 endPos = firstPointPos;

        for (int i = 0; i < nodeCount; i++)
        {
            float t = (float)i / (nodeCount - 1);
            Vector3 nodePos = Vector3.Lerp(startPos, endPos, t);

            bool isFixed = (i == nodeCount - 1);

            nodes[i] = new RopeNode();
            nodes[i].position = nodePos;
            nodes[i].oldPosition = nodePos;
            nodes[i].isFixed = isFixed;
        }
    }

    void SetupLineRenderer()
    {
        /*if (lineRenderer == null)
            lineRenderer = gameObject.AddComponent<LineRenderer>();

        lineRenderer.positionCount = nodeCount;
        lineRenderer.useWorldSpace = true;*/
    }

    void FixedUpdate()
    {
        UpdateRope();
    }

    void UpdateRope()
    {
        VerletIntegration();

        for (int i = 0; i < constraintIterations; i++)
        {
            ApplyConstraints();
        }

        // 급격한 움직임 제한
        LimitSuddenMovement();

        // 플레이어 동기화 체크
        SyncPlayerNode();

        UpdateFixedConnections();

        UpdateLineRenderer();
    }

    void VerletIntegration()
    {
        for (int i = 0; i < nodeCount; i++)
        {
            if (nodes[i].isFixed) continue;

            Vector3 velocity = (nodes[i].position - nodes[i].oldPosition) * damping;
            nodes[i].oldPosition = nodes[i].position;

            Vector3 acceleration = Vector3.down * gravity;
            nodes[i].position += velocity + acceleration * Time.fixedDeltaTime * Time.fixedDeltaTime;
        }
    }

    void ApplyConstraints()
    {
        for (int i = 0; i < nodeCount - 1; i++)
        {
            Vector3 delta = nodes[i + 1].position - nodes[i].position;
            float distance = delta.magnitude;
            float difference = segmentLength - distance;

            if (distance > 0)
            {
                Vector3 correction = delta.normalized * difference * 0.5f;

                if (!nodes[i].isFixed)
                    nodes[i].position -= correction;

                if (!nodes[i + 1].isFixed)
                    nodes[i + 1].position += correction;
            }
        }
    }

    // 급격한 움직임 제한
    void LimitSuddenMovement()
    {
        float maxMovementPerFrame = segmentLength * 0.5f; // 한 프레임당 최대 이동 거리

        for (int i = 1; i < nodeCount; i++)
        {
            if (nodes[i].isFixed) continue;

            Vector3 movement = nodes[i].position - nodes[i].oldPosition;
            float movementMagnitude = movement.magnitude;

            // 급격한 움직임이 감지되면 제한
            if (movementMagnitude > maxMovementPerFrame)
            {
                Vector3 limitedMovement = movement.normalized * maxMovementPerFrame;
                nodes[i].position = nodes[i].oldPosition + limitedMovement;
            }
        }
    }

    // 플레이어 노드 동기화
    void SyncPlayerNode()
    {
        if (player == null || nodes == null || nodes.Length == 0) return;

        // 0번째 노드와 플레이어 사이의 거리 계산
        float distanceToPlayer = Vector3.Distance(nodes[0].position, player.position);

        // 최대 허용 거리를 초과하면 플레이어 위치로 이동
        if (distanceToPlayer > maxDistanceFromPlayer)
        {
            // 부드럽게 플레이어 위치로 이동
            Vector3 targetPosition = player.position;
            nodes[0].position = Vector3.Lerp(nodes[0].position, targetPosition,
                                           Time.fixedDeltaTime * playerSyncSpeed);

            // oldPosition도 업데이트하여 velocity 계산에 영향을 줄임
            nodes[0].oldPosition = Vector3.Lerp(nodes[0].oldPosition, targetPosition,
                                              Time.fixedDeltaTime * playerSyncSpeed * 0.5f);
        }
    }

    void UpdateFixedConnections()
    {
        if (wall != null)
        {
            nodes[nodeCount - 1].position = firstPointPos;
        }
    }

    void UpdateLineRenderer()
    {
        Vector3[] positions = new Vector3[nodeCount];
        for (int i = 0; i < nodeCount; i++)
        {
            positions[i] = nodes[i].position;
        }
        //lineRenderer.SetPositions(positions);
    }

    public void ApplyForce(Vector3 force, int nodeIndex)
    {
        if (nodeIndex >= 0 && nodeIndex < nodeCount && !nodes[nodeIndex].isFixed)
        {
            nodes[nodeIndex].position += force * Time.fixedDeltaTime;
        }
    }

    public void ApplyWindForce(Vector3 windForce)
    {
        for (int i = 1; i < nodeCount - 1; i++)
        {
            nodes[i].position += windForce * Time.fixedDeltaTime;
        }
    }

    public void ApplyForceAtPosition(Vector3 worldPosition, Vector3 force, float radius = 1f)
    {
        for (int i = 1; i < nodeCount - 1; i++)
        {
            float distance = Vector3.Distance(nodes[i].position, worldPosition);
            if (distance < radius)
            {
                float influence = 1f - (distance / radius);
                nodes[i].position += force * influence * Time.fixedDeltaTime;
            }
        }
    }

    public void ApplyForceToCenter(Vector3 force)
    {
        int centerIndex = nodeCount / 2;
        ApplyForce(force, centerIndex);
    }

    public void ApplyForceToIndex(Vector3 force, int index)
    {
        ApplyForce(force, index);
    }

    public Vector2 GetNodePosition(int index)
    {
        return nodes[index].position;
    }

    public void ApplyForceToRange(Vector3 force, int startIndex, int endIndex)
    {
        for (int i = startIndex; i <= endIndex; i++)
        {
            if (i >= 0 && i < nodeCount)
            {
                ApplyForce(force, i);
            }
        }
    }

    // 플레이어와의 거리 확인용 메서드
    public float GetDistanceToPlayer()
    {
        if (player == null || nodes == null || nodes.Length == 0) return float.MaxValue;
        return Vector3.Distance(nodes[0].position, player.position);
    }

    // 플레이어 노드를 즉시 플레이어 위치로 이동
    public void ForcePlayerNodeToPlayer()
    {
        if (player != null && nodes != null && nodes.Length > 0)
        {
            nodes[0].position = player.position;
            nodes[0].oldPosition = player.position;
        }
    }

    private void OnDrawGizmos()
    {
        if (nodes == null) return;

        Gizmos.color = Color.red;
        for (int i = 0; i < nodeCount; i++)
        {
            Gizmos.DrawSphere(nodes[i].position, 0.1f);
        }

        // 플레이어와 0번째 노드 사이의 거리 시각화
        if (player != null && nodes.Length > 0)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(player.position, nodes[0].position);

            // 최대 허용 거리 범위 표시
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(player.position, maxDistanceFromPlayer);
        }
    }
}