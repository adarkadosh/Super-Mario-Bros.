using UnityEngine;

public static class Extensions
{
    private static LayerMask _layerMask = LayerMask.GetMask("Default");

    public static bool Raycast(this Rigidbody2D rigidbody, Vector2 direction)
    {
        if (rigidbody.bodyType == RigidbodyType2D.Kinematic)
        {
            return false;
        }

        float radius = 0.25f;
        float distance = 0.375f;

        RaycastHit2D hit = Physics2D.CircleCast(rigidbody.position, radius, direction.normalized, distance,
            _layerMask);
        return hit.collider != null && hit.rigidbody != rigidbody;
    }

    public static void DrawCircleCast(Vector2 origin, float radius, Vector2 direction, float distance)
    {
        // Draw the circle at the origin
        const int segments = 20;
        var angle = 0f;
        for (var i = 0; i < segments; i++)
        {
            var nextAngle = angle + 360f / segments;
            var start = origin +
                        new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) *
                        radius;
            var end = origin + new Vector2(Mathf.Cos(nextAngle * Mathf.Deg2Rad),
                Mathf.Sin(nextAngle * Mathf.Deg2Rad)) * radius;
            Debug.DrawLine(start, end, Color.red);
            angle = nextAngle;
        }

        // Draw the cast direction
        Debug.DrawRay(origin, direction.normalized * distance, Color.green);
    }
}