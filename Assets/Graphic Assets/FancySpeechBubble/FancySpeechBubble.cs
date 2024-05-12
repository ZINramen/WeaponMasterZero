using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(TextMeshProUGUI))]
public class FancySpeechBubble : MonoBehaviour
{
    public int characterStartSize = 1;
    public float characterAnimateSpeed = 1000f;
    public Image bubbleBackground;
    public float backgroundMinimumHeight;
    public float backgroundVerticalMargin;
    
    private string _rawText;
    public string rawText { get { return _rawText; } }

    private string _processedText;
    public string processedText { get { return _processedText; } }

    public void Set(string text)
    {
        StopAllCoroutines();
        StartCoroutine(SetRoutine(text));
    }

    public IEnumerator SetRoutine(string text)
    {
        _rawText = text;
        yield return StartCoroutine(TestFit());
        yield return StartCoroutine(CharacterAnimation());
    }

    private IEnumerator TestFit()
    {
        TextMeshProUGUI label = GetComponent<TextMeshProUGUI>();

        float alpha = label.color.a;
        label.color = new Color(label.color.r, label.color.g, label.color.b, 0f);

        label.text = _rawText;

        yield return new WaitForEndOfFrame();

        float totalHeight = label.preferredHeight;

        if (bubbleBackground != null)
        {
            bubbleBackground.rectTransform.sizeDelta = new Vector2(
                bubbleBackground.rectTransform.sizeDelta.x,
                Mathf.Max(totalHeight + backgroundVerticalMargin, backgroundMinimumHeight));
        }

        _processedText = "";
        string buffer = "";
        string line = "";
        float currentHeight = -1f;

        foreach (string word in _rawText.Split(' '))
        {
            buffer += word + " ";
            label.text = buffer;
            yield return new WaitForEndOfFrame();

            if (currentHeight < 0f)
            {
                currentHeight = label.preferredHeight;
            }

            if (currentHeight != label.preferredHeight)
            {
                currentHeight = label.preferredHeight;
                _processedText += line.TrimEnd(' ') + "\n";
                line = "";
            }

            line += word + " ";
        }

        _processedText += line;

        label.text = "";
        label.rectTransform.sizeDelta = new Vector2(label.rectTransform.sizeDelta.x, totalHeight);
        label.color = new Color(label.color.r, label.color.g, label.color.b, alpha);
    }

    private IEnumerator CharacterAnimation()
    {
        TextMeshProUGUI label = GetComponent<TextMeshProUGUI>();

        string prefix = "";
        foreach (char c in _processedText.ToCharArray())
        {
            float size = characterStartSize;
            while (size < label.fontSize)
            {
                size += (int)(Time.deltaTime * characterAnimateSpeed);
                size = Mathf.Min(size, label.fontSize);
                label.text = prefix + "<size=" + size + ">" + c + "</size>";
                yield return new WaitForEndOfFrame();
            }
            prefix += c;
        }

        label.text = _processedText;
    }
}