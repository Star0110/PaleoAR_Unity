using UnityEngine;

/**
 * Clase que permite interactuar con un objeto en realidad aumentada.
 * Funcionalidades:
 * - Rotar el objeto arrastrándolo (mouse o touch)
 * - Reproducir sonido mientras se arrastra
 * - Rotar 180° al hacer tap/click
 * - Reproducir sonido al hacer tap
 */
public class InteraccionAR : MonoBehaviour
{
    // Audio que se reproduce mientras se arrastra el objeto
    public AudioSource audioDrag;

    // Audio que se reproduce al hacer tap/click
    public AudioSource audioTap;

    // Variable para saber si el usuario está arrastrando el objeto
    private bool isDragging = false;

    /**
     * Update se ejecuta en cada frame
     * Aquí se maneja toda la interacción del usuario
     */
    void Update()
    {
     
        // INTERACCIÓN CON MOUSE (PC)
        

        // Detecta cuando se presiona el botón izquierdo del mouse
        if (Input.GetMouseButtonDown(0))
        {
            // Crea un rayo desde la cámara hacia la posición del mouse
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Verifica si el rayo golpea este objeto
            if (Physics.Raycast(ray, out hit) && hit.transform == transform)
            {
                isDragging = true;

                // Inicia el sonido SOLO cuando empieza el arrastre
                audioDrag.Play();
            }
        }

        // Mientras el botón está presionado y se está arrastrando
        if (Input.GetMouseButton(0) && isDragging)
        {
            // Obtiene el movimiento horizontal del mouse
            float rotacion = Input.GetAxis("Mouse X") * 5f;

            // Rota el objeto en el eje Y (horizontal)
            transform.Rotate(Vector3.up, -rotacion, Space.World);
        }

        // Cuando se suelta el botón del mouse
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;

            // Detiene el sonido de arrastre
            audioDrag.Stop();
        }
        // INTERACCIÓN TOUCH (CELULAR)


        // Verifica si hay al menos un dedo tocando la pantalla
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Crea un rayo desde la cámara hacia la posición del dedo
            Ray ray = Camera.main.ScreenPointToRay(touch.position);
            RaycastHit hit;

            // Verifica si el toque está sobre este objeto
            if (Physics.Raycast(ray, out hit) && hit.transform == transform)
            {
                // Cuando el dedo toca la pantalla
                if (touch.phase == TouchPhase.Began)
                {
                    isDragging = true;

                    // Reproduce el sonido al iniciar
                    audioDrag.Play();
                }

                // Mientras el dedo se mueve
                if (touch.phase == TouchPhase.Moved && isDragging)
                {
                    float velocidad = 0.3f;

                    // Rota el objeto según el movimiento del dedo
                    transform.Rotate(Vector3.up, -touch.deltaPosition.x * velocidad, Space.World);
                }

                // Cuando el dedo deja de tocar la pantalla
                if (touch.phase == TouchPhase.Ended)
                {
                    isDragging = false;

                    // Detiene el sonido
                    audioDrag.Stop();
                }
            }
        }
    }

    /**
     * Evento que se ejecuta cuando se hace click/tap sobre el objeto
     * (requiere collider en el objeto)
     */
    void OnMouseDown()
    {
        // Rota el objeto 180 grados en el eje Y
        transform.Rotate(Vector3.up, 180f, Space.World);

        // Reproduce el sonido de tap
        audioTap.Play();
    }
}