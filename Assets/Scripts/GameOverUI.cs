using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public GameObject canvasGameobject;
    public TextMeshProUGUI winLooseText;
    private MillGame game;

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
            winLooseText.text = game.Winner == 1 ? "You WIN!" : "You LOOSE!";
        }
    }

    public void OnRestart()
    {
        var scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.buildIndex);
    }
}