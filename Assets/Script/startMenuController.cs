using UnityEngine;
using UnityEngine.SceneManagement;

public class startMenuControllerAsh : MonoBehaviour
{
    public GameObject levelSelectMenu;
    public void OnStartClick() {
        SceneManager.LoadScene("TutorialScene");
    }
    public void toMainMenu() {
        SceneManager.LoadScene("StartScene");
    }

    public void openLevelSelect() {
        levelSelectMenu.SetActive(true);
    }

    public void closeLevelSelect() {
        levelSelectMenu.SetActive(false);
    }

    public void loadLevel(string level) {
        SceneManager.LoadScene(level);
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
    void Start() {
        if (levelSelectMenu != null) {
            levelSelectMenu.SetActive(false);
        }
    }
}
