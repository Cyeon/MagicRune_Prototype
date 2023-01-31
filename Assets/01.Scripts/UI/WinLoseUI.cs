using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinLoseUI : MonoBehaviour
{
    [Header("GameObjects")]
    public GameObject background = null;
    public TMP_Text winLoseText = null;

    [Header("Buttons")]
    public Button restartButton = null;
    public Button quitButton = null;

    private void Awake()
    {
        restartButton.onClick.AddListener(() => ClickRestart());
        quitButton.onClick.AddListener(() => ClickQuit());
        EventManager.StartListening(Define.GAME_LOSE, () => winLoseText.text = "LOSE");
        EventManager.StartListening(Define.GAME_WIN, () => winLoseText.text = "WIN");
        EventManager.StartListening(Define.GAME_END, () => background.SetActive(true));
    }

    private void ClickRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void ClickQuit()
    {
        Application.Quit();
    }

    private void OnDestroy()
    {
        EventManager.StopListening(Define.GAME_LOSE, () => winLoseText.text = "LOSE");
        EventManager.StopListening(Define.GAME_WIN, () => winLoseText.text = "WIN");
        EventManager.StopListening(Define.GAME_END, () => background.SetActive(true));
    }
}