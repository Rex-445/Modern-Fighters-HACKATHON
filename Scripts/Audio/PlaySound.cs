using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PlaySound : MonoBehaviour
{
    [SerializeField] private AudioClip[] combatSounds;
    [SerializeField] private AudioClip[] randomCombatSounds;
    [SerializeField] private AudioMixerGroup mixer;

    public float targetVolume = .5f;
    public float soundType = 1;
    internal Transform target;

    public float frequency = 0;
    internal float maxFrequency;

    private void Start()
    {
        target = transform;
        maxFrequency = frequency;

        //GetComponent<AudioSource>().ignoreListenerPause = true;
    }

    private void Update()
    {
        frequency -= Time.deltaTime;
    }


    public void Sound(int id)
    {
        if (!Application.isPlaying || frequency > 0)
            return;

        frequency = maxFrequency;

        GameObject go = new GameObject();
        go.transform.SetParent(target);
        go.transform.position = target.transform.position;
        go.AddComponent<AudioSource>();
        go.GetComponent<AudioSource>().clip = combatSounds[id];
        go.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("SoundVolume", .5f);
        go.GetComponent<AudioSource>().spatialBlend = soundType;
        if (targetVolume != .5f)
            go.GetComponent<AudioSource>().volume = targetVolume;
        go.GetComponent<AudioSource>().outputAudioMixerGroup = mixer;
        go.GetComponent<AudioSource>().Play();
        Destroy(go, go.GetComponent<AudioSource>().clip.length);
    }

    public void PlayRandomSound()
    {
        if (!Application.isPlaying || frequency > 0)
            return;

        frequency = maxFrequency;

        if (randomCombatSounds.Length <= 0)
            return;

        GameObject go = new GameObject();
        go.transform.SetParent(target);
        go.transform.position = target.transform.position;
        go.AddComponent<AudioSource>();
        go.GetComponent<AudioSource>().clip = randomCombatSounds[Random.Range(0, randomCombatSounds.Length)];
        go.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("SoundVolume", .5f);
        go.GetComponent<AudioSource>().spatialBlend = soundType;
        if (targetVolume != .5f)
            go.GetComponent<AudioSource>().volume = targetVolume;
        go.GetComponent<AudioSource>().outputAudioMixerGroup = mixer;
        go.GetComponent<AudioSource>().Play();
        Destroy(go, go.GetComponent<AudioSource>().clip.length);
    }
}

