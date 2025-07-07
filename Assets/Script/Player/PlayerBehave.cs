using UnityEngine;
using UnityEngine.UIElements.Experimental;

public enum PlayerBehaviors
{
    Idle,
    Run,
    Jump,
    Land,
}

public class PlayerBehave : HalfSingleMono<PlayerBehave>
{
   [SerializeField] private bool _isSilking;
    public bool IsSilking
    {
        get => _isSilking;
        set
        {
            _isSilking = value;
        }
    }
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
        if (!IsSilking)
        {
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
        else
        {
            ani.SetTrigger("Jump");
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
    public void SetAnimationTrigger(string key)
    {
        if (HasAnimationParam(key))
        {
            ani.SetTrigger(key);
        }
    }
    private bool HasAnimationParam(string key)
    {
        foreach (var param in ani.parameters)
        {
            if (param.name == key)
            {
                return true;
            }
        }
        return false;
    }
    public void ForceSetAnimation(string key) 
    {
        ani.Play(key);
    }
    public void SetSilking(bool val)
    {
        IsSilking = val;
    }
    public bool CheckIsSilking()
    {
        return IsSilking;
    }
}
