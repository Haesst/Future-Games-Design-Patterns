using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject m_MainMenu;
    [SerializeField] private GameObject m_InGameUI;
    [SerializeField] private MapGenerator m_MapGenerator;

    [SerializeField] private TMP_Text m_WaveText;
    [SerializeField] private TMP_Text m_PlayerBaseHealthText;

    private EnemyBase m_EnemyBase;
    private PlayerBase m_PlayerBase;

    public void Awake()
    {
        GameTime.m_IsPaused = true;
        m_MainMenu.SetActive(true);
        m_InGameUI.SetActive(false);

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
        m_MapGenerator.OnPlayerBaseLoaded -= PlayerBaseLoaded;
    }

    public void LoadMap(int mapId)
    {
        m_MapGenerator.GenerateMap(mapId);
        ToggleMenu();
        GameTime.m_IsPaused = false;
    }

    public void ToggleMenu()
    {
        m_MainMenu.SetActive(!m_MainMenu.activeSelf);
        m_InGameUI.SetActive(!m_InGameUI.activeSelf);
        GameTime.m_IsPaused = !GameTime.m_IsPaused;
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
        enemyBase.OnWaveChange += UpdateWaveText;
    }

    private void PlayerBaseLoaded(PlayerBase playerBase)
    {
        m_PlayerBase = playerBase;
        playerBase.OnBaseHealthChanged += UpdatePlayerBaseHealth;
    }

    private void UpdateWaveText(int wave)
    {
        m_WaveText.text = $"Current Wave: {wave}";
    }

    private void UpdatePlayerBaseHealth(float health)
    {
        m_PlayerBaseHealthText.text = $"Base Health: {health}";
    }
}
