using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] private Button m_playButton;
    [SerializeField] private Button m_creditButton;
    [SerializeField] private Button m_quitButton;
    [SerializeField] private Button m_returnButton;
    [SerializeField] private GameObject m_panelMain;
    [SerializeField] private GameObject m_panelCredits;

    private void Awake()
    {
        m_playButton.onClick.AddListener(PlayGame);
        m_creditButton.onClick.AddListener(GoToCredits);
        m_quitButton.onClick.AddListener(QuitGame);
        m_returnButton.onClick.AddListener(GoToMain);
    }


    public void PlayGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }


    public void GoToCredits()
    {
        m_panelCredits.SetActive(true);
        m_panelMain.SetActive(false);
    }

    public void GoToMain()
    {
        m_panelCredits.SetActive(false);
        m_panelMain.SetActive(true);
    }

}
