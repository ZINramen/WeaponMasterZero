using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName ="Image_", menuName = "ImageData", order = int.MaxValue)]
public class ImageData : ScriptableObject
{
    public Sprite[] medal;
}
