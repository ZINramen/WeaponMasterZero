using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ReverseWipe : SceneTransition
{
    public Image img;
 
    public override IEnumerator AnimateTransitionIn()
    {
        img.rectTransform.anchoredPosition = new Vector2(2400f, 0f);
        var tweener = img.rectTransform.DOAnchorPosX(0f, 1.5f);
        yield return tweener.WaitForCompletion();
    }
 
    public override IEnumerator AnimateTransitionOut()
    {
        var tweener = img.rectTransform.DOAnchorPosX(-2400f, 1.5f);
        yield return tweener.WaitForCompletion();
    }
}
