using System.Collections;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public static class Extensions
{
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

    // public static IEnumerator AnimatedBlockGotHit(GameObject gameObject, float offsetY = 0.5f,
    //     float duration = 0.125f)
    // {
    //     Vector3 originalPosition = gameObject.transform.position;
    //     yield return gameObject.transform.DOMoveY(gameObject.transform.position.y + offsetY, duration)
    //         .SetEase(Ease.Linear);
    //     yield return new WaitForSeconds(duration);
    //     yield return gameObject.transform.DOMoveY(originalPosition.y, duration / 2).SetEase(Ease.Linear);
    // }
    public static IEnumerator AnimatedBlockGotHit(GameObject gameObject, float offsetY = 0.5f,
        float duration = 0.125f, float offsetOnReturn = 0f, float delay = 0.05f)
    {
        Vector3 originalPosition = gameObject.transform.position;

        // Move the block up
        Tweener moveUp = gameObject.transform.DOMoveY(originalPosition.y + offsetY, duration)
            .SetEase(Ease.Linear);
        yield return moveUp.WaitForCompletion();
        
        if (gameObject.layer == LayerMask.NameToLayer("Bricks"))
            KillEnemiesOnBlock(gameObject);

        // Optional: Wait for a short duration if needed
        yield return new WaitForSeconds(delay); // Adjust as necessary

        // Move the block back down
        Tweener moveDown = gameObject.transform.DOMoveY(originalPosition.y + offsetOnReturn, duration / 2)
            .SetEase(Ease.Linear);
        yield return moveDown.WaitForCompletion();
    }
    
    public static void KillEnemiesOnBlock(GameObject block)
    {
        // Get the BoxCollider2D component from the block
        BoxCollider2D blockCollider = block.GetComponent<BoxCollider2D>();
        if (blockCollider == null)
        {
            Debug.LogWarning("Block does not have a BoxCollider2D component.");
            return;
        }

        Vector2 blockCenter = blockCollider.bounds.center;
        Vector2 blockSize = blockCollider.bounds.size;

        int enemyLayerMask = LayerMask.GetMask("Enemy");
        Collider2D[] enemyColliders = Physics2D.OverlapBoxAll(blockCenter, blockSize, 0f, enemyLayerMask);

        foreach (Collider2D collider in enemyColliders)
        {
            // Assuming each enemy has a script/component named "Enemy"
            EnemyBehavior enemy = collider.GetComponent<EnemyBehavior>();
            if (enemy != null)
            {
                GameEvents.OnEventTriggered?.Invoke(ScoresSet.OneHundred, enemy.transform.position);
                enemy.StartCoroutine(enemy.DeathSequence());
            }
        }
    }

    public static bool IsVisibleToCamera(this Camera mainCamera, Transform transform)
    {
        if (mainCamera == null)
            return false;

        Vector3 viewportPos = mainCamera.WorldToViewportPoint(transform.position);
        return viewportPos.x is >= 0 and <= 1 && viewportPos.y is >= 0 and <= 1 && viewportPos.z > 0;
    }
    
    public static IEnumerator WaitForSeconds(float duration)
    {
        yield return new WaitForSeconds(duration);
    }
    
    public static void Log(string message)
    {
#if UNITY_EDITOR
        Debug.Log(message);
#endif
    }
}