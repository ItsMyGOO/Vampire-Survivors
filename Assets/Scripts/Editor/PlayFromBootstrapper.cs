using UnityEditor;
using UnityEditor.SceneManagement;

namespace Editor
{
    public static class PlayFromBootstrapper
    {
        private const string BootstrapperScenePath = "Assets/Scene/Bootstrapper.unity";

        [MenuItem("Game/Play From Bootstrapper %F5")]
        public static void PlayFromBootstrapperScene()
        {
            if (EditorApplication.isPlaying)
            {
                EditorApplication.isPlaying = false;
                return;
            }

            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(BootstrapperScenePath);
                EditorApplication.isPlaying = true;
            }
        }

        [MenuItem("Game/Play From Bootstrapper %F5", true)]
        public static bool ValidatePlayFromBootstrapper()
        {
            return !EditorApplication.isCompiling;
        }
    }
}
