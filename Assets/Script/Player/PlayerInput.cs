using System;
using UnityEngine;
using UnityEngine.Playables;

public class PlayerInput : HalfSingleMono<PlayerInput>
{
    public event Func<PlayerBehaviors, bool> comparePlayerBehavor;

    public event Action<PlayerBehaviors> setPlayerBehavior;
    public event Action<float> moveX;
    //public event Action<float> moveY;
    public event Action throwSilk;
    public event Action returnSilk;
    public event Action jump;

    private float maxJumpTime =0.4f;
    private float currentJumpingTime;
    private void Start()
    {
        setPlayerBehavior += PlayerBehave.Instance.SetPlayerBehave;
        comparePlayerBehavor += PlayerBehave.Instance.ComparePlayerBehave;
        moveX += PlayerMove.Instance.SetVelocityX;
        jump += PlayerMove.Instance.Jump;
    }

    void Update()
    {
        KeyDetecter();
    }
    public void MoveX(float dir) 
    {
        if (dir != 0)
        {
            if (dir > 0)
            {
                moveX?.Invoke(1);
            }
            else if (dir < 0)
            {
                moveX?.Invoke(-1);
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
        currentJumpingTime = 0f;
    }
    public void ThrowSilk()
    {
        throwSilk?.Invoke();
    }
    public void ReturnSilk()
    {
        returnSilk?.Invoke();
    }
    public void Jump()
    {
        if (currentJumpingTime < maxJumpTime)
        {
            currentJumpingTime += Time.fixedDeltaTime;
            jump?.Invoke();
            setPlayerBehavior?.Invoke(PlayerBehaviors.Jump);
        }

    }
    private void KeyDetecter()
    {
        float dir = 0f;
        setPlayerBehavior?.Invoke(PlayerBehaviors.Idle);
        if (Input.GetKey(KeyCode.A)) dir -= 1f;
        if (Input.GetKey(KeyCode.D)) dir += 1f;
        if (Input.GetMouseButtonDown(0))
        {
            ThrowSilk();
        }
        if (Input.GetMouseButtonUp(0))
        {
            ReturnSilk();
        }
        if (Input.GetKey(KeyCode.Space))
        {
            Jump();
        }
        MoveX(dir);
    }
}
