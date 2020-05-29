using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Делегат события изменения количества золота
/// </summary>
public delegate void GoldChanged();

public class GameManager : Singleton<GameManager>
{
    /// <summary>
    /// Событие, вызываемое, когда изменено количество денег
    /// </summary>
    public event GoldChanged Changed;

    /// <summary>
    /// Свойство для кнопки башни
    /// </summary>
    public TowerBtn ClickedBtn { get; set; }

    private int wave = 0;

    private int health = 10;

    public int Health { get => health; set
        {
            health = value;
            if (health <= 0)
            {
                GameOver();
            }
            
            for (int i = 0; i < hearts.Length; i++)
            {
                if (i < (int) health / 2)
                {
                    hearts[i].sprite = fullHeart;
                }
                else if(health%2 != 0){
                    hearts[(int)health/2].sprite = halfFullHeart;
                }
                else
                {
                    hearts[i].sprite = emptyHeart;
                }
            }
        }
    }

    [SerializeField]
    private Image[] hearts;
    [SerializeField]
    private Sprite fullHeart;
    [SerializeField]
    private Sprite halfFullHeart;
    [SerializeField]
    private Sprite emptyHeart;

    [SerializeField]
    private Text WaveText;

    private int monsterHealth = 10;

    [SerializeField]
    private GameObject gameOverMenu;

    [SerializeField]
    private GameObject inGameMenu;
    
    [SerializeField]
    private GameObject optionsMenu;
    
    [SerializeField]
    private GameObject aboutMenu;

    [SerializeField]
    private GameObject upgradePanel;

    [SerializeField]
    private GameObject statsPanel;

    private bool inMenu = false;

    [SerializeField]
    private Text statTxt;

    [SerializeField]
    private Text sellTxt;
    [SerializeField]
    private Text upgradePrice;

    public int Wave { get => wave; set
        {
            wave = value;
            WaveText.text = string.Format("Волна: <color=pink>{0}</color>", wave);
        }
    }
    /// <summary>
    /// Свойство для изменения количество монет
    /// </summary>
    public int Gold { get => gold; 
        set
        {
            goldCurrency.text = value.ToString() + " <color=green>$</color>";
            gold = value;

            OnGoldChanged();
        }
    }
    /// <summary>
    /// Ссылка на goldCurrency (сколько у нас денег)
    /// </summary>
    private int gold;

    [SerializeField]
    private Text goldCurrency;

    public ObjectPool Pool { get; set; }

    /// <summary>
    /// Кнопка начала волны
    /// </summary>
    [SerializeField]
    private GameObject waveBtn;

    /// <summary>
    /// Закончилась волна или  нет
    /// </summary>
    public bool WaveActive
    {
        get
        {
            return activeMonsters.Count > 0;
        }
    }

    private List<Monster> activeMonsters = new List<Monster>();

    /// <summary>
    /// Башня, выбранная в данный момент
    /// </summary>
    private Tower selectedTower;
    

    private bool gameOver = false;

    private void Awake()
    {
        Pool = GetComponent<ObjectPool>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Gold = 50;
    }

    // Update is called once per frame
    void Update()
    {
        HandleEscape();
    }

    /// <summary>
    /// При выборе башни, нажатии на кнопку
    /// </summary>
    /// <param name="towerBtn"></param>
    public void PickTower(TowerBtn towerBtn)
    {
        if(Gold >= towerBtn.Price && !WaveActive)
        {
            this.ClickedBtn = towerBtn;
            Hover.Instance.Activate(towerBtn.Sprite);
        }

    }

    /// <summary>
    /// При покупке башни
    /// </summary>
    public void BuyTower()
    {
        Gold -= ClickedBtn.Price;
        Hover.Instance.Deactivate();
        ClickedBtn = null;
    }

    /// <summary>
    /// При клике на башню выбирает ее
    /// </summary>
    /// <param name="tower"></param>
    public void SelectTower(Tower tower)
    {
        if(selectedTower != null)
        {
            selectedTower.Select();
        }
        selectedTower = tower;
        selectedTower.Select();

        sellTxt.text = (selectedTower.Price / 2) + "$";

        upgradePanel.SetActive(true);
    }

    public void DeselectTower()
    {
        if (selectedTower != null)
        {
            selectedTower.Select();
        }
        upgradePanel.SetActive(false);
        selectedTower = null;

        
    }

