using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject   m_MainMenu = default;
    [SerializeField] private GameObject   m_InGameUI = default;
    [SerializeField] private GameObject   m_EndScreen = default;
    [SerializeField] private MapGenerator m_MapGenerator = default;

    [SerializeField] private TMP_Text     m_WaveText = default;
    [SerializeField] private TMP_Text     m_PlayerBaseHealthText = default;
    [SerializeField] private TMP_Text     m_ResultText = default;

    private EnemyBase                     m_EnemyBase = default;
    private PlayerBase                    m_PlayerBase = default;

    #region Unity Functions
    public void Awake()
    {
        GameTime.IsPaused = true;
        m_MainMenu.SetActive(true);
        m_InGameUI.SetActive(false);
        m_EndScreen.SetActive(false);

        m_MapGenerator.OnEnemyBaseLoaded += EnemyBaseLoaded;
        m_MapGenerator.OnPlayerBaseLoaded += PlayerBaseLoaded;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }
    }

    private void OnDisable()
    {
        m_MapGenerator.OnEnemyBaseLoaded -= EnemyBaseLoaded;
        m_EnemyBase.OnWaveStart -= UpdateCurrentWaveText;
        m_EnemyBase.OnAllWavesComplete -= PlayerWins;

        m_MapGenerator.OnPlayerBaseLoaded -= PlayerBaseLoaded;
        m_PlayerBase.OnBaseHealthChanged -= UpdatePlayerBaseHealth;
        m_PlayerBase.OnBaseDestroyed -= BoxymonWins;
    }

    #endregion Unity Regions

    public void LoadMap(int mapId)
    {
        m_MapGenerator.GenerateMap(mapId);
        ToggleMenu();
        GameTime.IsPaused = false;
    }

    public void ToggleMenu(bool togglePause = true)
    {
        m_MainMenu.SetActive(!m_MainMenu.activeSelf);
        m_InGameUI.SetActive(!m_InGameUI.activeSelf);
        m_EndScreen.SetActive(false);

        if (togglePause)
        {
            GameTime.IsPaused = !GameTime.IsPaused;
        }
    }

    public void QuitApplication()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void EnemyBaseLoaded(EnemyBase enemyBase)
    {
        m_EnemyBase = enemyBase;
        enemyBase.OnWaveStart += UpdateCurrentWaveText;
        enemyBase.OnAllWavesComplete += PlayerWins;
    }

    private void PlayerBaseLoaded(PlayerBase playerBase)
    {
        m_PlayerBase = playerBase;
        playerBase.OnBaseHealthChanged += UpdatePlayerBaseHealth;
        playerBase.OnBaseDestroyed += BoxymonWins;
    }

    private void UpdateCurrentWaveText(int wave)
    {
        m_WaveText.text = $"Current Wave: {wave}";
    }

    private void UpdatePlayerBaseHealth(float health)
    {
        m_PlayerBaseHealthText.text = $"Base Health: {health}";
    }

    private void BoxymonWins()
    {
        m_ResultText.text = "Boxymon Wins!";
        m_EndScreen.SetActive(true);
        GameTime.IsPaused = true;
    }

    private void PlayerWins()
    {
        m_ResultText.text = "Player Wins!";
        m_EndScreen.SetActive(true);
        GameTime.IsPaused = true;
    }
}
