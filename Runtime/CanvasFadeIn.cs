using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;

public class CanvasFadeIn : MonoBehaviour
{
    public float fadeDuration = 1f;
    private CanvasGroup canvasGroup;

    public UnityEvent OnBeginFadeIn;
    public UnityEvent OnEndFadeIn;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 0f;
    }

    public void StartFadeIn()
    {
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        // 淡入開始時，啟用射線阻擋和交互
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;

        OnBeginFadeIn.Invoke();

        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;

        OnEndFadeIn.Invoke();
    }

    // 公開方法來手動控制射線阻擋
    public void SetRaycastBlocking(bool block)
    {
        canvasGroup.blocksRaycasts = block;
        canvasGroup.interactable = block;
    }

    // 公開方法來重置 Canvas
    public void ResetCanvas()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }
}