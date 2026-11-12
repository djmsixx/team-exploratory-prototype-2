using UnityEngine;
using UnityEngine.SceneManagement;

public static class StorybookSceneLoader
{
    public const string IntroductionScene = "IntroductionScene";
    public const string ClimaxScene = "ClimaxScene";

    public static void LoadStorybook(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogError("A storybook scene name must be provided.");
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError($"Storybook scene '{sceneName}' is not included in the build settings.");
            return;
        }

        SceneManager.LoadScene(sceneName);
    }
}
