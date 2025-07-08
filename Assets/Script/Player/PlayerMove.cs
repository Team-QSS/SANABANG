/*using System;
using UnityEngine;
public class PlayerMove : HalfSingleMono<PlayerMove>
{
    public event Action<PlayerBehaviors> animationJumpState;
    public event Action<string> setAni;
    public event Func<bool> isPlayerDown;
    public event Func<PlayerBehaviors, bool> comparePlayerBehavor;
    public event Func<float> getPlayerAngle;

    public float moveSpeed;
    public float jumpPower;
    public float gavityScale;
    private float _moveDir;
    private float _velY;
    [SerializeField] private Rigidbody2D rb2D;
    [SerializeField] private float rotateSpeed = 2f;
    private float radius = 0f;
    private float currentAngle = 0f;
    private Vector2 hookPoint;

    private bool wasSwinging = false;
    private Vector2 swingVelocity;
    private float releaseGravityScale = 0f;
    private float gravityTransitionSpeed = 20f;
    private float releaseBoostMultiplier = 3f;

    public float VelY
    {
        get => _velY;
        set
        {
            if (!PlayerInput.Instance.IsGrounded())
            {
                if (value > _velY)
                {
                    animationJumpState?.Invoke(PlayerBehaviors.Jump);
                }
                else if (value < _velY)
                {
                    animationJumpState?.Invoke(PlayerBehaviors.Land);
                }
            }
            else
            {
                setAni?.Invoke("Return");
            }
            _velY = value;
        }
    }
    void Start()
    {
        getPlayerAngle += SilkThrower.Instance.GetRotation;
        setAni += PlayerBehave.Instance.SetAnimationTrigger;
        comparePlayerBehavor += PlayerBehave.Instance.ComparePlayerBehave;
        animationJumpState += PlayerBehave.Instance.SetPlayerBehave;
        isPlayerDown += SilkThrower.Instance.IsPlayerDown;
    }
    private void FixedUpdate()
    {
        bool isSwinging = PlayerBehave.Instance.CheckIsSilking();

        if (isSwinging)
        {
            HandleSwinging();
            wasSwinging = true;
        }
        else
        {
            if (wasSwinging)
            {
                HandleSwingRelease();
                wasSwinging = false;
            }
            else
            {
                HandleNormalMovement();
            }
        }
    }

    private void HandleSwinging()
    {
        rb2D.gravityScale = 0;
        hookPoint = SilkThrower.Instance.GetHookPoint();
        Vector2 dir = (Vector2)transform.position - hookPoint;
        radius = dir.magnitude;
        currentAngle = Mathf.Atan2(dir.y, dir.x);
        float input = _moveDir;

        Vector2 previousPos = transform.position;
        gameObject.transform.rotation = Quaternion.Euler(0, 0, (float)getPlayerAngle?.Invoke());
        if (Mathf.Abs(input) > 0.04f)
        {
            currentAngle += input * rotateSpeed * Time.fixedDeltaTime;
            if (!(bool)isPlayerDown?.Invoke())
            {
                if (rb2D.gravityScale <= 0)
                {
                    rb2D.gravityScale = 10f;
                }
            }
            else
            {
                if (rb2D.gravityScale > 0)
                {
                    rb2D.gravityScale = 0;
                }
            }
        }
        else
        {
            if (!PlayerInput.Instance.IsGrounded())
            {
                float gravityRotateSpeed = rotateSpeed * 0.8f;
                float targetAngle = -Mathf.PI / 2f;
                float angleDiff = Mathf.DeltaAngle(currentAngle * Mathf.Rad2Deg, targetAngle * Mathf.Rad2Deg) * Mathf.Deg2Rad;
                if (Mathf.Abs(angleDiff) > 0.1f)
                {
                    float rotateDirection = Mathf.Sign(angleDiff);
                    currentAngle += rotateDirection * gravityRotateSpeed * Time.fixedDeltaTime;
                }
            }
        }

        Vector2 offset = new Vector2(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle)) * radius;
        Vector2 targetPos = hookPoint + offset;
        rb2D.MovePosition(targetPos);


        swingVelocity = ((Vector2)transform.position - previousPos) / Time.fixedDeltaTime;
    }

    private void HandleSwingRelease()
    {
        Vector2 boostedVelocity = swingVelocity * releaseBoostMultiplier;
        rb2D.linearVelocity = boostedVelocity;

        gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);

        releaseGravityScale = 0f;
        rb2D.gravityScale = releaseGravityScale;

        VelY = rb2D.linearVelocity.y;
    }

    private void HandleNormalMovement()
    {
        if (releaseGravityScale < gavityScale)
        {
            releaseGravityScale = Mathf.MoveTowards(releaseGravityScale, gavityScale, gravityTransitionSpeed * Time.fixedDeltaTime);
            rb2D.gravityScale = releaseGravityScale;
        }
        else
        {
            rb2D.gravityScale = gavityScale;
        }

        rb2D.linearVelocity = new Vector2(_moveDir * moveSpeed, rb2D.linearVelocity.y);
        VelY = rb2D.linearVelocity.y;
    }

    public void SetVelocityX(float dir)
    {
        _moveDir = dir;
    }
    public void Jump()
    {
        Vector2 velocity = rb2D.linearVelocity;
        velocity.y = jumpPower;
        rb2D.linearVelocity = velocity;
    }
}*/
using System;
using UnityEngine;

