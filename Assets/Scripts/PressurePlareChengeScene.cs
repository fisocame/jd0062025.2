using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
public class PressurePlateChangeScene : MonoBehaviour
{
    [Header("Configuração de Peso")]
    public float requiredMass = 1f;      // massa mínima pra ativar a placa

    [Header("Troca de Cena")]
    public string sceneName;             // nome da cena a ser carregada (tem que estar Build Settings)
    public float delay = 2f;             // tempo de espera antes de mudar de cena

    float totalMass;
    Coroutine loadCoroutine;

    void Awake()
    {
        
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        var rb = other.attachedRigidbody;
        if (rb != null) totalMass += rb.mass;
        Evaluate();
    }

    void OnTriggerExit(Collider other)
    {
        var rb = other.attachedRigidbody;
        if (rb != null) totalMass -= rb.mass;
        Evaluate();
    }

    void Evaluate()
    {
        bool on = totalMass >= requiredMass;

        
        if (on && loadCoroutine == null)
        {
            loadCoroutine = StartCoroutine(LoadSceneAfterDelay());
        }
        
        else if (!on && loadCoroutine != null)
        {
            StopCoroutine(loadCoroutine);
            loadCoroutine = null;
        }
    }

    System.Collections.IEnumerator LoadSceneAfterDelay()
    {
        
        yield return new WaitForSeconds(delay);

        
        if (totalMass >= requiredMass)
        {
            if (!string.IsNullOrEmpty(sceneName))
            {
                SceneManager.LoadScene(sceneName);
            }
            else
            {
                Debug.LogWarning("PressurePlateChangeScene: nenhum nome de cena definido no Inspector.");
            }
        }

        loadCoroutine = null;
    }
}