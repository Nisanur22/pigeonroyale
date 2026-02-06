using UnityEngine;
using UnityEngine.UI;

public class PigeonButtonController : MonoBehaviour
{
    public Image pigeonImage;
    public Text pigeonNameText;

    // Bu fonksiyonla butonun görseli ve ismi atanır
    public void SetPigeon(Sprite sprite, string name)
    {
        if (pigeonImage != null)
            pigeonImage.sprite = sprite;
        if (pigeonNameText != null)
            pigeonNameText.text = name;
    }
} 