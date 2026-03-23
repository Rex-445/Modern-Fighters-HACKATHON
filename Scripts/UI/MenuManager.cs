using Assets.Scripts.UI;
using Michsky.UI.Shift;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


[Serializable]
public class LevelMenu
{
    public Sprite levelIcon1;
    public Sprite levelIcon2;

    public bool isLocked;

    public string levelName;
}


public class MenuManager : MonoBehaviour
{
    public int playerLevelProgress;

    int targetLevel;

    public Animator targetMenu;

    public GameObject menuParent;
    public List<Animator> allMenus;
    public Animator tipMenu;
    public Animator IDCreationMenu;
    public Animator slotWarningMenu;

    public List<GameObject> tips;

    public List<MainButton> gameSlots;
    public TabGroup difficultyTab;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI ageText;

    DataPersistenceManager dataPersistence;

    //UI Level Menus
    public LevelMenu[] levelMenus;
    public SpotlightButton[] levelMenuUI;
    public Text levelTitle;
    public Animator levelAnimator;
    public int LevelID;
    public float lvlStallTime = .19f;

    //Tutorial Screen
    public Animator tutorialScreen;
    public Toggle showTutScreen;
    public GameObject[] tutorialList;
    int tutIndex;

    // Start is called before the first frame update
    void Start()
    {
        dataPersistence = GetComponent<DataPersistenceManager>();
        playerLevelProgress = dataPersistence.gameData.levelProgress;

        ResetMenus();
        ShowTips();

        UpdateLevelMenus();
        showTutScreen.isOn = DataManager.instance.dataPersistence.saveData.showTutorialScreen;
    }

    private void ResetMenus()
    {
        //Clear the list to avoid redundancy
        allMenus.Clear();

        //Look through all the childred in the menu child list and add them to the 'allMenus' list
        for (int i = 0; i < menuParent.transform.childCount; i++)
        {
            allMenus.Add(menuParent.transform.GetChild(i).GetComponentInChildren<Animator>());
        }

        //Reset them
        foreach (Animator anim in allMenus)
        {
            if (anim != targetMenu)
                anim.Play("Menu_Out", 0, 1);
        }

        //Play the Main Menu
        targetMenu.Play("Menu_In");

        //Tutorial Screen
        if (DataManager.instance.dataPersistence.saveData.showTutorialScreen)
            LoadMenu(tutorialScreen);
    }

    public void TUT_Index(int ID)
    {
        //If index is not out of range
        if (tutIndex + ID > -1 && tutIndex + ID < tutorialList.Length)
        {
            tutorialList[tutIndex].SetActive(false);
            tutIndex += ID;
            tutorialList[tutIndex].SetActive(true);
        }
    }

    public void ToogleTutShow(bool value)
    {
        DataManager.instance.dataPersistence.saveData.showTutorialScreen = value;
    }

