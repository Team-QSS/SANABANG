using UnityEngine;

public class PlayerMove : HalfSingleMono<PlayerMove>
{
    public float moveSpeed;
    public float jumpPower;
    private float moveDir;
    [SerializeField] private Rigidbody2D rb2D;
    
    void Start()
    {
        
    }
    
    void Update()
    {
       
    }
    
    private void FixedUpdate()
    {
        rb2D.linearVelocityX = moveDir * moveSpeed;

    }
    
    public void SetVelocityX(float dir)
    {
        moveDir = dir;
    }

    public void Jump()
    {
        Vector2 velocity = rb2D.linearVelocity;
        velocity.y = jumpPower;
        rb2D.linearVelocity = velocity;
    }
}