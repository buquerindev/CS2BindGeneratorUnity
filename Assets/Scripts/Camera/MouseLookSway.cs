using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLookSway : MonoBehaviour
{
    [Header("Ajustes de Sway")]
    public float swayAmount = 5f;
    public float smoothSpeed = 5f;

    public Vector3 initialRotation;

    void Start()
    {
        initialRotation = transform.localEulerAngles;
    }

    void Update()
    {
        // Obtener la posición del ratón en píxeles
        Vector2 mousePos = Mouse.current.position.ReadValue();

        // Normalizar entre -1 y 1 según el tamaño de pantalla
        float halfWidth = Screen.width / 2f;
        float halfHeight = Screen.height / 2f;

        float normalizedX = (mousePos.x - halfWidth) / halfWidth;   // -1 a 1
        float normalizedY = (mousePos.y - halfHeight) / halfHeight; // -1 a 1

        // Aplica sway basado en posición relativa
        float rotX = Mathf.Clamp(-normalizedY * swayAmount, -swayAmount, swayAmount);
        float rotY = Mathf.Clamp(normalizedX * swayAmount, -swayAmount, swayAmount);

        Vector3 targetRotation = new Vector3(initialRotation.x + rotX, initialRotation.y + rotY, initialRotation.z);
        transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, targetRotation, Time.deltaTime * smoothSpeed);
    }
}


