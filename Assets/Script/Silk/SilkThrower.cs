using System;
using System.Data.Common;
using UnityEngine;

public enum SilkStatus
{
    Stay,
    Thrown,
    Hang,
    Return
}

public class SilkThrower : HalfSingleMono<SilkThrower>
{
    public event Action<bool> silking;
    [SerializeField] private LayerMask masks;
    private PlayerStatus playerStatus;
    private GameObject _attachedObj;
    private Vector2 _hookPoint;
    public GameObject AttachedObj
    {
        get => _attachedObj;
        set
        {
            _attachedObj = value;
        }
    }
    void Start()
    {
        playerStatus = PlayerStatus.Instance;
        silking += PlayerBehave.Instance.SetSilking;
    }

    void Update()
    {
        
    }
    public void SilkThrow()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 throwDir = (mouseWorldPos - transform.position).normalized;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, throwDir, playerStatus.PlayerSilkRange,masks);

        if (hit.collider != null)
        {
            CustomTagger tagger = hit.collider.GetComponent<CustomTagger>();
            Tags validTags = Tags.Wall | Tags.Silkable | Tags.Ground;

            if (tagger != null && (tagger.tags & validTags) != 0)
            {
                Debug.Log(hit.collider.name);
                AttachedObj = hit.collider.gameObject;
                _hookPoint = hit.point;
                silking?.Invoke(true);
            }
        }
        else
        {
            AttachedObj = null;
        }
    }
    public void SilkRelease()
    {
        silking?.Invoke(false);
        AttachedObj = null;
    }
    public Vector2 GetHookPoint()
    {
        return _hookPoint;
    }
    public bool IsPlayerDown()
    {
        if (gameObject.transform.position.y > AttachedObj.transform.position.y)
        {
            return false;
        }
        return true;
    }
    public float GetRotation()
    {
        var dir = AttachedObj.transform.position - gameObject.transform.position;
        var angle = Mathf.Atan2(dir.y, dir.x)*Mathf.Rad2Deg;
        return angle-90f;
    }

}