public class PlayerMove : HalfSingleMono<PlayerMove>
{
    public event Action<PlayerBehaviors> animationJumpState;
    public event Action<string> setAni;
    public event Func<bool> isPlayerDown;
    public event Func<PlayerBehaviors, bool> comparePlayerBehavor;
    public event Func<float> getPlayerAngle;

    public float moveSpeed;
    public float jumpPower;
    public float gavityScale;
    private float _moveDir;
    private float _velY;

    [SerializeField] private Rigidbody2D rb2D;
    [SerializeField] private float swingForce = 5f;
    [SerializeField] private float swingDamping = 0.8f;

    private Silk currentRope;
    private bool wasSwinging = false;
    private Vector2 swingVelocity;
    private float releaseGravityScale = 0f;
    private float gravityTransitionSpeed = 20f;
    private float releaseBoostMultiplier = 3f;

    public float VelY
    {
        get => _velY;
        set
        {
            if (!PlayerInput.Instance.IsGrounded())
            {
                if (value > _velY)
                    animationJumpState?.Invoke(PlayerBehaviors.Jump);
                else if (value < _velY)
                    animationJumpState?.Invoke(PlayerBehaviors.Land);
            }
            else
            {
                setAni?.Invoke("Return");
            }
            _velY = value;
        }
    }

    void Start()
    {
        getPlayerAngle += SilkThrower.Instance.GetRotation;
        setAni += PlayerBehave.Instance.SetAnimationTrigger;
        comparePlayerBehavor += PlayerBehave.Instance.ComparePlayerBehave;
        animationJumpState += PlayerBehave.Instance.SetPlayerBehave;
        isPlayerDown += SilkThrower.Instance.IsPlayerDown;
    }

    void FixedUpdate()
    {
        bool isSwinging = PlayerBehave.Instance.CheckIsSilking();
        currentRope = SilkThrower.Instance.GetCurrentRope();

        if (isSwinging && currentRope != null)
        {
            HandleVerletRopeSwinging();
            wasSwinging = true;
        }
        else
        {
            if (wasSwinging)
            {
                HandleSwingRelease();
                wasSwinging = false;
            }
            else
            {
                HandleNormalMovement();
            }
        }
    }

