using UnityEngine;
using System.Collections;

public class CameraMover : MonoBehaviour
{
    private Coroutine currentMove = null;
    [SerializeField] private MouseLookSway mouseLookSway;

    public void MoveCameraTo(Transform target, float duration)
    {
        if (currentMove != null)
            StopCoroutine(currentMove);
        currentMove = StartCoroutine(SmoothMove(target, duration));
    }

    IEnumerator SmoothMove(Transform target, float duration)
    {
        // Desactiva el sway
        mouseLookSway.enabled = false;

        Transform cam = Camera.main.transform;
        Vector3 startPos = cam.position;
        Quaternion startRot = cam.rotation;

        Vector3 endPos = target.position;
        Quaternion endRot = target.rotation;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            cam.position = Vector3.Lerp(startPos, endPos, t);
            cam.rotation = Quaternion.Slerp(startRot, endRot, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        cam.position = endPos;
        cam.rotation = endRot;

        // Reactiva el sway
        mouseLookSway.enabled = true;
        mouseLookSway.SetRotation(target.rotation);
    }

}

