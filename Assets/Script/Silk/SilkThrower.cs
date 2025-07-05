using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public enum SilkStatus
{
    Stay,
    Thrown,
    Hang,
    Return
}

public class SilkThrower : HalfSingleMono<SilkThrower>
{
    private PlayerStatus playerStatus;
    private GameObject _attachedObj;
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
    }

    void Update()
    {
        
    }
    public void SilkThrow()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 throwDir = (mouseWorldPos - transform.position).normalized;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, throwDir, playerStatus.PlayerSilkRange);

        if (hit.collider != null)
        {
            CustomTagger tagger = hit.collider.GetComponent<CustomTagger>();
            Tags validTags = Tags.Wall | Tags.Silkable | Tags.Ground;

            if (tagger != null && (tagger.tags & validTags) != 0)
            {
                AttachedObj = hit.collider.gameObject;
            }
        }
        else
        {
            AttachedObj = null;
        }
    }
    public void SilkRelease()
    {
        AttachedObj = null;
    }
    public Vector2 GetDirectionFromPlayer()
    {
        return (AttachedObj.transform.position - gameObject.transform.position).normalized;
    }

}