    private void HandleVerletRopeSwinging()
    {
        rb2D.gravityScale = 0;
        Vector2 previousPos = transform.position;
        gameObject.transform.rotation = Quaternion.Euler(0, 0, (float)getPlayerAngle?.Invoke());

        // 로프 노드 위치로 부드러운 동기화
        Vector2 targetPosition = currentRope.GetNodePosition(0);
        Vector2 currentPosition = transform.position;

        // 거리 체크로 떨림 방지
        float distanceToTarget = Vector2.Distance(currentPosition, targetPosition);
        float maxSyncDistance = 0.1f; // 이 값을 조정하여 허용 거리 설정

        if (distanceToTarget > maxSyncDistance)
        {
            // 거리가 멀면 부드럽게 이동
            Vector2 velocityToTarget = (targetPosition - currentPosition) / Time.fixedDeltaTime;
            rb2D.linearVelocity = Vector2.Lerp(rb2D.linearVelocity, velocityToTarget, Time.fixedDeltaTime * 10f);
        }
        else
        {
            // 특정 범위 내에 있으면 현재 속도 유지 (떨림 방지)
            Vector2 currentVelocity = rb2D.linearVelocity;
            Vector2 ropeVelocity = EstimateRopeVelocity();

            // 로프 속도와 플레이어 속도 간의 차이가 크지 않으면 그대로 유지
            if (Vector2.Distance(currentVelocity, ropeVelocity) < 2f)
            {
                rb2D.linearVelocity = Vector2.Lerp(currentVelocity, ropeVelocity, Time.fixedDeltaTime * 5f);
            }
        }

        // 입력 처리 - 로프에만 힘 적용
        float input = _moveDir;
        if (Mathf.Abs(input) > 0.04f)
        {
            Vector3 horizontalForce = Vector3.right * input * swingForce;
            currentRope.ApplyForceToIndex(horizontalForce, 0);

            if (!(isPlayerDown?.Invoke() ?? false))
            {
                Vector3 gravityForce = Vector3.down * currentRope.gravity * 0.5f;
                currentRope.ApplyForceToIndex(gravityForce, 0);
            }
        }
        else
        {
            if (!PlayerInput.Instance.IsGrounded())
            {
                Vector3 gravityForce = Vector3.down * currentRope.gravity * 0.3f;
                currentRope.ApplyForceToIndex(gravityForce, 0);
            }
        }

        // 댐핑 적용 (떨림 억제를 위해 더 강하게)
        Vector3 dampingDir = EstimateRopeVelocity() * -swingDamping;

        // 거리가 가까울 때는 더 강한 댐핑 적용
        if (distanceToTarget <= maxSyncDistance)
        {
            dampingDir *= 1.5f; // 댐핑 강도 증가
        }

        currentRope.ApplyForceToIndex(dampingDir, 0);

        // 스윙 속도 계산 (릴리스할 때 사용) - 부드럽게 계산
        Vector2 newSwingVelocity = ((Vector2)transform.position - previousPos) / Time.fixedDeltaTime;
        swingVelocity = Vector2.Lerp(swingVelocity, newSwingVelocity, Time.fixedDeltaTime * 8f);
    }
    private void HandleSwingRelease()
    {
        Vector2 boostedVelocity = swingVelocity * releaseBoostMultiplier;
        rb2D.linearVelocity = boostedVelocity;

        transform.rotation = Quaternion.Euler(0, 0, 0);

        releaseGravityScale = 0f;
        rb2D.gravityScale = releaseGravityScale;

        VelY = rb2D.linearVelocity.y;

        if (currentRope != null)
        {
            currentRope.player = null;
            currentRope = null;
        }
    }

    private void HandleNormalMovement()
    {
        if (releaseGravityScale < gavityScale)
        {
            releaseGravityScale = Mathf.MoveTowards(releaseGravityScale, gavityScale, gravityTransitionSpeed * Time.fixedDeltaTime);
            rb2D.gravityScale = releaseGravityScale;
        }
        else
        {
            rb2D.gravityScale = gavityScale;
        }

        rb2D.linearVelocity = new Vector2(_moveDir * moveSpeed, rb2D.linearVelocity.y);
        VelY = rb2D.linearVelocity.y;
    }

    public void SetVelocityX(float dir)
    {
        _moveDir = dir;
    }

    public void Jump()
    {
        Vector2 velocity = rb2D.linearVelocity;
        velocity.y = jumpPower;
        rb2D.linearVelocity = velocity;
    }

    private Vector3 EstimateRopeVelocity()
    {
        if (currentRope == null || currentRope.player == null)
            return Vector3.zero;

        Vector3 head = currentRope.transform.position;
        Vector3 tail = currentRope.wall.position;
        return (tail - head).normalized;
    }

    /*private Vector3 CalculatePlayerPositionFromRope()
    {
        if (currentRope == null)
            return transform.position;

        Vector3 centerNode = currentRope.transform.position;
        Vector3 wall = currentRope.wall.position;
        float ropeCurvatureFactor = Vector3.Distance(centerNode, wall) / currentRope.ropeLength;


        float offsetY = Mathf.Clamp(1f - ropeCurvatureFactor, 0, 0.5f);
        Vector3 adjusted = centerNode + Vector3.down * offsetY;
        return adjusted;
    }*/
}
