using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public GameObject canvasGameobject;
    public TextMeshProUGUI winLooseText;
    private Game game;

    private void Start()
    {
        var board = GameObject.Find("Board").GetComponent<Board>();
        game = board.Game;
    }

    private void Update()
    {
        if (game.IsGameOver)
        {
            canvasGameobject.SetActive(true);
            winLooseText.text = game.Winner == 1 ? "You WON!" : "You LOST!";
        }
    }

    public void OnRestart()
    {
        var scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.buildIndex);
    }
}