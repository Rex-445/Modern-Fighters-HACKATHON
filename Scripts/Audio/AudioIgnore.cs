using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Audio
{
    public class AudioIgnore : MonoBehaviour
    {
        AudioSource sound;

        // Use this for initialization
        void Start()
        {
            if (sound == null)
                GetComponent<AudioSource>().ignoreListenerPause = true;
        }
    }
}