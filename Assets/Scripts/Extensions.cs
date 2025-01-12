using System.Collections;
using DG.Tweening;
using UnityEngine;

public static class Extensions
{
    private static LayerMask _layerMask = LayerMask.GetMask("Default");

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

    public static IEnumerator AnimatedBlockGotHit(GameObject gameObject, float offsetY = 0.5f,
        float duration = 0.125f)
    {
        Vector3 originalPosition = gameObject.transform.position;
        yield return gameObject.transform.DOMoveY(gameObject.transform.position.y + offsetY, duration)
            .SetEase(Ease.Linear);
        yield return new WaitForSeconds(duration);
        yield return gameObject.transform.DOMoveY(originalPosition.y, duration / 2).SetEase(Ease.Linear);
    }
}