using UnityEditor;
using UnityEditor.SceneManagement;

namespace Editor
{
    public static class Config
    {
        [MenuItem("Open Scene/Loading Scene &1")]
        public static void OpenSceneStartLoading()
        {
            string localPath = "Assets/Scenes/LoadingScene.unity";
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene(localPath);
        }
    }
}