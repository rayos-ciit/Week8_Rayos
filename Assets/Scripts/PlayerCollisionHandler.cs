using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCollisionHandler : MonoBehaviour
{
    #region Serialized Fields
    [Header("Dependencies")]
    [SerializeField] private AABBCollider _playerCollider;
    [SerializeField] private PlayerMovement _playerMovement;
    #endregion

    #region Unity Lifecycle
    private void OnEnable()
    {
        if (_playerCollider == null) return;

        _playerCollider.OnCollided += HandleCollision;
    }

    private void OnDisable()
    {
        if (_playerCollider == null) return;

        _playerCollider.OnCollided -= HandleCollision;
    }
    #endregion

    #region Collision Dispatcher
    private void HandleCollision(AABBCollider other)
    {
        if (other == null) return;

        switch (other.Type)
        {
            case ColliderType.Ground:
                ResolveGroundCollision(other);
                break;

            case ColliderType.Obstacle:
                ResolveObstacleCollision(other);
                break;

            case ColliderType.FallCollider:
                TriggerLevelRestart();
                break;
        }
    }
    #endregion

    #region Resolution Logic
    private void ResolveGroundCollision(AABBCollider ground)
{
    float overlapX = Mathf.Min(_playerCollider.Max.x, ground.Max.x) - Mathf.Max(_playerCollider.Min.x, ground.Min.x);
    float overlapY = Mathf.Min(_playerCollider.Max.y, ground.Max.y) - Mathf.Max(_playerCollider.Min.y, ground.Min.y);

    if (overlapX <= 0f || overlapY <= 0f) return;

    if (overlapX < overlapY)
    {
        ResolveHorizontalOverlap(ground, overlapX);
        return;
    }

    if (_playerCollider.Center.y > ground.Center.y)
    {
        //only land if falling downward
        if (_playerMovement.VerticalVelocity > 0.05f) return;

        float groundTop = ground.Max.y;
        float playerHalfHeight = _playerCollider.Size.y * 0.5f;

        Vector3 currentPos = transform.position;
        currentPos.y = groundTop + playerHalfHeight;
        transform.position = currentPos;

        _playerMovement.SetGrounded(true);
        return;
    }

    Vector3 pos = transform.position;
    pos.y -= overlapY;
    transform.position = pos;
    _playerMovement.ZeroVerticalVelocity();
}


    private void ResolveObstacleCollision(AABBCollider obstacle)
    {
        float overlapX = Mathf.Min(_playerCollider.Max.x, obstacle.Max.x) - Mathf.Max(_playerCollider.Min.x, obstacle.Min.x);
        float overlapY = Mathf.Min(_playerCollider.Max.y, obstacle.Max.y) - Mathf.Max(_playerCollider.Min.y, obstacle.Min.y);

        if (overlapX <= 0f || overlapY <= 0f) return;

        if (overlapX < overlapY)
        {
            ResolveHorizontalOverlap(obstacle, overlapX);
            return;
        }

        ResolveVerticalOverlap(obstacle, overlapY);
    }

    private void ResolveHorizontalOverlap(AABBCollider obstacle, float overlapX)
    {
        Vector3 pos = transform.position;

        if (_playerCollider.Center.x < obstacle.Center.x)
        {
            pos.x -= overlapX;
        }
        else
        {
            pos.x += overlapX;
        }

        transform.position = pos;
    }

    private void ResolveVerticalOverlap(AABBCollider obstacle, float overlapY)
    {
        Vector3 pos = transform.position;

        if (_playerCollider.Center.y > obstacle.Center.y)
        {
            pos.y += overlapY;
            transform.position = pos;
            _playerMovement.SetGrounded(true);
            return;
        }

        pos.y -= overlapY;
        transform.position = pos;
        _playerMovement.ZeroVerticalVelocity();
    }

    private void TriggerLevelRestart()
    {
        int activeSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(activeSceneIndex);
    }
    #endregion
}
