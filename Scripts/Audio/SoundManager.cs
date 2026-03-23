using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public PlaySound sounds;
    internal AudioSource lowHealthSound;
    public static SoundManager instance;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            print("Another instance of 'SoundManager' is running on the gameObject '" + this.name + "'.");
        }
    }

    public void PlaySound(int soundType, Transform parent)
    {
        sounds.target = parent;
        sounds.Sound(soundType);
    }
    public void PlayPoison(Transform parent)
    {
        sounds.target = parent;
        sounds.PlayRandomSound();
    }
}
