using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    Mingyu_SoundManager master;
    AudioSource sound;
    // Start is called before the first frame update
    void Start()
    {
        master = FindObjectOfType<Mingyu_SoundManager>();
        sound = GetComponent<AudioSource>();
        if (master) 
        {
            SoundUpdatetoMaster();
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
        sound.PlayOneShot(clip);
    }
}
