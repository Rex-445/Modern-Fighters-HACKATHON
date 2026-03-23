using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterSelectionUI : MonoBehaviour
{
    public List<CharacterSlot> characterSlots;
    public List<GameObject> characterSlotUI;

    public Dictionary<string, Color> teamColor;

    [Header("Confirm Character")]
    public CharacterSlot targetSlot;
    public GameObject targetSlotUI;

    public int slotID;

    public void UpdateCharacterSlots()
    {
        foreach(GameObject slotUI in characterSlotUI)
        {

        }
    }

    public void SetSlotID(int ID)
    {
        slotID = ID;
    }

    public void SetCharacterSlot(int ID, CharacterSlot slot, string characterType = "Human")
    {
        characterSlotUI[ID].transform.Find("");
    }
}

[Serializable]
public class CharacterSlot
{
    public string characterName = "--";
    public string fighterName = "Random?";
    public string fighterTeam = "Independent";
    public Sprite characterFace;

    public CharacterSlot()
    {
        characterName = "--";
        fighterName = "Random?";
        fighterTeam = "Independent";
    }
}
