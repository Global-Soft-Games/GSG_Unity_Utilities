using UnityEngine;

public class ImageScaler : MonoBehaviour
{
    public static Vector2 ScaleImageToFit(float originalWidth, float originalHeight, float frameWidth, float frameHeight)
    {
        float widthRatio = frameWidth / originalWidth;
        float heightRatio = frameHeight / originalHeight;

        float scaleFactor = Mathf.Min(widthRatio, heightRatio);

        float newWidth = Mathf.Floor(originalWidth * scaleFactor);
        float newHeight = Mathf.Floor(originalHeight * scaleFactor);

        return new Vector2(newWidth, newHeight);
    }

    public static Vector2 CalculateCenterPosition(float frameWidth, float frameHeight, float imageWidth, float imageHeight)
    {
        float x = (frameWidth - imageWidth) * 0.5f;
        float y = (frameHeight - imageHeight) * 0.5f;
        return new Vector2(x, y);
    }

    // 示例：如何在 Unity 中使用這些方法
    public void ScaleImage(RectTransform imageRectTransform, RectTransform frameRectTransform, float imageWidth, float imageHeight)
    {
        Vector2 frameSize = frameRectTransform.rect.size;

        Vector2 newSize = ScaleImageToFit(imageWidth, imageHeight, frameSize.x, frameSize.y);
        imageRectTransform.sizeDelta = newSize;
    }
}