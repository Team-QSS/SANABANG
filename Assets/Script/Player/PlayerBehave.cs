using UnityEngine;
using UnityEngine.UIElements.Experimental;

public enum PlayerBehaviors
{
    Idle,
    Run,
    Jump,
    Land,
    Silking,
}

public class PlayerBehave : HalfSingleMono<PlayerBehave>
{
    [SerializeField] private PlayerBehaviors behavior;
    [SerializeField] private Animator ani;
    [SerializeField] private SpriteRenderer sr;
    private bool _isFacingRight;
    public bool IsFacingRight
    {
        get => _isFacingRight;
        set
        {
            if(value != _isFacingRight)
            {
                _isFacingRight = value;
                Flip();
            }
        }
    }
    void Start()
    {
        IsFacingRight = _isFacingRight;
    }
    private void Flip()
    {
        sr.flipX = IsFacingRight;
    }
    public void SetPlayerBehave(PlayerBehaviors playerBehaviors)
    {
        if (behavior == playerBehaviors) return;
        behavior = playerBehaviors;
        switch (playerBehaviors)
        {
            case PlayerBehaviors.Jump:
                ani.SetTrigger("Jump");
                break;
            case PlayerBehaviors.Land:
                ani.SetTrigger("Land");
                break;
            default:
                ani.SetInteger("Behave", (int)behavior);
                break;
        }
    }
    public bool ComparePlayerBehave(PlayerBehaviors playerBehave)
    {
        if(behavior == playerBehave)
        {
            return true;
        }
        return false;
    }
    public void ChangeDir(bool flip)
    {
        IsFacingRight = flip;
    } 
}
