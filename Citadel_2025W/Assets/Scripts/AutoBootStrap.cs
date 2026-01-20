using UnityEngine;
using UnityEngine.SceneManagement;

public static class AutoBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        SceneManager.LoadScene("BootStrap", LoadSceneMode.Additive);
    }
}