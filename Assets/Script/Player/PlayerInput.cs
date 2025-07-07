using System;
using UnityEngine;

public class PlayerInput : HalfSingleMono<PlayerInput>
{
    public event Func<PlayerBehaviors, bool> comparePlayerBehavor;
    public event Action<PlayerBehaviors> setPlayerBehavior;
    public event Action<float> moveX;
    public event Action jump;
    public event Action throwSilk;
    public event Action returnSilk;

    public event Action<bool> flip;

    private float maxJumpTime = 0.5f;
    private float currentJumpingTime;
    private bool isJumping;
    private bool isGrounded;
    private bool jumpRequested;

    private void Start()
    {
        setPlayerBehavior += PlayerBehave.Instance.SetPlayerBehave;
        comparePlayerBehavor += PlayerBehave.Instance.ComparePlayerBehave;
        moveX += PlayerMove.Instance.SetVelocityX;
        jump += PlayerMove.Instance.Jump;
        flip += PlayerBehave.Instance.ChangeDir;
    }

    void Update()
    {
        KeyDetecter();
    }

    void FixedUpdate()
    {
        if (jumpRequested && isGrounded)
        {
            isJumping = true;
            currentJumpingTime = 0f;
            isGrounded = false;
            jumpRequested = false;
        }

        if (isJumping)
        {
            if (currentJumpingTime < maxJumpTime)
            {
                currentJumpingTime += Time.fixedDeltaTime;
                jump?.Invoke();
                setPlayerBehavior?.Invoke(PlayerBehaviors.Jump);
            }
            else
            {
                isJumping = false;
            }
        }
    }

    public void MoveX(float dir)
    {
        if (dir != 0)
        {
            if (dir > 0)
            {
                moveX?.Invoke(1);
                flip?.Invoke(false);
            }
            else
            {
                moveX?.Invoke(-1);
                flip?.Invoke(true);
            }
            setPlayerBehavior?.Invoke(PlayerBehaviors.Run);
        }
        else
        {
            moveX?.Invoke(0);
        }
    }

    public void AbleJump()
    {
        isGrounded = true;
        isJumping = false;
        currentJumpingTime = maxJumpTime;
    }

    public void ThrowSilk()
    {
        throwSilk?.Invoke();
    }

    public void ReturnSilk()
    {
        returnSilk?.Invoke();
    }

    private void KeyDetecter()
    {
        float dir = 0f;
        setPlayerBehavior?.Invoke(PlayerBehaviors.Idle);

        if (Input.GetKey(KeyCode.A)) dir -= 1f;
        if (Input.GetKey(KeyCode.D)) dir += 1f;

        if (Input.GetMouseButtonDown(0)) ThrowSilk();
        if (Input.GetMouseButtonUp(0)) ReturnSilk();

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            jumpRequested = true;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            isJumping = false;
        }

        MoveX(dir);
    }
}
