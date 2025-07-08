using System;
using System.Data.Common;
using UnityEngine;

/*public enum SilkStatus
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

}*/
public enum RopeStatus
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
    [SerializeField] private GameObject ropePrefab;
    [SerializeField] private float maxRopeDistance = 15f;

    private PlayerStatus playerStatus;
    private GameObject _attachedObj;
    private Vector2 _hookPoint;
    private Silk currentRope;
    private GameObject currentRopeGameObject;

    public GameObject AttachedObj
    {
        get => _attachedObj;
        set => _attachedObj = value;
    }

    void Start()
    {
        playerStatus = PlayerStatus.Instance;
        silking += PlayerBehave.Instance.SetSilking;
    }

    public void SilkThrow()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 throwDir = (mouseWorldPos - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, throwDir, playerStatus.PlayerSilkRange, masks);

        if (hit.collider != null)
        {
            CustomTagger tagger = hit.collider.GetComponent<CustomTagger>();
            Tags validTags = Tags.Wall | Tags.Silkable | Tags.Ground;

            if (tagger != null && (tagger.tags & validTags) != 0)
            {
                Debug.Log(hit.collider.name);
                AttachedObj = hit.collider.gameObject;
                _hookPoint = hit.point;

                CreateRope(_hookPoint);
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
        DestroyRope();
    }

    private void CreateRope(Vector2 hitPoint)
    {
        if (ropePrefab != null && AttachedObj != null)
        {
            currentRopeGameObject = Instantiate(ropePrefab, transform.position, Quaternion.identity);
            currentRope = currentRopeGameObject.GetComponent<Silk>();

            if (currentRope != null)
            {
                currentRope.player = transform;
                currentRope.wall = AttachedObj.transform;
                currentRope.firstPointPos = hitPoint;
                float distance = Vector2.Distance(transform.position, _hookPoint);
                currentRope.ropeLength = Mathf.Min(distance, maxRopeDistance);
                currentRope.nodeCount = Mathf.Max(10, Mathf.RoundToInt(distance / 0.5f));

                currentRope.SetRope();
            }
        }
    }

    private void DestroyRope()
    {
        if (currentRopeGameObject != null)
        {
            Destroy(currentRopeGameObject);
            currentRopeGameObject = null;
            currentRope = null;
        }
    }

    public Vector2 GetHookPoint()
    {
        return _hookPoint;
    }

    public bool IsPlayerDown()
    {
        if (AttachedObj != null)
        {
            return gameObject.transform.position.y < AttachedObj.transform.position.y;
        }
        return true;
    }

    public float GetRotation()
    {
        if (AttachedObj != null)
        {
            var dir = AttachedObj.transform.position - gameObject.transform.position;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            return angle - 90f;
        }
        return 0f;
    }

    public Silk GetCurrentRope()
    {
        return currentRope;
    }

    public void ApplyForceToRope(Vector3 force)
    {
        if (currentRope != null)
        {
            currentRope.ApplyForceToCenter(force);
        }
    }

    public void ApplyWindToRope(Vector3 windForce)
    {
        if (currentRope != null)
        {
            currentRope.ApplyWindForce(windForce);
        }
    }

    public void ApplyForceAtRopePosition(Vector3 worldPosition, Vector3 force, float radius = 1f)
    {
        if (currentRope != null)
        {
            currentRope.ApplyForceAtPosition(worldPosition, force, radius);
        }
    }
}
