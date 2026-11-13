using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField]
    private GameObject playerPrefab;
    private GameObject spawnedPlayer;
    private Vector2 playerSpawnPosition = Vector2.left * 1.5f;
    [SerializeField]
    private GameObject[] Waves;
    private GameObject currentWaveObject;

    [SerializeField]
    private TextMeshProUGUI shotsText;

    [SerializeField]
    private GameObject youWinUI;

    private int currentWave = 0;

    private int activeEnemies = 0;

    int currentShots = 0;

    [SerializeField]
    private AudioMixerSnapshot[] musicSnapshots;
    private void Start()
    {
        if(Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        ResetGame();
    }

    void SpawnNextWave()
    {
        Destroy(currentWaveObject);
        if (currentWave == Waves.Length)
        {
            youWinUI.SetActive(true);
            return;
        }
        currentWaveObject = Instantiate(Waves[currentWave], null);
        switch (currentWave)
        {
            case 2:
                musicSnapshots[1].TransitionTo(0.2f);
                break;
            case 4:
                musicSnapshots[2].TransitionTo(0.2f);
                break;
        }
        currentWave++;
    }

    public void NotifyEnemySpawn()
    {
        activeEnemies++;
    }
    public void NotifyEnemyDeath()
    {
        activeEnemies--;
        if(activeEnemies == 0 )
            Invoke("SpawnNextWave", 4.0f);
    }
    public void NotifyPlayerDeath()
    {
        Invoke("ResetGame", 4.0f);
    }

    public void NotifyPlayerShot()
    {
        currentShots++;
        UpdateUI();
    }

    public void ResetGame()
    {
        Destroy(spawnedPlayer);
        musicSnapshots[0].TransitionTo(1.0f);
        spawnedPlayer = Instantiate(playerPrefab, playerSpawnPosition, Quaternion.identity, null);
        currentWave = 0;
        currentShots = 0;
        activeEnemies = 0;
        UpdateUI();
        SpawnNextWave();
    }

    void UpdateUI()
    {
        shotsText.text = "SHOTS: " + currentShots.ToString();
    }
}
