using UnityEngine;

public class PlayerStatus :HalfSingleMono<PlayerStatus>
{
    private float _playerSilkRange;
    public float PlayerSilkRange;
    void Start()
    {
        PlayerSilkRange = 10f;
    }

    void Update()
    {
        
    }
}
