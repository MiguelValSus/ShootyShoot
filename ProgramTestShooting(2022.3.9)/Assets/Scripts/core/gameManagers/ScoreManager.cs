using core;
using UnityEngine;
using UnityEngine.UI;
using static Enemy;

public class ScoreManager : SingletonBase<ScoreManager>
{
    [Header("Scoring")]
    public int  m_game_score = 0;
    public Text m_stage_score_text;
    [Header("Defeated enemies")]
    public int EnemyAlphaCount;
    public int EnemyDeltaCount;
    public int EnemyGammaCount;
    public int EnemyTanksCount;
    public int PlasmaWallCount;
    public int EnemyMiniXCount;
    public int EnemyGigaXCount;

    #region Initialization
    private void Awake()
    {
        InitScoring();
    }

    private void OnDestroy() { }

    private void InitScoring()
    {
        Instance = this;
    }
    #endregion

    #region Scoring
    public void ResetScore()
    {
        m_game_score = 0;
        RefreshScore();
    }

    public void AddScore(int a_value, EnemyType enemyType)
    {
        m_game_score += a_value;
        Mathf.Clamp(m_game_score, 0, 9999);
        RefreshScore();
        // Keep track of defeated enemies
        AddEnemyToScoreCount(enemyType);
    }

    public void RefreshScore()
    {
        if (m_stage_score_text != null) {
            m_stage_score_text.text = "Score: " + m_game_score;
        }
    }

    private void AddEnemyToScoreCount(EnemyType enemyType)
    {
        switch (enemyType)
        {
            default:
            case EnemyType.ALPHA:
                ++EnemyAlphaCount;
                break;
            case EnemyType.DELTA:
                ++EnemyDeltaCount;
                break;
            case EnemyType.GAMMA:
                ++EnemyGammaCount;
                break;
            case EnemyType.TANKS:
                ++EnemyTanksCount;
                break;
            case EnemyType.PLASMA:
                ++PlasmaWallCount;
                break;
            case EnemyType.CUBEX:
                ++EnemyMiniXCount;
                break;
            case EnemyType.BOSSX:
                ++EnemyGigaXCount;
                break;
        }
    }
    #endregion
}
