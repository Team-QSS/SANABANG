using System;
using UnityEngine;

public class PlayerInteracter : HalfSingleMono<PlayerInteracter>
{
    public event Action onLand;
    private void Start()
    {
        onLand += PlayerInput.Instance.AbleJump;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.GetComponent<CustomTagger>().tags.HasFlag(Tags.Ground))
        {
            onLand?.Invoke();
        }
    }
}
