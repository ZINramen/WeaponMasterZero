using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComboView : MonoBehaviour
{
    public Text comboValue;
    public GameObject view;

    public static int currValue = 1;
    public static int waitNumber = 0;

    private void Awake()
    {   waitNumber++;
        currValue++;

    }
    void Update()
    {
        if (currValue > 1) 
            view.SetActive(true);
        else
            view.SetActive(false);
        comboValue.text = currValue.ToString();
        
        StartCoroutine(CheckCombo());
    }
    IEnumerator CheckCombo() 
    {
        yield return new WaitForSeconds(1.5f);
        if (waitNumber > 0)
            waitNumber--;
        if (waitNumber == 0)
            currValue = 0;
        Destroy(gameObject);

    }
}