    private void UpdateLevelMenus()
    {
        //Update all Level Menus
        for (int i = 0; i < levelMenus.Length; i++)
        {
            levelMenus[i].isLocked = i + 1 > dataPersistence.gameData.levelProgress;
        }


        levelTitle.text = levelMenus[LevelID].levelName;
        //Apply all values to the UI
        /*for (int i=0; i < levelMenuUI.Length; i++)
        {
            //Left Menu
            if (LevelID > 1)
            {
                levelMenuUI[0].gameObject.SetActive(true);
                levelMenuUI[0].transform.Find("Lock").gameObject.SetActive(levelMenus[LevelID - 2].isLocked);
                levelMenuUI[0].firstImage = levelMenus[LevelID - 2].levelIcon1;
                levelMenuUI[0].secondImage = levelMenus[LevelID - 2].levelIcon2;
            }
            if (LevelID == 0 && i == 1)
            {
                levelMenuUI[1].gameObject.SetActive(false);
                continue;
            }
            else
            {
                if (i == 1)
                {
                    levelMenuUI[1].gameObject.SetActive(true);
                    levelMenuUI[1].transform.Find("Lock").gameObject.SetActive(levelMenus[LevelID - 1].isLocked);
                    levelMenuUI[1].firstImage = levelMenus[LevelID - 1].levelIcon1;
                    levelMenuUI[1].secondImage = levelMenus[LevelID - 1].levelIcon2;
                }
            }

            //Middle Menu
            if (i == 2)
            {
                levelMenuUI[2].gameObject.SetActive(true);
                levelMenuUI[2].transform.Find("Lock").gameObject.SetActive(levelMenus[LevelID].isLocked);
                levelMenuUI[2].firstImage = levelMenus[LevelID].levelIcon1;
                levelMenuUI[2].secondImage = levelMenus[LevelID].levelIcon2;
            }

            //Right Menu
            if (LevelID == levelMenus.Length - 1)
            {
                continue;
            }
            else
            {
                if (i == 3)
                {
                    levelMenuUI[3].gameObject.SetActive(true);
                    levelMenuUI[3].transform.Find("Lock").gameObject.SetActive(levelMenus[LevelID + 1].isLocked);
                    levelMenuUI[3].firstImage = levelMenus[LevelID + 1].levelIcon1;
                    levelMenuUI[3].secondImage = levelMenus[LevelID + 1].levelIcon2;
                }

                if (LevelID < levelMenus.Length - 2)
                {
                    levelMenuUI[4].gameObject.SetActive(true);
                    levelMenuUI[4].transform.Find("Lock").gameObject.SetActive(levelMenus[LevelID + 2].isLocked);
                    levelMenuUI[4].firstImage = levelMenus[LevelID + 2].levelIcon1;
                    levelMenuUI[4].secondImage = levelMenus[LevelID + 2].levelIcon2;
                }
            }



            //Outer Menus
            levelMenuUI[0].gameObject.SetActive(LevelID <= 1);
            levelMenuUI[4].gameObject.SetActive(LevelID >= levelMenus.Length - 2);

            //Finally Update Variables
            levelMenuUI[i].UpdateVariables();
        }*/



        //Apply all values
        {
            //Left Side
            if (LevelID > 0)
            {
                levelMenuUI[1].gameObject.SetActive(true);
                levelMenuUI[1].transform.Find("Lock").gameObject.SetActive(levelMenus[LevelID - 1].isLocked);
                levelMenuUI[1].firstImage = levelMenus[LevelID - 1].levelIcon1;
                levelMenuUI[1].secondImage = levelMenus[LevelID - 1].levelIcon2;
                levelMenuUI[1].UpdateVariables();
            }
            else
            {
                levelMenuUI[1].gameObject.SetActive(false);
            }

            //Middle Menu
            levelMenuUI[2].gameObject.SetActive(true);
            levelMenuUI[2].transform.Find("Lock").gameObject.SetActive(levelMenus[LevelID].isLocked);
            levelMenuUI[2].firstImage = levelMenus[LevelID].levelIcon1;
            levelMenuUI[2].secondImage = levelMenus[LevelID].levelIcon2;
            levelMenuUI[2].UpdateVariables();

            if (LevelID >= levelMenus.Length - 1)
            {
                levelMenuUI[3].gameObject.SetActive(false);
            }
            else
            {
                //Right Menu
                levelMenuUI[3].gameObject.SetActive(true);
                levelMenuUI[3].transform.Find("Lock").gameObject.SetActive(levelMenus[LevelID + 1].isLocked);
                levelMenuUI[3].firstImage = levelMenus[LevelID + 1].levelIcon1;
                levelMenuUI[3].secondImage = levelMenus[LevelID + 1].levelIcon2;
                levelMenuUI[3].UpdateVariables();
            }
        }
    }

    public void Next()
    {/*
        bool goOn = true;

        //If this, on the other had is the first in the list play the reverse animation
        if (LevelID == 0)
        {
            levelAnimator.Play("Slide_Left_End_Reverse");

            goOn = false;
        }

        //If this is the max in the list then play the left end animation
        if (LevelID == levelMenus.Length - 2)
        {
            levelAnimator.Play("Slide_Left_End");

            goOn = false;
        }

        if (LevelID < levelMenus.Length - 1)
        {
            if (goOn)
                levelAnimator.Play("Slide_Left");
        }*/


        if (LevelID < levelMenus.Length - 1)
            LevelID++;
        Invoke("UpdateLevelMenus", 0);
    
    }

    public void Prev()
    {/*
        bool goOn = true;

        //If this is the second in the list then play the left end animation
        if (LevelID == 1)
        {
            levelAnimator.Play("Slide_Right_End");

            goOn = false;
        }

        //If this is the max in the list then play the left end animation
        if (LevelID == levelMenus.Length - 1)
        {
            levelAnimator.Play("Slide_Right_End_Reverse");

            goOn = false;
        }


        if (LevelID > 0)
        {
            if (goOn)
                levelAnimator.Play("Slide_Right");
        }*/


        if (LevelID > 0)
            LevelID--;
        Invoke("UpdateLevelMenus", 0);

    }

    public void LoadGameSlots()
    {
        //Update Game Slots
        for (int i = 0; i < gameSlots.Count; i++)
        {
            GameData gameData = dataPersistence.CheckData(i);

            //Check if the current slot is activated or enabled

            //Time Played converted to "Time Data"
            float hour = gameData.playtime == 0 ? 0 : (int)(gameData.playtime / 3600);
            float minute = gameData.playtime == 0 ? 0 : (int)(gameData.playtime / 60);
            float seconds = gameData.playtime == 0 ? 0 : Mathf.Round(gameData.playtime % 60);

            string playTimeSecond = seconds.ToString().ToIntArray().Length == 1 ? "0" + seconds : "" + seconds;


            //Make a subtract count for the minute variable
            int subtractCount = 0;
            //This should only be after an hour
            if (gameData.playtime > 3600)
                subtractCount = (int)(gameData.playtime / 3600);

            //Make a loop with the result and pass the value to the "minute" variable
            for (int g=0; g < subtractCount; g++)
            {
                minute -= 60;
            }

            //Prepare the result
            string playTimeMinute= minute.ToString().ToIntArray().Length == 1 ? "0" + minute : "" + minute;
            string playTimeHour = hour.ToString().ToIntArray().Length == 1 ? "0" + hour : "" + hour;


            //Apply the result
            string playTime = playTimeHour + ":" + playTimeMinute + ":" + playTimeSecond;
            gameSlots[i].buttonText = gameData.userName.ToUpper() + ": " + playTime;
            gameSlots[i].UpdateVariables();
            gameSlots[i].transform.Find("Reset").gameObject.SetActive(gameData.usedSlot);
        }
    }

