using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class ShortRangePull : MonoBehaviour
{
    [Header("Refs")]
    public XRDirectInteractor directInteractor;          // XRDirectInteractor desta mão
    public Transform pullTarget;                         // ponto na palma/attach transform

    [Header("Input")]
    public InputActionReference pullAction;              // botão lateral (Grip/Side)

    [Header("Config")]
    public float maxDistance = 1.2f;                     // alcance máx. para puxar
    public float searchRadius = 0.25f;                   // “largura” do cone de busca
    public float pullSpeed = 8f;                         // velocidade de aproximação
    public LayerMask obstructionMask = ~0;               // layers que BLOQUEIAM visão

    [Header("Filtros")]
    public LayerMask candidateMask = ~0;                 // onde estão os objetos puxáveis
    public bool requireGrabbable = false;                // se true, só puxa se tiver XRGrabInteractable

    // ---- estado interno ----
    bool _pullHeld = false;                              // botão pressionado?
    bool _pulling = false;                               // coroutine ativa?
    Collider _currentCol;                                // collider do alvo atual
    XRGrabInteractable _currentGrab;                     // XRGrabInteractable (se existir)
    Rigidbody _currentRb;                                // rb do alvo (se existir)
    bool _hadGravity;

    void Reset()
    {
        directInteractor = GetComponentInChildren<XRDirectInteractor>();
        if (!pullTarget && directInteractor) pullTarget = directInteractor.attachTransform;
    }

    void OnEnable()
    {
        if (pullAction)
        {
            pullAction.action.performed += OnPullPerformed;  
            pullAction.action.canceled  += OnPullCanceled;   
        }
    }
    void OnDisable()
    {
        if (pullAction)
        {
            pullAction.action.performed -= OnPullPerformed;
            pullAction.action.canceled  -= OnPullCanceled;
        }
        StopPullCoroutineIfAny();
        RestorePhysics();
        ClearCurrent();
    }

    // ------ INPUT ------

    void OnPullPerformed(InputAction.CallbackContext _)
    {
        _pullHeld = true;

        if (_pulling) return;
        if (!directInteractor || directInteractor.hasSelection) return;

        var col = FindNearestColliderCandidate();
        if (!col) return;

        StartCoroutine(PullAndMaybeGrab(col));
    }

    void OnPullCanceled(InputAction.CallbackContext _)
    {
        _pullHeld = false;

        if (directInteractor && directInteractor.hasSelection)
        {
            directInteractor.EndManualInteraction(); 
        }

        StopPullCoroutineIfAny();
        RestorePhysics();
        ClearCurrent();
    }

    // ------ BUSCA DO ALVO (qualquer Collider) ------

    Collider FindNearestColliderCandidate()
    {
        Vector3 origin = transform.position;
        Vector3 fwd = transform.forward;

        var hits = Physics.SphereCastAll(
            origin, searchRadius, fwd, maxDistance,
            candidateMask, QueryTriggerInteraction.Ignore);

        // Ordena por distância real do centro da mão
        var ordered = hits
            .Select(h => h.collider)
            .Where(c => c != null && c.enabled && c.gameObject.activeInHierarchy)
            .Distinct()
            .OrderBy(c => Vector3.Distance(origin, c.bounds.center));

        foreach (var col in ordered)
        {
            // Se exigir grabbable, verifica XRGrabInteractable
            if (requireGrabbable)
            {
                if (!col.GetComponentInParent<XRGrabInteractable>())
                    continue;
            }

            // Linha de visão desobstruída até o centro do collider
            Vector3 targetPoint = col.bounds.center;
            Vector3 dir = targetPoint - origin;
            float dist = dir.magnitude;

            if (Physics.Raycast(origin, dir.normalized, out RaycastHit hit, dist,
                                obstructionMask, QueryTriggerInteraction.Ignore))
            {
                // Se bateu em algo que NÃO é o próprio alvo (ou um irmão com XRGrabInteractable),
                // considera obstruído.
                var hitRoot = hit.collider.transform.root;
                var colRoot = col.transform.root;
                if (hitRoot != colRoot)
                {
                    // Porém, se o hit também fizer parte de um mesmo objeto grabbable, deixa passar
                    if (!hit.collider.GetComponentInParent<XRGrabInteractable>() ||
                        hitRoot != colRoot)
                        continue;
                }
            }

            return col;
        }

        return null;
    }

    // ------ COROUTINE DE PUXAR E (se possível) PEGAR ------

    IEnumerator PullAndMaybeGrab(Collider col)
    {
        _pulling = true;

        _currentCol  = col;
        _currentGrab = col.GetComponentInParent<XRGrabInteractable>();
        _currentRb   = col.attachedRigidbody;

        if (_currentRb)
        {
            _hadGravity = _currentRb.useGravity;
            _currentRb.useGravity   = false;
            _currentRb.isKinematic  = false;
            _currentRb.linearVelocity   = Vector3.zero;
            _currentRb.angularVelocity  = Vector3.zero;
        }

        Transform t = col.transform;

        while (_pullHeld && _currentCol && pullTarget &&
               Vector3.Distance(t.position, pullTarget.position) > 0.06f)
        {
            if (_currentRb)
            {
                Vector3 next = Vector3.Lerp(t.position,
                                            pullTarget.position,
                                            Time.deltaTime * pullSpeed);
                _currentRb.MovePosition(next);
                _currentRb.linearVelocity  = Vector3.zero;
                _currentRb.angularVelocity = Vector3.zero;
            }
            else
            {
                t.position = Vector3.Lerp(t.position,
                                          pullTarget.position,
                                          Time.deltaTime * pullSpeed);
            }

            // se já pegou algo manualmente, encerra o loop
            if (directInteractor && directInteractor.hasSelection) break;

            yield return null;
        }

        // Se ainda segurando, e existe XRGrabInteractable, tenta "grudar" na mão
        if (_pullHeld && _currentGrab && directInteractor && !directInteractor.hasSelection)
        {
            directInteractor.StartManualInteraction(_currentGrab);
        }

        _pulling = false;
    }

    // ------ UTILS ------

    void StopPullCoroutineIfAny()
    {
        if (_pulling)
        {
            StopAllCoroutines();
            _pulling = false;
        }
    }

    void RestorePhysics()
    {
        if (_currentRb)
        {
            _currentRb.useGravity = _hadGravity;
            _currentRb.linearVelocity  = Vector3.zero;
            _currentRb.angularVelocity = Vector3.zero;
        }
    }

    void ClearCurrent()
    {
        _currentCol  = null;
        _currentGrab = null;
        _currentRb   = null;
    }
}