using UnityEngine;

public enum PlayerBehaviors
{
    Idle,
    Run,
    Jump,
    Silking,
}

public class PlayerBehave : HalfSingleMono<PlayerBehave>
{
    [SerializeField] private PlayerBehaviors behavior;
    void Start()
    {
        
    }
    void Update()
    {
        
    }
    public void SetPlayerBehave(PlayerBehaviors playerBehaviors)
    {
        behavior = playerBehaviors;
    }
    public bool ComparePlayerBehave(PlayerBehaviors playerBehave)
    {
        if(behavior == playerBehave)
        {
            return true;
        }
        return false;
    }
}
