using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLookSway : MonoBehaviour
{
    [Header("Ajustes de Sway")]
    public float swayAmount = 5f;
    public float smoothSpeed = 5f;

    private Quaternion initialRotation;

    void Start()
    {
        initialRotation = transform.localRotation;
        Debug.Log($"Initial Rotation: {initialRotation.eulerAngles}");
    }

    void Update()
    {
        // Obtener la posición del ratón en píxeles
        Vector2 mousePos = Mouse.current.position.ReadValue();

        // Clamp a los valores del ratón dentro de los límites de la pantalla
        mousePos.x = Mathf.Clamp(mousePos.x, 0f, Screen.width);
        mousePos.y = Mathf.Clamp(mousePos.y, 0f, Screen.height);

        // Normalizar entre -1 y 1 según el tamaño de pantalla
        float halfWidth = Screen.width / 2f;
        float halfHeight = Screen.height / 2f;

        float normalizedX = (mousePos.x - halfWidth) / halfWidth;   // -1 a 1
        float normalizedY = (mousePos.y - halfHeight) / halfHeight; // -1 a 1

        // Aplica sway basado en posición relativa
        float rotX = Mathf.Clamp(-normalizedY * swayAmount, -swayAmount, swayAmount);
        float rotY = Mathf.Clamp(normalizedX * swayAmount, -swayAmount, swayAmount);

        // Crear la rotación objetivo como Quaternion
        Quaternion targetRotation = initialRotation * Quaternion.Euler(rotX, rotY, 0f);

        // Suavizar la rotación
        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * smoothSpeed);
    }

    public void SetRotation(Quaternion rotation)
    {
        initialRotation = rotation;
        Debug.Log($"SetRotation called with: {rotation.eulerAngles}");
    }
}


