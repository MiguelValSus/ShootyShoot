using Player;
using core.config;
using UnityEngine;
using UnityEngine.UI;
using core.Audio;
using UnityEngine.SceneManagement;

namespace core.gameManagers {
    public class GameManager : SingletonBase<GameManager> {
        
        #region Variables
        [Header("Player")]
        public GameObject Player;
        public PlayerController PController;
        public RectTransform HealthBar;
        public RectTransform ShieldBar;
        public bool GameOver;

        [Header("Main Camera")]
        public Camera MainCamera;

        [Header("Menus")]
        public MenusManager Menu;
        public bool Paused;

        [Header("Screen Layout")]
        public Transform m_stage_transform;
        public Texture2D MouseCrosshair;

        [Header("Power-Ups")]
        public PowerUpsManager PowerUpManager;

        [Header("Pools")]
        public PoolManager PoolManager;
        public GameObject  FiredBulletsHolder;

        [Header("Prefabs")]
        public PlayerController m_player_prefab;
        public Vector3 PlayerSpawnPos = new Vector3(0, -6, 0);
        //public EnemySpawner m_prefab_enemy_spawner;

        [Header("Waves control")]
        public StageSequencer WavesSequencer;
        public WavesManager WavesManager;
        public GroundTroopsSpawner GroundTroopsSpawner;

        private bool _stageEnded;
        #endregion

        #region Initialization
        private void Awake() {
            Settings.SetAspect();
            Settings.SetFramerate();
            Settings.SetMouseVisibility(MouseCrosshair);
            InitGameState();
        }

        private void Start() {
            //Call the Main Menu logic, for now
            Menu.ShowMainMenu();
        }

        private void OnDestroy() { }

        private void InitGameState() {
            Instance = this;
            //Load savegame data, if any
        }

        private void SetupStage()
        {
            //Set score to default value
            ScoreManager.Instance.ResetScore();

            //Create player
            SpawnPlayer(PlayerSpawnPos);

            //Start the ^Power-Up management
            PowerUpManager.enabled = true;

            //Commence enemy spawning
            SpawnEnemies();

            //Crank up the music!
            AudioManager.Instance.PlaySoundtrack();
        }

        private void CleanupStage()
        {
            for (var n = 0; n < m_stage_transform.childCount; ++n)
            {
                Transform temp = m_stage_transform.GetChild(n);
                Destroy(temp.gameObject);
            }
        }

        public void LaunchStage()
        {
            Menu.UnpauseScrolling();
            Settings.SetMouseVisibility(null, false);
            //TODO: We should be handling different stages in a StageManager
            SetupStage();
        }

        public void MarkStageAsEnded()
        {
            _stageEnded = true;
        }
        #endregion

        #region Gameplay
        private void Update() 
        {
            if (GameOver || _stageEnded)
            {
                if (Controls.Interact() || Controls.Retry())
                {
                    RestartGame();
                }
                return;
            }

            if (Controls.Pause()) 
            {                
                Paused = !Paused;
                Menu.Pause(Paused);
            }
        }

        public void SpawnPlayer(Vector3 playerSpawnPos)
        {
            PlayerController player = Instantiate(m_player_prefab, m_stage_transform, false);
            player.transform.position = playerSpawnPos;
            PController = player;
        }

        public void SpawnEnemies()
        {
            WavesSequencer.LaunchStageSequence(WavesSequencer.CurrentStage);
            GroundTroopsSpawner.SendGroundTroopsToBattle();
        }
        #endregion

        #region GameOver
        public void OnGameOver()
        {
            GameOver = true;
            Menu.ShowGameOverScreen();
        }
        #endregion

        #region Command actions
        public void RestartGame() 
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        #endregion
        
        #region Application closure
        private void OnApplicationQuit() 
        {
            ResetDebugSerialization();
        }

        private void ResetDebugSerialization() {
            //--Reset debug values we have serialized in PlayerPrefs--
            PlayerPrefs.DeleteAll();
        }
        #endregion
    }
}