using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ButtonScene : MonoBehaviour
{
    bool _loading;

    [Tooltip("0 = Single (substitui a cena atual). Additive carrega por cima.")]
    public LoadSceneMode loadMode = LoadSceneMode.Single;

    public void MudarCena(string nomeCena)
    {
        if (_loading) return;
        if (!CenaExiste(nomeCena))
        {
            Debug.LogError($"[ButtonHandler] Cena '{nomeCena}' não está no Build Settings.");
            return;
        }
        StartCoroutine(LoadAsync(nomeCena));
    }

    IEnumerator LoadAsync(string nomeCena)
    {
        _loading = true;

        // (opcional) pequeno fade-out aqui
        // FadeManager.Instance?.FadeOut(0.25f);

        var op = SceneManager.LoadSceneAsync(nomeCena, loadMode);
        op.allowSceneActivation = true;
        while (!op.isDone) yield return null;

        _loading = false;
    }

    bool CenaExiste(string nome)
    {
        int count = SceneManager.sceneCountInBuildSettings;
        for (int i = 0; i < count; i++)
        {
            var path = SceneUtility.GetScenePathByBuildIndex(i);
            var sceneName = System.IO.Path.GetFileNameWithoutExtension(path);
            if (sceneName == nome) return true;
        }
        return false;
    }
}