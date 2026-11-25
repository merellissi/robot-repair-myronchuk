using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class StartMenu : MonoBehaviour
{
    public string levelName = "MyLevel";

    void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        Button startButton = root.Q<Button>("StartButton");
        startButton.clicked += StartGame;
    }

    void StartGame()
    {
        SceneManager.LoadScene(levelName);
    }
}