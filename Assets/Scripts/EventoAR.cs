using UnityEngine;
using Firebase;
using Firebase.Firestore;
using System.Collections.Generic;

public class EventoAR : MonoBehaviour
{
    FirebaseFirestore db;

    private Vector2 touchStartPos;

    private float initialDistance;
    private Vector3 initialScale;

    private float lastEventTime = 0f;
    private float cooldown = 0.5f;

    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
    }

    void Update()
    {
        DetectarToque();
        DetectarRotacionPC();
        DetectarEscalaPC();
        DetectarGestosMovil();
    }

    void DetectarToque()
    {
        
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            touchStartPos = Input.GetTouch(0).position;

            Ray ray = Camera.main.ScreenPointToRay(touchStartPos);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && hit.transform == transform)
            {
                GuardarEvento("toque", "Objeto tocado en móvil");
            }
        }

        
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && hit.transform == transform)
            {
                GuardarEvento("toque", "Objeto tocado en PC");
            }
        }
    }

    void DetectarRotacionPC()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            transform.Rotate(0, 45, 0);
            GuardarEvento("rotacion", "Objeto rotado en PC");
        }
    }


    void DetectarEscalaPC()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            transform.localScale += new Vector3(0.2f, 0.2f, 0.2f);
            GuardarEvento("escala", "Objeto escalado en PC");
        }
    }

 
    void DetectarGestosMovil()
    {
 
        if (Input.touchCount == 2)
        {
            Touch t1 = Input.GetTouch(0);
            Touch t2 = Input.GetTouch(1);

            float distanciaActual = Vector2.Distance(t1.position, t2.position);

            if (t1.phase == TouchPhase.Began || t2.phase == TouchPhase.Began)
            {
                initialDistance = distanciaActual;
                initialScale = transform.localScale;
            }
            else if (t1.phase == TouchPhase.Moved || t2.phase == TouchPhase.Moved)
            {
                if (initialDistance > 0)
                {
                    float factor = distanciaActual / initialDistance;
                    transform.localScale = initialScale * factor;

                    GuardarEvento("escala", "Zoom con dos dedos (móvil)");
                }
            }
        }


        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                float deltaX = touch.deltaPosition.x;

                // Solo si el movimiento es horizontal significativo
                if (Mathf.Abs(deltaX) > 5f)
                {
                    float rotationSpeed = deltaX * 0.2f;
                    transform.Rotate(0, -rotationSpeed, 0);

                    GuardarEvento("rotacion", "Deslizamiento horizontal (móvil)");
                }
            }
        }
    }


    void GuardarEvento(string tipo, string descripcion)
    {
        if (Time.time - lastEventTime < cooldown) return;
        lastEventTime = Time.time;

        Dictionary<string, object> evento = new Dictionary<string, object>
        {
            { "tipo", tipo },
            { "descripcion", descripcion },
            { "fecha", Timestamp.GetCurrentTimestamp() }
        };

        db.Collection("eventosAR").AddAsync(evento);
    }
}