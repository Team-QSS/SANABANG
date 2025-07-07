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
}