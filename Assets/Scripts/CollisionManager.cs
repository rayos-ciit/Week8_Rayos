using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    #region Private Variables
    private readonly List<AABBCollider> _activeColliders = new();
    #endregion

    #region Registration Methods
    public void Register(AABBCollider collider)
    {
        if (collider == null) return;
        if (_activeColliders.Contains(collider)) return;

        _activeColliders.Add(collider);
    }

    public void Unregister(AABBCollider collider)
    {
        if (collider == null) return;
        if (!_activeColliders.Contains(collider)) return;

        _activeColliders.Remove(collider);
    }
    #endregion

    #region Unity Lifecycle
    private void FixedUpdate()
    {
        EvaluateAllCollisions();
    }
    #endregion

    #region Collision Processing
    private void EvaluateAllCollisions()
    {
        int count = _activeColliders.Count;
        if (count < 2) return;

        for (int i = 0; i < count; i++)
        {
            AABBCollider colliderA = _activeColliders[i];
            if (colliderA == null) continue;

            for (int j = i + 1; j < count; j++)
            {
                AABBCollider colliderB = _activeColliders[j];
                if (colliderB == null) continue;

                if (EvaluateOverlap(colliderA, colliderB))
                {
                    colliderA.NotifyCollision(colliderB);
                    colliderB.NotifyCollision(colliderA);
                }
            }
        }
    }

    private bool EvaluateOverlap(AABBCollider a, AABBCollider b)
    {
        bool overlapsX = a.Min.x <= b.Max.x && b.Min.x <= a.Max.x;
        bool overlapsY = a.Min.y <= b.Max.y && b.Min.y <= a.Max.y;

        return overlapsX && overlapsY;
    }
    #endregion
}