using UnityEngine;

public class CanvasFaceCamera : MonoBehaviour
{
    private GameObject targetCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        targetCamera = Camera.main.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - targetCamera.transform.position);
    }
}