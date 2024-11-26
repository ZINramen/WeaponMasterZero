using System.Collections;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class FancySpeechBubble : MonoBehaviour
{
    public static CsvReader csv;
    AudioSource soundPlayer;
    public int characterStartSize = 1;
    public float characterAnimateSpeed = 1000f;

    private string _rawText;
    public string rawText { get { return _rawText; } }

    private string _processedText;
    public string processedText { get { return _processedText; } }

    private void Start()
    {
        soundPlayer = gameObject.AddComponent<AudioSource>();
        soundPlayer.volume = 1;
    }
    public void Set_Int(int textNum)
    {
        string text = csv.lines[textNum][PlayerPrefs.GetInt("Country_Code", 1)];
        StopAllCoroutines();
        StartCoroutine(SetRoutine(text));
    }

    public void Set(string text)
    {
        StopAllCoroutines();
        StartCoroutine(SetRoutine(text));
    }

    public IEnumerator SetRoutine(string text)
    {
        _rawText = text;
        yield return StartCoroutine(CharacterAnimation());
    }

    private IEnumerator CharacterAnimation()
    {
        TextMeshProUGUI label = GetComponent<TextMeshProUGUI>();
        label.enableWordWrapping = true;
        
        string prefix = "";
        foreach (char c in _rawText.ToCharArray())
        {
            float size = characterStartSize;
            while (size < label.fontSize)
            {
                size += (int)(Time.deltaTime * characterAnimateSpeed);
                size = Mathf.Min(size, label.fontSize);
                label.text = prefix + "<size=" + size + ">" + c + "</size>";
                yield return new WaitForEndOfFrame();
            }
            if(transform.parent.parent.parent.GetComponentInChildren<Canvas>().enabled)
                soundPlayer.PlayOneShot(Resources.Load("text-Typing") as AudioClip);
            prefix += c;
        }

        label.text = _rawText;
    }
}