    public void ResetGameSlot(int ID)
    {
        dataPersistence.SlotID = ID;
        slotWarningMenu.Play("Menu_In");
    }

    public void ResetGameSlot()
    {
        dataPersistence.ResetGameSlot();
        LoadGameSlots();
    }

    private void ShowTips()
    {
        int rand = UnityEngine.Random.Range(0, tips.Count);
        if (tips.Count > 0)
            tips[rand].SetActive(true);
    }

    public void PlayGame()
    {
        if (LevelID + 1 <= playerLevelProgress)
        {
            DataManager.instance.dataPersistence.saveData.currentLevel = LevelID + 1;
            Invoke("Load2", 5);
            LoadMenu(tipMenu);
        }
    }


    //This uses the "Last Level Played" value as a LOAD
    public void ContinueGame()
    {
        DataManager.instance.dataPersistence.saveData.currentLevel = LevelID + 1;
        Invoke("Load2", 5);
        LoadMenu(tipMenu);
    }


    public void Load2()
    {
        SceneManager.LoadScene("Game" + (LevelID + 1));
    }

    public void LoadLevel(int level)
    {
        if (playerLevelProgress >= level)
        {
            targetLevel = level;

            Invoke("Load", 5);

            LoadMenu(tipMenu);
        }
    }

    public void Load()
    {
        SceneManager.LoadScene(targetLevel);
    }

    public void ExitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#endif
    }

    public void RawLoad(int lvl)
    {
        targetLevel = lvl;
        Invoke("Load", 15);
    }

    public void SlotChecker(int ID)
    {
        //Check for the slot in the data file has or has not yet been used
        GameData gamedata = dataPersistence.CheckData(ID);

        //Difficulty UI TabGoup
        difficultyTab.LoadDifficulty(gamedata);

        //After getting the reqiored file from FileManager then use it to compare
        //If this slot has not been used yet the open the ID creation menu and make a new one
        if (!gamedata.usedSlot)
        {
            LoadMenu(IDCreationMenu);
        }

        //If this slot has already been used then load the "Difficulty_Setting" Menu
        //and continue the game
        if (gamedata.usedSlot)
        {
            LoadMenu(allMenus[0]);
        }
    }

    //Save the difficulty lvl based on the target slot selected
    public void SetDifficulty(string value)
    {
        //This is useful for saving and loading in the correct game slot
        dataPersistence.gameData.difficulty = value;
        dataPersistence.SaveGame();
    }
    
    public void SetJoystickKey(bool value)
    {
        dataPersistence.gameData.isJoystick = value;
    }

    public void Accept(Animator animator)
    {
        if (nameText.text != "" && ageText.text != "")
        {
            if (nameText.text.ToCharArray().Length < 3 || nameText.text.ToCharArray().Length > 11)
            {
                //Reject it
                RejectName();
                return;
            }

            //Reject Age
            if (ageText.text.ToCharArray().Length < 2 || ageText.text.ToCharArray().Length > 3)
            {
                //Reject it
                RejecAge();
                return;
            }

            //Accept it
            LoadMenu(animator);

            //Save Values
            dataPersistence.gameData.userName = nameText.text;
            dataPersistence.gameData.userAge  = ageText.text;
            dataPersistence.gameData.usedSlot = true;

            //Reset UI
            nameText.text = "";
            ageText.text = "";

            //Save and Reload
            dataPersistence.SaveGame();
            LoadGameSlots();
        }

        //Else Show incomplete
        else
        {
            //Reject Name
            if (nameText.text.ToCharArray().Length < 3)
            {
                //Reject it
                RejectName();
            }

            //Reject Age
            if (ageText.text.ToCharArray().Length < 2)
            {
                //Reject it
                RejecAge();
            }
        }
    }

    public void RejectName()
    {
        nameText.transform.parent.parent.Find("Incomplete").GetComponent<Animator>().Play("Incomplete", 0, 0);
    }

    public void RejecAge()
    {
        ageText.transform.parent.parent.Find("Incomplete").GetComponent<Animator>().Play("Incomplete", 0, 0);
    }

    public void LoadMenu(Animator animator)
    {
        targetMenu.Play("Menu_Out", 0, 0);
        targetMenu = animator;
        targetMenu.Play("Menu_In", 0, 0);

        UpdateLevelMenus();
    }
}
