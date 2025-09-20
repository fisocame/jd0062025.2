using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PressurePlateCompleta : MonoBehaviour
{
    public enum PressMode { HoldWhilePressed, Toggle, OneShot }

    [Header("Behavior")]
    public PressMode mode = PressMode.HoldWhilePressed;
    public float requiredMass = 5f;          // massa mínima para ativar
    public float debounceTime = 0.05f;       // filtro anti-tremida

    [Header("Targets (IActivatable)")]
    public List<MonoBehaviour> targets = new(); // arraste DoorController, DrawerController etc

    [Header("Plate Visual (opcional)")]
    public Transform topVisual;         // a parte que afunda
    public float pressDepth = 0.03f;    // profundidade do afundamento
    public float pressLerpSpeed = 8f;   // velocidade do Lerp

    private readonly HashSet<Rigidbody> bodies = new();
    private float lastChange = -999f;
    private bool isActive = false;
    private Vector3 topStartLocalPos;

    void Awake()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true; // precisa ser Trigger

        if (topVisual)
            topStartLocalPos = topVisual.localPosition;
    }

    void Update()
    {
        // animação do tampo afundando
        if (topVisual)
        {
            Vector3 target = topStartLocalPos + (isActive ? Vector3.down * pressDepth : Vector3.zero);
            topVisual.localPosition = Vector3.Lerp(topVisual.localPosition, target, Time.deltaTime * pressLerpSpeed);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        var rb = other.attachedRigidbody;
        if (rb) bodies.Add(rb);
        Evaluate();
    }

    void OnTriggerExit(Collider other)
    {
        var rb = other.attachedRigidbody;
        if (rb) bodies.Remove(rb);
        Evaluate();
    }

    void Evaluate()
    {
        if (Time.time - lastChange < debounceTime) return;

        float total = 0f;
        foreach (var rb in bodies) if (rb) total += rb.mass;

        bool shouldActivate = total >= requiredMass;

        switch (mode)
        {
            case PressMode.HoldWhilePressed:
                SetActive(shouldActivate);
                break;
            case PressMode.Toggle:
                SetActive(shouldActivate);
                break;
            case PressMode.OneShot:
                if (shouldActivate && !isActive) SetActive(true);
                break;
        }

        lastChange = Time.time;
    }

    void SetActive(bool on)
    {
        if (on == isActive) return;
        isActive = on;

        foreach (var mb in targets)
        {
            if (mb is IActivatable act)
            {
                if (on) act.Activate();
                else    act.Deactivate();
            }
        }
    }
}