using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyCardManager : MonoBehaviour
{
    public int keysCollected;


    private void Start()
    {
        keysCollected = PlayerPrefs.GetInt("KeysCollected", 0);
        GameObject.FindGameObjectWithTag("UIManager").transform.Find("DisableUI").Find("Top").Find("KeysCollected").Find("Text").GetComponent<Text>().text = "KEYS: " + keysCollected;
    }


    public void SaveKey()
    {
        PlayerPrefs.SetInt("KeysCollected", PlayerPrefs.GetInt("KeysCollected", 0) + 1);
        keysCollected = PlayerPrefs.GetInt("KeysCollected", 0);
        GameObject.FindGameObjectWithTag("UIManager").transform.Find("DisableUI").Find("Top").Find("KeysCollected").Find("Text").GetComponent<Text>().text = "KEYS: " + keysCollected;
    }
}
