using System;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private float screenThreshold = 0.5f;
    // [SerializeField] private float smoothSpeed = 0.125f;
    // [SerializeField] private Vector3 offset;

    private void Update()
    {
        Vector3 playerViewportPosition = Camera.main.WorldToViewportPoint(player.position);
        if (playerViewportPosition.x > screenThreshold && player.transform.position.x > transform.position.x)
        {
            Vector3 newPosition = new Vector3(player.position.x, transform.position.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, newPosition, followSpeed * Time.deltaTime);
        }
    }
}
