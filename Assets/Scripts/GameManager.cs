using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    InputActions controls;
    public static GameManager GM;
    [Header("--- Main Menu --- \n")]
    [SerializeField] private bool titleScene = false;
    [SerializeField] private Button firstSelected = null;
    [SerializeField] private GameObject quitButton = null;
    [SerializeField] private List<GameObject> openedPanels = new List<GameObject>();

    [Header("-- Game UI -- \n")]
    [SerializeField] private GameObject gamePausedPanel = null;
    [SerializeField] private GameObject victoryPanel = null;
    [SerializeField] private GameObject gameOverPanel = null;
    [SerializeField] private Text scoreText = null;
    [SerializeField] private Text missileText = null;
    [SerializeField] private Text levelText = null;
    [SerializeField] private Image progressBar = null;

    [System.Serializable]
    public struct EnemyCounter
    {
        public int GreenCount;
        public Text GreenCountText;
        public int BlueCount;
        public Text BlueCountText;
        public int RedCount;
        public Text RedCountText;
        public int PurpleCount;
        public Text PurpleCountText;
        public int GoldenCount;
        public Text GoldenCountText;
    }
    public EnemyCounter enemyCounter;

    [Header("--- Gameplay --- \n")]
    public string levelAfterVictory;
    [Header (" -- Aircraft -- \n")]
    public Aircraft airCraft = null;
    [SerializeField] private float flightSpeed = 5.0f;
    public int numberOfMissiles = 20;   
    [HideInInspector]
    public int waveNumber = 0;
    [HideInInspector]
    public GameObject currentWave = null;
    [Header(" -- Waves -- ")]
    public int numberOfWaves = 4;    
    [SerializeField] private float waveEverySec = 5;
    [HideInInspector]
    public bool canSpawnWave = true;
    private int waveIndex = 0;
    [System.Serializable]
    public struct WaveStruct
    {
        public GameObject wavePrefab;
        [Range (0,1f)]
        public float enemyPositionScreenHeightPercent;
        public bool canShoot;
        public float shootEverySec;
    }
    public List<WaveStruct> waveParameters = new List<WaveStruct>();
    [SerializeField] private Transform waveSpawnPosition = null;
    
    public int score = 0;
    public float enemyPositionScreenHeightRatio = 0.8f;
    [HideInInspector]
    public float enemyDistance = 0f;
    
    [Header ("-- Game Settings -- \n")]
    [SerializeField] private float screenBorders = 50.0f;

    private Vector2 movement;
    private int startHealth = 0;

    private void Awake()
    {
        controls = new InputActions();
        GM = GetComponent<GameManager>();

        if (airCraft != null)
            startHealth = airCraft.GetHealth();

        if (Time.timeScale != 1)
            Time.timeScale = 1;

        enemyDistance =
            Camera.main.scaledPixelHeight * enemyPositionScreenHeightRatio;

        if (levelText != null)
        {
            levelText.text = SceneManager.GetActiveScene().name;
        }

        bindInputActions();
        displayQuitWhenAppropriate();
        UpdateMissiles();
        UpdateHealth();
        initializeEnemyScore();
        UpdateEnemyScore();
    }

    private void Start()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            if (firstSelected != null)
            {
                firstSelected.Select();
            }
        }
    }

    private void Update()
    {
        if (!titleScene)
        {
            moveAirCraft();

            if (currentWave == null)
            {
                spawnWaves();
            }
        }
        else
        {
            if (EventSystem.current.currentSelectedGameObject == null)
            {
                if (firstSelected != null)
                {
                    firstSelected.Select();
                }
            }
        }
        
    }

    public void OpenPanel(GameObject UIPanel)
    {
        if (UIPanel != null)
        {
            if (!UIPanel.activeSelf)
                UIPanel.SetActive(true);

            if (!openedPanels.Contains(UIPanel))
                openedPanels.Add(UIPanel);
        }

        controls.Player.Movement.Disable();
        controls.UI.Submit.Disable();
    }

    private void closeLastPanel()
    {
        if (openedPanels.Count > 0)
        {
            int panelCount = openedPanels.Count;
            if (openedPanels[panelCount - 1] != null)
            {
                if (openedPanels[panelCount - 1].activeSelf)
                {
                    openedPanels[panelCount - 1].SetActive(false);
                    openedPanels.Remove(openedPanels[panelCount - 1]);
                }
            }
        }

        if (!controls.Player.Movement.enabled)
            controls.Player.Movement.Enable();

        if (!controls.UI.Submit.enabled)
            controls.UI.Submit.Enable();

        if (Time.timeScale == 0)
            Time.timeScale = 1;

        Debug.Log(nameof(closeLastPanel));
    }

    private void bindInputActions()
    {

        if (controls != null)
        {
            if (airCraft != null)
            {
                controls.Player.Fire.performed += ctx => airCraft.FireMissile();
                
            }
            
            controls.UI.Cancel.performed += ctx => closeLastPanel();
            controls.UI.Pause.performed += ctx => GamePause();
            controls.Player.Movement.performed += ctx =>
            {
                movement = controls.Player.Movement.ReadValue<Vector2>();
            };
        } 
    }

    private void displayQuitWhenAppropriate()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer ||
            Application.platform == RuntimePlatform.OSXEditor ||
            Application.platform == RuntimePlatform.WindowsEditor)
        {
            if (quitButton != null)
            {
                if (quitButton.activeSelf)
                    quitButton.SetActive(false);
            }

        }
    }
    private void initializeEnemyScore()
    {
        enemyCounter.GreenCount = 0;
        enemyCounter.BlueCount = 0;
        enemyCounter.RedCount = 0;
        enemyCounter.PurpleCount = 0;
        enemyCounter.GoldenCount = 0;     
    }

    private void spawnWaves()
    {
        if (canSpawnWave && waveNumber < numberOfWaves)
        {
            if (waveIndex >= waveParameters.Count)
                waveIndex = 0;

            enemyDistance =
                Camera.main.scaledPixelHeight *
                waveParameters[waveIndex].enemyPositionScreenHeightPercent;

            Invoke(nameof(spawnWave), waveEverySec);
            canSpawnWave = false;
        }
    }

    private void spawnWave()
    {      
        if (waveSpawnPosition != null && waveParameters.Count > 0)
        {
            if (waveIndex >= waveParameters.Count)
                waveIndex = 0;

            if (waveParameters[waveIndex].wavePrefab != null)
                Instantiate(waveParameters[waveIndex].wavePrefab, waveSpawnPosition.position, Quaternion.identity);
            if (waveParameters[waveIndex].canShoot)
            {
                foreach (GameObject enemy in EnemyWave.EW.enemies)
                {
                    Enemy _enemy = enemy.GetComponent<Enemy>();
                    _enemy.canShoot = true;
                    if (_enemy.enemytype != Enemy.EnemyType.Golden)
                        _enemy.shootEverySec = waveParameters[waveIndex].shootEverySec;
                }            
            }

            waveIndex++;
        }
    }

    public void UpdateEnemyScore()
    {
        if (enemyCounter.GreenCountText != null)
        {
            enemyCounter.GreenCountText.text = $"x {enemyCounter.GreenCount}";
        }

        if (enemyCounter.BlueCountText != null)
        {
            enemyCounter.BlueCountText.text = $"x {enemyCounter.BlueCount}";
        }

        if (enemyCounter.RedCountText != null)
        {
            enemyCounter.RedCountText.text = $"x {enemyCounter.RedCount}";
        }

        if (enemyCounter.PurpleCountText != null)
        {
            enemyCounter.PurpleCountText.text = $"x {enemyCounter.PurpleCountText}";
        }

        if (enemyCounter.GoldenCountText != null)
        {
            enemyCounter.GoldenCountText.text = $"x {enemyCounter.GoldenCountText}";
        }
    }

    public void UpdateHealth()
    {
        if (progressBar != null)
        {
            progressBar.fillAmount = (float)airCraft.GetHealth() / startHealth;
        }
    }
    public void UpdateScore()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {score}";
    }

    public void UpdateMissiles()
    {
        if (missileText != null)
            missileText.text = $"{numberOfMissiles}";
    }

    public void GameOver()
    {
        StartCoroutine(nameof(gameOver));
    }

    public void Victory()
    {
        UpdateEnemyScore();
        Invoke(nameof(victory), 2f);   
    }

    public void LoadVictoryScene()
    {
        SceneManager.LoadScene(levelAfterVictory);
    }

    private void victory()
    {
        if (victoryPanel != null)
        {
            if (!victoryPanel.activeSelf)
                victoryPanel.SetActive(true);
            controls.UI.Pause.Disable();
        }
    }

    IEnumerator gameOver()
    {
        yield return new WaitForSeconds(2f);
        if (gameOverPanel != null)
        {
            if (!gameOverPanel.activeSelf)
                gameOverPanel.SetActive(true);
            controls.UI.Pause.Disable();
        }
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(0);
    }

    public void GamePause()
    {
        if (gamePausedPanel != null)
        {
            if (!gamePausedPanel.activeSelf)
            {
                gamePausedPanel.SetActive(true);
                if (Time.timeScale != 0)
                    Time.timeScale = 0;

                if (!openedPanels.Contains(gamePausedPanel))
                    openedPanels.Add(gamePausedPanel);
            }
            else
            {
                gamePausedPanel.SetActive(false);

                if (Time.timeScale != 1)
                    Time.timeScale = 1;

                if (openedPanels.Contains(gamePausedPanel))
                    openedPanels.Remove(gamePausedPanel);
            }           
        }
    }

    private void moveAirCraft()
    {
        if (airCraft != null)
        {
            Vector3 airCraftPosition = airCraft.transform.position;
            float screenWidth = Camera.main.scaledPixelWidth;
            float screenHeight = Camera.main.scaledPixelHeight;
            Vector3 screenCoordinates = Camera.main.WorldToScreenPoint(airCraftPosition);

            if (movement.x != 0 || movement.y != 0)
            {
                airCraftPosition.x += movement.x * flightSpeed * Time.deltaTime;
                airCraftPosition.y += movement.y * flightSpeed * Time.deltaTime;

                screenCoordinates = Camera.main.WorldToScreenPoint(airCraftPosition);

                if (screenCoordinates.x > 0f + screenBorders &&
                    screenCoordinates.x < screenWidth - screenBorders &&
                    screenCoordinates.y > 0f + screenBorders &&
                    screenCoordinates.y < screenHeight - screenBorders)
                {
                    airCraft.transform.position = airCraftPosition;
                }       
            }
        }
    }

    public void DestroyWave()
    {
        Destroy(currentWave);
        currentWave = null;
        canSpawnWave = true;
    }

    public void LoadLevel(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void Quit()
    {
        Application.Quit();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
}
