using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using NineMensMorris;

public class GameOverUI : MonoBehaviour
{
    public GameObject canvasGameobject;
    public TextMeshProUGUI winLooseText;
    private Game game;

    private void Start()
    {
        game = GameObject.Find("GameManager").GetComponent<GameManager>().Game;
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