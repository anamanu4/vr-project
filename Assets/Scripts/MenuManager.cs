using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Panel del menú")]
    public GameObject menuPanel;

    [Header("Botones")]
    public Button playButton;
    public Button settingsButton;
    public Button quitButton;

    [Header("Panel de Settings (opcional)")]
    public GameObject settingsPanel;

    private void Start()
    {
        menuPanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);

        playButton.onClick.AddListener(OnPlay);
        quitButton.onClick.AddListener(OnQuit);
        if (settingsButton != null)
            settingsButton.onClick.AddListener(OnSettings);
    }

    private void OnPlay()
    {
        menuPanel.SetActive(false);
        GameManager.Instance.StartGame();
    }

    private void OnSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(!settingsPanel.activeSelf);
    }

    private void OnQuit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}