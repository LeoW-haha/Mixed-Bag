using UnityEngine;
using UnityEngine.SceneManagement;

public class startMenuController : MonoBehaviour
{
    public void OnStartClick() {
        SceneManager.LoadScene("SampleScene");
    }
    public void toMainMenu() {
        SceneManager.LoadScene("StartScene");
    }

    public void restartScene() {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
    public void OnExitClick() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
