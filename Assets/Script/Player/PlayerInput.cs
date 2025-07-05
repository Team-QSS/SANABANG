using System;
using UnityEngine;

public class PlayerInput : HalfSingleMono<PlayerInput>
{
    public event Action<float> moveX;
    public event Action<float> moveY;
    public event Action throwSilk;
    private void Start()
    {
        moveX += PlayerMove.Instance.SetVelocityX;
    }

    void Update()
    {
        KeyDetecter();
    }
    public void MoveX(float dir) 
    {
        if (dir > 0)
        {
            moveX?.Invoke(1);
        }
        else if(dir < 0)
        {
            moveX?.Invoke(-1);
        }
        else
        {
            moveX?.Invoke(0);
        }
    }
    public void MoveSilk(SilkStatus status)
    {
        switch (status)
        {
            //case SilkStatus.Thrown:

        }
    }
    public void ThrowSilk()
    {

    }
    public void ReturnSilk()
    {

    }
    public void HangingSilk()
    {

    }
    private void KeyDetecter()
    {
        float dir = 0f;
        if (Input.GetKey(KeyCode.A)) dir -= 1f;
        if (Input.GetKey(KeyCode.D)) dir += 1f;
        if (Input.GetMouseButtonDown(0))
        {
            ThrowSilk();
        }

        MoveX(dir);
    }
}
