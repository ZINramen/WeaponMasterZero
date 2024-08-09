using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public bool isStartToMute;
    public float delayMusicTime = 0;
    Mingyu_SoundManager master;
    AudioSource sound;
    // Start is called before the first frame update
    void Start()
    {
        if(delayMusicTime > 0)
        {
            StartCoroutine(DelayPlayMusic(delayMusicTime));
        }
        master = FindObjectOfType<Mingyu_SoundManager>();
        sound = GetComponent<AudioSource>();
        if (master) 
        {
            SoundUpdatetoMaster();
        }

        if (isStartToMute)
        {
            foreach (AudioSource audio in FindObjectsOfType<AudioSource>())
            {
                audio.Stop();
            }
        }
    }

    public void SoundUpdatetoMaster() 
    {
        // 현재는 배열이라 사운드 추가안됨.
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void PlaySound(AudioClip clip) 
    {
        if(sound != null) 
        {
            sound.PlayOneShot(clip);
        }
            
    }
    IEnumerator DelayPlayMusic(float delay)
    {
        yield  return new WaitForSeconds(delay);
        sound.Play();
    }
}
