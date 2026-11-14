using UnityEngine;

public class AABBCollider : MonoBehaviour
{
    #region Serialized Fields
    [Header("Collider Dimensions")]
    [SerializeField] private float _width = 1f;
    [SerializeField] private float _height = 1f;

    [Header("Collider Settings")]
    [SerializeField] private ColliderType _colliderType = ColliderType.Obstacle;
    [SerializeField] private Color _debugWireColor = Color.green;

    [Header("Dependencies")]
    [SerializeField] private CollisionManager _collisionManager;
    #endregion

    #region Properties
    public Vector2 Min { get; private set; }
    public Vector2 Max { get; private set; }
    public Vector2 Size => new Vector2(_width, _height);
    public Vector2 Center => transform.position;
    public ColliderType Type => _colliderType;
    #endregion

    #region Events
    public delegate void CollisionEventHandler(AABBCollider other);
    public event CollisionEventHandler OnCollided;
    #endregion

    #region Unity Lifecycle
    private void Start()
    {
        RegisterWithManager();
    }

    private void Update()
    {
        UpdateBounds();
        DrawDebugWireframe();
    }

    private void OnDestroy()
    {
        UnregisterFromManager();
    }
    #endregion

    #region Public Methods
    public void NotifyCollision(AABBCollider other)
    {
        if (other == null) return;
        OnCollided?.Invoke(other);
    }

    public void SetWireColor(Color newColor)
    {
        _debugWireColor = newColor;
    }
    #endregion

    #region Calculation & Drawing Helpers
    private void UpdateBounds()
    {
        Vector2 currentPosition = transform.position;
        Vector2 halfExtents = new Vector2(_width * 0.5f, _height * 0.5f);
        Min = currentPosition - halfExtents;
        Max = currentPosition + halfExtents;
    }

    private void DrawDebugWireframe()
    {
        Vector3 topLeft = new Vector3(Min.x, Max.y, 0f);
        Vector3 topRight = new Vector3(Max.x, Max.y, 0f);
        Vector3 bottomRight = new Vector3(Max.x, Min.y, 0f);
        Vector3 bottomLeft = new Vector3(Min.x, Min.y, 0f);
        
        Debug.DrawLine(topLeft, topRight, _debugWireColor);
        Debug.DrawLine(topRight, bottomRight, _debugWireColor);
        Debug.DrawLine(bottomRight, bottomLeft, _debugWireColor);
        Debug.DrawLine(bottomLeft, topLeft, _debugWireColor);
    }
    #endregion

    #region Registration Logic
    private void RegisterWithManager()
    {
        if (_collisionManager == null)
        {
            Debug.LogWarning($"[AABBCollider] Missing CollisionManager on {gameObject.name}!", this);
            return;
        }
        _collisionManager.Register(this);
    }

    private void UnregisterFromManager()
    {
        if (_collisionManager == null) return;
        _collisionManager.Unregister(this);
    }
    #endregion
}