using UnityEngine;

namespace DefaultNamespace
{
    public static class Extensions
    {
        private static LayerMask _layerMask = LayerMask.GetMask("Default");
        
        public static bool Raycast(this Rigidbody2D rigidbody, Vector2 direction)
        {
            if (rigidbody.bodyType == RigidbodyType2D.Kinematic) {
                return false;
            }

            float radius = 0.25f;
            float distance = 0.375f;

            RaycastHit2D hit = Physics2D.CircleCast(rigidbody.position, radius, direction.normalized, distance, _layerMask);
            return hit.collider != null && hit.rigidbody != rigidbody;
        }

    }
}