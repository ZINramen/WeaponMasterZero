using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

using UnityEngine.UI;
using Slider = UnityEngine.UI.Slider;

public class HP_Ctrl : MonoBehaviour
{
    [SerializeField] private GameObject monsterObj;

    private float monsterMaxHP;
    private float monsterCurrHP;
    
    // Start is called before the first frame update
    void Start()
    {
        monsterMaxHP = monsterObj.GetComponent<Entity>().maxHP;
        monsterCurrHP = monsterMaxHP;
    }

    // Update is called once per frame
    void Update()
    {
        monsterCurrHP = monsterObj.GetComponent<Entity>().GetHp();
        this.gameObject.GetComponent<Slider>().value = monsterCurrHP / monsterMaxHP;
    }
}
