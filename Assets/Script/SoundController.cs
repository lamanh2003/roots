using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip clickSound;
    // Start is called before the first frame update
    
    public void PlayClickSound()
    {
        audioSource.PlayOneShot(clickSound);
    }
    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