    public void UpgradeTower()
    {
        if (selectedTower != null && !WaveActive)
        {
            if (selectedTower.Level <= selectedTower.Upgrades.Length && Gold >= selectedTower.NextUpgrade.Price)
            {
                selectedTower.Upgrade();
            }
        }
    }

    public void ShowStats()
    {
        statsPanel.SetActive(!statsPanel.activeSelf);
    }

    public void ShowUpgradeStats()
    {
        statsPanel.SetActive(!statsPanel.activeSelf);
        UpdateUpgradeTip();
    }

    public void SetTooltipText(string txt)
    {
        statTxt.text = txt;
    }

    public void UpdateUpgradeTip()
    {
        if (selectedTower != null)
        {
            sellTxt.text = (selectedTower.Price / 2).ToString() + "$";
            SetTooltipText(selectedTower.GetStats());

            if (selectedTower.NextUpgrade != null)
            {
                upgradePrice.text = selectedTower.NextUpgrade.Price.ToString() + "$";
            }
            else
            {
                upgradePrice.text = string.Empty;
            }
        }
    }

    /// <summary>
    /// обрабатывает отмену
    /// </summary>
    private void HandleEscape()
    {
        if(Hover.Instance.IsVisible && Input.GetMouseButtonDown(1))
        {
            Hover.Instance.Deactivate();
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {

            if (Hover.Instance.IsVisible)
            {
                DropTower();
            }else if (selectedTower != null)
            {
                DeselectTower();
            }
            else
            {
                ShowIngameMenu();
            }
        }
    }
    
    public void StartWave()
    {
        Wave++;

        waveBtn.SetActive(false);

        StartCoroutine(SpawnWave());
    }
    /// <summary>
    /// Спавнит волну монстров
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawnWave()
    {
        LevelManager.Instance.GeneratePath();
        for (int i = 0; i < wave*2; i++)
        {
            int monsterIndex = Random.Range(0, 5);

            string type = string.Empty;

            int health = monsterHealth;

            switch (monsterIndex)
            {
                case 0:
                    type = "bigBlob";
                    health = monsterHealth + 5;
                    break;
                case 1:
                    type = "blob";
                    break;
                case 2:
                    type = "octopus";
                    break;
                case 3:
                    type = "spider";
                    break;
                case 4:
                    type = "death";
                    health = monsterHealth + 10;
                    break;
            }
            // запрашивает монстра из пула
            Monster monster = Pool.GetObject(type).GetComponent<Monster>();
            monster.Spawn(health);
            if(wave % 5 == 0)
            {
                monsterHealth += 5;
            }

            activeMonsters.Add(monster);
            yield return new WaitForSeconds(2.5f);
        }
        
    }

    public void SellTower()
    {
        if (selectedTower != null && !WaveActive)
        {
            Gold += selectedTower.Price / 2;

            SoundManager.Instance.PlaySFX("Coin");

            selectedTower.GetComponentInParent<TileScript>().IsEmpty = true;

            Destroy(selectedTower.transform.parent.gameObject);

            DeselectTower();
        }
    }

    public void OnGoldChanged()
    {
        if (Changed != null)
        {
            Changed();
        }
    }

    public void RemoveMonster(Monster monster)
    {
        activeMonsters.Remove(monster);
        if (!WaveActive && !gameOver)
        {
            waveBtn.SetActive(true);
        }
    }

    public void ShowIngameMenu()
    {
        

        inGameMenu.SetActive(!inGameMenu.activeSelf);

        if (!inGameMenu.activeSelf)
        {
            Time.timeScale = 1;
            inMenu = false;
        }
        else
        {
            Time.timeScale = 0;
            inMenu = true;
        }
    }

    public void GameOver()
    {
        if (!gameOver)
        {
            gameOver = true;
            gameOverMenu.SetActive(true);
            waveBtn.SetActive(false);
        }
    }

    public void ShowOptions()
    {
        inGameMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void ShowAboutMenu()
    {
        inGameMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        aboutMenu.SetActive(true);
    }

    public void GoBack()
    {
        optionsMenu.SetActive(false);
        inGameMenu.SetActive(true);
    }
    
    public void GoBackAbout()
    {
        if (inMenu)
        {
            aboutMenu.SetActive(false);
            inGameMenu.SetActive(true);
        }
        else
        {
            aboutMenu.SetActive(false);
            gameOverMenu.SetActive(true);
        }
    }



    private void DropTower()
    {
        ClickedBtn = null;
        Hover.Instance.Deactivate();
    }

    public void Restart()
    {
        Time.timeScale = 1;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}
