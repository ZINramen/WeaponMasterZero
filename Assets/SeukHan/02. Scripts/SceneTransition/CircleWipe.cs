using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
 
public class CircleWipe : SceneTransition
{
    public Image circle;
 
    public override IEnumerator AnimateTransitionIn()
    {
        circle.rectTransform.anchoredPosition = new Vector2(-2400f, 0f);
        var tweener = circle.rectTransform.DOAnchorPosX(0f, 1.5f);
        yield return tweener.WaitForCompletion();
    }
 
    public override IEnumerator AnimateTransitionOut()
    {
        var tweener = circle.rectTransform.DOAnchorPosX(2400f, 1.5f);
        yield return tweener.WaitForCompletion();
    }
}