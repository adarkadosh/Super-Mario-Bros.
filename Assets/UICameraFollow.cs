using UnityEngine;

public class UICameraFollow : MonoBehaviour
{
    public Transform mainCameraTransform;

    void LateUpdate()
    {
        // transform.position = mainCameraTransform.position;
        // transform.rotation = mainCameraTransform.rotation;
        transform.position = new Vector3(mainCameraTransform.position.x, mainCameraTransform.position.y,
            transform.position.z);
    }
}