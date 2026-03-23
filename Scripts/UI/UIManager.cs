using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using UnityStandardAssets.CrossPlatformInput;
using TMPro;

public class UIManager : MonoBehaviour
{

    public static UIManager instance;

    public Animator failedAnim;
    public Animator successAnim;

    public Button keysButton;

    public Slider bossUI;

    UnitManager unitManager;

    public CanvasGroup analogue;
    public CanvasGroup joystickKeys;

    public CanvasGroup uiGroup;

    public Slider mainHealthBar;
    public List<Slider> mainResourceBar;

    public GameObject androidSkillLayout;
    public GameObject pcSkillLayout;

    [Header("Game Title")]
    public TextMeshProUGUI gameTitleText;

    [Space(50)]
    [Header("Pause Menu Variables")]
    //Pause Menu
    public Slider healthBar;
    public Slider expBar;
    public TextMeshProUGUI score;
    public Text unitLevel;
    public GameObject pauseObject;
    public GameObject androidController;




    /// <summary>
    /// Controller Type: This check if the controller is an analogue stick or a normal joystick button
    /// </summary>
    [HideInInspector]
    public bool isJoystick = false;

    [Header("Ending")]
    public List<GameObject> stars;
    public Image starSprite;

    [System.Obsolete]
    // Start is called before the first frame update
    void Awake()
    {
        foreach(Slider slider in mainResourceBar)
        {
            slider.maxValue = 30;
            slider.value = 0;
        }

#if PLATFORM_ANDROID

        androidController.SetActive(true);
        pcSkillLayout.SetActive(false);
#endif

#if PLATFORM_STANDALONE || PLATFORM_WEBGL

        pcSkillLayout.SetActive(true);
        androidController.SetActive(false);
#endif

        try
        {
            if (UnitManager.instance.player.GetComponent<UnitController>().canControl)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    Pause(!pauseObject.active);
                }
            }
        } catch { }

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            print("Another instance of UIManager is running on the gameObject '" + this.name + "'.");
        }

        try
        {
            unitManager = GameObject.FindGameObjectWithTag("UnitManager").GetComponent<UnitManager>();
        }
        catch { }

        isJoystick = GetComponent<DataPersistenceManager>().gameData.isJoystick;
        try
        {
            UpdateController();
        }
        catch { }

        try
        {
            EnableUI();
        } catch { }
    }


    private void UpdateController()
    {

        //Enable Analoge
        analogue.alpha = isJoystick ? 1 : 0;
        analogue.blocksRaycasts = isJoystick;
        analogue.interactable = isJoystick;

        //Disable Keys
        joystickKeys.alpha = isJoystick ? 0 : 1;
        joystickKeys.blocksRaycasts = !isJoystick;
        joystickKeys.interactable = !isJoystick;
    }

    public void DisableUI()
    {
        uiGroup.alpha = 0;
        uiGroup.interactable = false;
        uiGroup.blocksRaycasts |= false;
    }

    public void EnableUI()
    {
        uiGroup.alpha = 1;
        uiGroup.interactable = true;
        uiGroup.blocksRaycasts = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "EndingScene")
            return;

        //Boss
        UpdateBossUI();


        //Pause UI
        UpdatePauseUI();

        //Key 
        if (keysButton == null)
            return;

        keysButton.enabled = PlayerPrefs.GetInt("KeysCollected", 0) >= 8;
        if (keysButton.enabled == false)
        {
            keysButton.GetComponent<Image>().color = new Color(1, 1, 1, .2f);
        }
        else
        {
            keysButton.GetComponent<Image>().color = new Color(1, 1, 1, 1f);
        }

        //Other UI
        mainHealthBar.maxValue = unitManager.player.maxHealth;
    }

    private void UpdatePauseUI()
    {
        // Pause Input
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Pause(!pauseObject.active);
        }
        //Health
        healthBar.value = unitManager.player.health;
        healthBar.maxValue = unitManager.player.maxHealth;
        healthBar.transform.Find("Value").GetComponent<TextMeshProUGUI>().text = unitManager.player.health + " / <b>" + unitManager.player.maxHealth + "</b>";

        //Exp
        expBar.value = DataManager.instance.currentExp;
        expBar.maxValue = DataManager.instance.maxExp;
        expBar.transform.Find("Value").GetComponent<TextMeshProUGUI>().text = DataManager.instance.currentExp + " / <b>" + DataManager.instance.maxExp + "</b>";

        //Score
        score.text = "Highest Score \n" + DataManager.instance.highestScore;
        unitLevel.text = "Level " + DataManager.instance.unitLevel;

    }

    private void UpdateBossUI()
    {
        try
        {
            bossUI.gameObject.SetActive(unitManager.boss != null);

            if (unitManager.boss != null)
            {
                bossUI.value = unitManager.boss.health;
                bossUI.maxValue = unitManager.boss.maxHealth;
                bossUI.transform.Find("HealthValue").GetComponent<Text>().text = bossUI.value + "/" + bossUI.maxValue;
                bossUI.transform.Find("BossName").GetComponent<Text>().text = unitManager.boss.unitName.ToUpper();
                bossUI.transform.Find("BossName").GetComponent<Text>().color = unitManager.boss.theme;
                bossUI.transform.Find("BossUI").Find("Face").GetComponent<Image>().sprite = unitManager.boss.image;
            }
        }
        catch { }

    }

    public void NextLevel()
    {
        DataManager.instance.dataPersistence.gameData.lastPlayedLevel = LevelManager.instance.levelID + 1;
        DataManager.instance.dataPersistence.saveData.currentLevel = LevelManager.instance.levelID + 1;
        CheckPointManager.instance.id = 0;
        DataManager.instance.dataPersistence.SaveGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }


    public void OpenScene(string sceneName)
    {
        TimeManager.instance.TimePlay();
        DataManager.instance.dataPersistence.SaveGame();
        SceneManager.LoadScene(sceneName);
    }

    public void LoadLevel(string level)
    {
        TimeManager.instance.TimePlay();
        DataManager.instance.dataPersistence.gameData.lastPlayedLevel = LevelManager.instance.levelID;
        DataManager.instance.dataPersistence.SaveGame();
        SceneManager.LoadScene(level);
    }

    public void RestartLevel()
    {
        TimeManager.instance.TimePlay();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        DataManager.instance.dataPersistence.SaveGame();
    }

    public void Failed()
    {
        failedAnim.Play("Failed");
        DisableUI();

        DataManager.instance.dataPersistence.SaveGame();
    }

    public void Success()
    {
        //Load Stars
        ComboManager cm = ComboManager.instance;
        for(int i=0; i < cm.starCombos.Length; i ++)
        {
            //Check for each and set for each
            if (cm.levelCombo >= cm.starCombos[i])
            {
                stars[i].SetActive(true);
                starSprite.sprite = cm.starSprites[i];
            }
        }



        CheckPointManager.instance.id = 0;
        successAnim.Play("Success");
        DisableUI();

        //Set the level
        //Save this only if the player has not reached this level already
        if (LevelManager.instance.levelID >= DataManager.instance.dataPersistence.gameData.levelProgress)
        {
            DataManager.instance.dataPersistence.gameData.levelProgress = LevelManager.instance.levelID + 1;
        }


        DataManager.instance.dataPersistence.gameData.lastPlayedLevel = LevelManager.instance.levelID;


        DataManager.instance.dataPersistence.SaveGame();
    }


    public IEnumerator DebugGameTitle(string title)
    {
        gameTitleText.text = title;

        //Fade it In First
        while (gameTitleText.alpha < 1)
        {
            gameTitleText.alpha += .1f;

            yield return null;
        }

        yield return new WaitForSeconds(2.5f);


        //Then Fade it out
        while (gameTitleText.alpha > 0)
        {
            gameTitleText.alpha -= .1f;

            yield return null;
        }

    }

    public void Pause(bool value)
    {
        pauseObject.SetActive(value);

        if (value)
            TimeManager.instance.TimePause();
        else
            TimeManager.instance.TimePlay();
    }
}
