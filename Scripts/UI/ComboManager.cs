using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComboManager : MonoBehaviour
{
    public float comboSpeed;
    public float comboMaxValue;
    internal float comboDefaultMaxValue;
    public float comboBonusValue;
    public float comboWaitTime;
    internal float maxComboWaitTime;

    public List<string> comboNames;
    public List<Color> comboColors;
    int counter;

    internal Slider comboSlider;
    internal Animator sliderAnimator;
    internal Text comboNameText;
    internal Text comboCounterText;

    bool showSlider;


    public AudioClip levelUpAudio;
    public AudioClip maxLevelUpAudio;
    public AudioClip levelDownAudio;

    public static ComboManager instance;

    public int levelCombo;
    public int[] starCombos;
    public Sprite[] starSprites;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {

        }

        comboSlider = GameObject.FindGameObjectWithTag("UIManager").transform.Find("DisableUI").Find("Top").Find("ComboSlider").GetComponent<Slider>();
        sliderAnimator = comboSlider.GetComponent<Animator>();

        comboNameText = comboSlider.transform.Find("ComboName").GetComponent<Text>();
        comboCounterText = comboSlider.transform.Find("ComboCounter").GetComponent<Text>();
        comboCounterText.text = "0";
        comboSlider.maxValue = comboMaxValue;
        comboSlider.value = 0;
        comboDefaultMaxValue = comboMaxValue;

        maxComboWaitTime = comboWaitTime;
    }


    private void Update()
    {
        if (comboSlider.value <= 0 && showSlider)
        {
            LevelDown();
        }

        showSlider = comboSlider.value > 0;
        comboWaitTime -= Time.deltaTime * 2;
        comboSlider.gameObject.SetActive(showSlider);
        comboCounterText.text = "" + (int)comboSlider.value;

        if (showSlider && comboWaitTime <= 0)
        {
            comboSlider.value -= Time.deltaTime * comboSpeed;
        }
    }

    void LevelDown()
    {
        //Reset
        counter = 0;
        GetComponent<AudioSource>().clip = levelDownAudio;
        GetComponent<AudioSource>().Play();
        comboMaxValue = comboDefaultMaxValue;
        comboSlider.maxValue = comboDefaultMaxValue;
        comboSpeed = 1;

        DataManager.instance.score = 0;

        //Graphics
        comboNameText.text = comboNames[counter];
        comboNameText.color = comboColors[counter];
    }

    public void ComboHit()
    {
        float score = Random.Range(1, 3);
        levelCombo += (int)score;
        comboSlider.value += score;
        DataManager.instance.score += (int)score;
        DataManager.instance.SetScore();

        comboWaitTime = maxComboWaitTime;

        if (comboSlider.value >= comboMaxValue)
        {
            if (counter + 1 < comboNames.Count)
            {
                counter++;
                comboSlider.value = 1;
                comboSpeed += .5f;
                comboMaxValue += comboBonusValue;
                comboSlider.maxValue = comboMaxValue;
                sliderAnimator.Play("ComboHit");
                GetComponent<AudioSource>().clip = levelUpAudio;
                GetComponent<AudioSource>().Play();

                //Graphics
                comboNameText.text = comboNames[counter];
                comboNameText.color = comboColors[counter];
            }
            else
            {
                comboSlider.value = comboMaxValue/2;
                comboSpeed += .5f;
                comboMaxValue += comboBonusValue;
                comboSlider.maxValue = comboMaxValue;
                sliderAnimator.Play("ComboHit");
                GetComponent<AudioSource>().clip = maxLevelUpAudio;
                GetComponent<AudioSource>().Play();

                //Graphics
                comboNameText.text = comboNames[counter];
                comboNameText.color = comboColors[counter];
            }
        }
    }
}
