using UnityEngine;
using Vuforia;

public class ARTargetController : MonoBehaviour
{
    private ObserverBehaviour observer;
    private Animator animator;

    private bool isTracking = false; 

    [Header("Configuración")]
    public string targetID;
    public AudioClip sonido;

    private float shakeThreshold = 2.0f;
    private float lastShakeTime = 0f;
    private float shakeCooldown = 2f;

    void Start()
    {
        observer = GetComponent<ObserverBehaviour>();
        animator = GetComponentInChildren<Animator>();

        observer.OnTargetStatusChanged += OnTargetStatusChanged;
    }

    void Update()
    {
        DetectarTap();
        DetectarSacudida();
    }

   
    void DetectarTap()
    {
        if (!isTracking) return; 

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
               
                if (hit.transform.GetComponentInParent<ObserverBehaviour>() == observer)
                {
                    Debug.Log("Tap en: " + targetID);

                    if (animator != null)
                        animator.SetTrigger("Play");

                    AudioManager.Instance.PlaySound(sonido);

                    ARInteractionManager.Instance.RegistrarInteraccion(targetID, "tap");
                }
            }
        }
    }

    
    void DetectarSacudida()
    {
        if (!isTracking) return; 

        if (Time.time - lastShakeTime < shakeCooldown) return;

        if (Input.acceleration.sqrMagnitude > shakeThreshold || Input.GetKeyDown(KeyCode.Space))
        {
            EjecutarShake();
        }
    }

    void EjecutarShake()
    {
        lastShakeTime = Time.time;

        Debug.Log("Shake en: " + targetID);

        if (animator != null)
            animator.SetTrigger("Shake");

        AudioManager.Instance.PlaySound(sonido);

        ARInteractionManager.Instance.RegistrarInteraccion(targetID, "shake");
    }

    
    private void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
    {
        if (status.Status == Status.TRACKED || status.Status == Status.EXTENDED_TRACKED)
        {
            isTracking = true; 
            ActivarTarget();
        }
        else
        {
            isTracking = false; 
        }
    }

    void ActivarTarget()
    {
        Debug.Log("Detectado: " + targetID);

        if (animator != null)
            animator.SetTrigger("Detect");

        AudioManager.Instance.PlaySound(sonido);

        ARInteractionManager.Instance.RegistrarInteraccion(targetID, "detect");
    }
}