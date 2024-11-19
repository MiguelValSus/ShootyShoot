using core.Audio;
using core.config;
using core.gameManagers;
using States.Main;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenusManager : MonoBehaviour
{
    [Header("Title Screen")]
    public TitleLoop  MainTitle;
    public GameObject MissionBriefing;
    [Header("Game menus")]
    public GameObject PauseUI;
    public GameObject GameOverUI;
    public List<MoveMaterialUVs> BackgroundScrollMaterials;
    [Header("Alerts")]
    public Canvas BossWarningUI;
    public GameObject AlertText;
    public Image AlertPanel;
    [Header("Scoreboard")]
    public GameObject StageClearUI;
    public GameObject AlphaEnemyIcon;
    public GameObject DeltaEnemyIcon;
    public GameObject GammaEnemyIcon;
    public GameObject TanksEnemyIcon;
    public GameObject CubeXEnemyIcon;
    public GameObject BossXEnemyIcon;
    public Text AlphaEnemyAmount;
    public Text DeltaEnemyAmount;
    public Text GammaEnemyAmount;
    public Text TanksEnemyAmount;
    public Text CubeXEnemyAmount;
    public Text BossXEnemyAmount;
    public Text FinalScore;
    public TextPulsate ScorePulse;

    private bool _scoreRecap;

    #region Main Menu
    public void ShowMainMenu()
    {
        MainTitle.StartGame();
    }

    public void ShowMission()
    {
        MissionBriefing.SetActive(true);
    }

    public void HideMission()
    {
        MissionBriefing.SetActive(false);
        PlayerPrefs.SetInt("SeenControls", 1);
    }
    #endregion

    #region Pause
    public void Pause(bool pause)
    {
        if (MainTitle.m_ui_title.gameObject.activeSelf || _scoreRecap) return;
        PauseUI.SetActive(pause);
        Settings.SetGameSpeed(pause ? 0 : 1);
    }

    public void UnpauseScrolling()
    {
        for (var m = 0; m < BackgroundScrollMaterials.Count; ++m)
        {
            BackgroundScrollMaterials[m].AnimateUv_YAxis = true;
        }
    }
    #endregion

    #region Alerts
    public IEnumerator ShowAlertWarning()
    {
        var startTime = Time.time;
        BossWarningUI.gameObject.SetActive(true);

        // Fade in the Alert Panel
        while (Time.time < startTime + .1f)
        {
            var panelColor = AlertPanel.color;
            panelColor.a += Time.deltaTime*5.5f;
            AlertPanel.color = panelColor;
            yield return null;
        }
        AlertText.SetActive(true);

        // Wait and hide the Alert
        startTime = Time.time;
        while (Time.time < startTime + 2f)
        {
            yield return null;
        }
        BossWarningUI.enabled = false;

        // Wait and hide the panel
        startTime = Time.time;
        while (Time.time < startTime + .5f)
        {
            yield return null;
        }
        BossWarningUI.gameObject.SetActive(false);
    }
    #endregion

    #region Score
    public void OnStageClear()
    {
        _scoreRecap = true;
        MainTitle.HideGameHUD();
        StartCoroutine(ShowStageScore());
        AudioManager.Instance.CrossfadeToNewMusic(AudioManager.Instance.StageClearMusic);
    }

    private IEnumerator ShowStageScore()
    {
        var scoreBoard = ScoreManager.Instance;
        var startTime = Time.time;
        while (Time.time < startTime + .6f)
        {
            yield return null;
        }
        // Send off the Player's ship
        GameManager.Instance.PController.SetState(new State_EndStage());
        startTime = Time.time;
        while (Time.time < startTime + .75f)
        {
            yield return null;
        }
        // Display final Score recap
        StageClearUI.SetActive(true);
        startTime = Time.time;
        while (Time.time < startTime + .5f)
        {
            yield return null;
        }
        // Show Alpha enemy type death toll
        AlphaEnemyIcon.SetActive(true);
        AlphaEnemyAmount.text = scoreBoard.EnemyAlphaCount.ToString();
        startTime = Time.time;
        while (Time.time < startTime + .35f)
        {
            yield return null;
        }
        // Show Delta enemy type death toll
        DeltaEnemyIcon.SetActive(true);
        DeltaEnemyAmount.text = scoreBoard.EnemyDeltaCount.ToString();
        startTime = Time.time;
        while (Time.time < startTime + .35f)
        {
            yield return null;
        }
        // Show Gamma enemy type death toll
        GammaEnemyIcon.SetActive(true);
        GammaEnemyAmount.text = scoreBoard.EnemyGammaCount.ToString();
        startTime = Time.time;
        while (Time.time < startTime + .35f)
        {
            yield return null;
        }
        // Show Tanks enemy type death toll
        TanksEnemyIcon.SetActive(true);
        TanksEnemyAmount.text = scoreBoard.EnemyTanksCount.ToString();
        startTime = Time.time;
        while (Time.time < startTime + .35f)
        {
            yield return null;
        }
        // Show MiniX enemy type death toll
        CubeXEnemyIcon.SetActive(true);
        CubeXEnemyAmount.text = scoreBoard.EnemyMiniXCount.ToString();
        startTime = Time.time;
        while (Time.time < startTime + .35f)
        {
            yield return null;
        }
        // Show GigaX enemy type death toll
        BossXEnemyIcon.SetActive(true);
        BossXEnemyAmount.text = scoreBoard.EnemyGigaXCount.ToString();
        startTime = Time.time;
        startTime = Time.time;
        while (Time.time < startTime + .5f)
        {
            yield return null;
        }
        // Show final Score
        FinalScore.gameObject.SetActive(true);
        var elapsedTime = 0f;
        while (elapsedTime < 1.25f)
        {
            var finalScore = Mathf.FloorToInt(Mathf.Lerp(0, ScoreManager.Instance.m_game_score, elapsedTime / 1.25f));
            FinalScore.text = finalScore.ToString();
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        FinalScore.text = ScoreManager.Instance.m_game_score.ToString();
        ScorePulse.enabled = true;
        // Allow controls for Restart
        GameManager.Instance.MarkStageAsEnded();
    }
    #endregion

    #region GameOver
    public void ShowGameOverScreen()
    {
        Pause(false);
        GameOverUI.SetActive(true);
    }
    #endregion
}
