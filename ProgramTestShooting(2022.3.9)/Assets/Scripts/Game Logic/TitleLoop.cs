using core;
using core.gameManagers;
using UnityEngine;

public class TitleLoop : MonoBehaviour
{
	[Header("Layout")]
	public Transform m_ui_title;
    public GameObject m_ui_gameName,
                      m_ui_keyPress,
                      m_ui_shipHUD;

    #region Initialization
    public void StartGame()
	{
        //default start
        Debug.Log($"Start game");
        SetupTitle();
    }

    void SetupTitle()
    {
        m_ui_title.gameObject.SetActive(true);
        HideGameHUD();
    }

    void CleanupTitle()
    {
        m_ui_gameName.SetActive(false);
        m_ui_keyPress.SetActive(false);
    }

    void HideTitle()
    {
        m_ui_title.gameObject.SetActive(false);
        m_ui_shipHUD.SetActive(true);
    }

    public void HideGameHUD()
    {
        m_ui_shipHUD.SetActive(false);
    }
    #endregion

    #region Main Menu interaction
    /// <summary>
    /// Title loop
    /// </summary>
    private void Update()
	{
        if (Controls.Interact())
        {
            if (!GameManager.Instance.Menu.MissionBriefing.activeSelf) {

                CleanupTitle();
                if (PlayerPrefs.GetInt("SeenControls") == 1) {
                    BeginMission();
                } else {
                    GameManager.Instance.Menu.ShowMission();
                }
            } else {
                BeginMission();
            }
        }
    }

    private void BeginMission()
    {
        HideTitle();
        GameManager.Instance.Menu.HideMission();
        GameManager.Instance.LaunchStage();
    }
    #endregion
}