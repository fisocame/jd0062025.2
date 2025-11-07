using UnityEngine;
using Unity.XR.CoreUtils; 

public class TeleportButton : MonoBehaviour
{
    [Header("Destino do teleporte")]
    public Transform destination;

    [Header("Referência (opcional)")]
    public XROrigin xrOrigin;

    void Awake()
    {
        // Encontra automaticamente o XROrigin se não for setado
        if (xrOrigin == null)
            xrOrigin = FindFirstObjectByType<XROrigin>();
    }

    // Este é o método que você vai puxar no botão UI
    public void TeleportToDestination()
    {
        if (xrOrigin == null || destination == null)
        {
            Debug.LogWarning("TeleportUI: faltando referência ao xrOrigin ou destination.");
            return;
        }

        // Move o rig para o destino
        xrOrigin.MoveCameraToWorldLocation(destination.position);

        // Ajusta rotação do rig
        xrOrigin.Origin.transform.rotation = destination.rotation;

        Debug.Log("Teleportado para " + destination.name);
    }
}