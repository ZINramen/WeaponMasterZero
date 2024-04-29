using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

using Photon.Pun;

public class Mingyu_SoundManager : MonoBehaviourPun
{
    public static float s_volume = 1;

    // 현재 맵에 있는 데이터를 저장하는 용도 (맵에 배치된 오디오 리스트만 가짐 -> 초기값)
    private AudioSource[] audio_Array;

    // 추가되는 요소를 저장하는 요소 (전체 오디오 리스트를 가짐)
    private List<AudioSource> audio_List = new List<AudioSource>();

    private Slider Slider_Obj;

    private GameObject masterUI;
    private GameObject soundUI;

    // Start is called before the first frame update
    void Start()
    {
        // 맵에 배치된 오디오를 모두 배열로 데이터를 받는다.
        audio_Array = FindObjectsOfType<AudioSource>();

        // 배열로는 데이터를 추가하기 어렵기 때문에, 배열 -> List로 저장하고, 추가 오디오를 추가한다.
        foreach (AudioSource audio in audio_Array)
        {
            audio.GetComponent<AudioSource>().volume = Mingyu_SoundManager.s_volume;
            audio_List.Add(audio);
        }

        if (GameObject.Find("Master_UI") != null)
        {
            masterUI = GameObject.Find("Master_UI");

            Slider_Obj = masterUI.transform.Find("Sound_Slider").gameObject.GetComponent<Slider>();
            Slider_Obj.value = Mingyu_SoundManager.s_volume;

            masterUI.transform.Find("Sound_Slider").gameObject.SetActive(false);
            masterUI.SetActive(false);
        }

        if (GameObject.Find("SoundSetting_UI") != null)
        {
            soundUI = GameObject.Find("SoundSetting_UI");

            Slider_Obj = soundUI.transform.Find("Sound_Slider").gameObject.GetComponent<Slider>();
            Slider_Obj.value = Mingyu_SoundManager.s_volume;

            soundUI.SetActive(false);
        }
    }

    public void BtnClick_SettingUI()
    {
        if (PhotonNetwork.IsMasterClient)
            masterUI.SetActive(true);

        else
            soundUI.SetActive(true);
    }

    // 슬라이더의 값을 조절했을 때, 호출
    public void Set_Volume(Slider slider)
    {
        Mingyu_SoundManager.s_volume = slider.value;

        foreach (AudioSource audio in audio_List)
        {
            audio.GetComponent<AudioSource>().volume = Mingyu_SoundManager.s_volume;
        }
    }

    // 사운드를 추가할 때, 사용 (사운드 생성 시, 사용 -> 호현이가 호출)
    public void Add_AudioSouce(AudioSource spawnAudio)
    {
        spawnAudio.GetComponent<AudioSource>().volume = Mingyu_SoundManager.s_volume;
        audio_List.Add(spawnAudio);
    }
}
