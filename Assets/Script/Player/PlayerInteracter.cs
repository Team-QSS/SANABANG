using System;
using UnityEngine;

public class PlayerInteracter : HalfSingleMono<PlayerInteracter>
{
    public event Action onLand;
    public event Action onJump;
    public event Action<PlayerBehaviors> landBehave;
    public event Action<string> returnAni;
    private void Start()
    {
        onJump += PlayerInput.Instance.DisableJump;
        landBehave += PlayerBehave.Instance.SetPlayerBehave;
        onLand += PlayerInput.Instance.AbleJump;
        returnAni += PlayerBehave.Instance.SetAnimationTrigger;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.GetComponent<CustomTagger>().tags.HasFlag(Tags.Ground))
        {
            onLand?.Invoke();
            landBehave?.Invoke(PlayerBehaviors.Idle);
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.GetComponent<CustomTagger>().tags.HasFlag(Tags.Ground))
        {
            Debug.Log(1);
            onJump?.Invoke();
        }
    }
}
