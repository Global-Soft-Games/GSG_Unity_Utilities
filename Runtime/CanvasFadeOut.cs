using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;

public class CanvasFadeOut : MonoBehaviour
{
    public float fadeDuration = 1f;
    private CanvasGroup canvasGroup;

    public UnityEvent OnBeginFadeOut;
    public UnityEvent OnEndFadeOut;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 1f;
    }

    public void StartFadeOut()
    {
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        OnBeginFadeOut.Invoke();

        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = 1f - Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 0f;

        // 淡出完成後，禁用射線阻擋和交互
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        OnEndFadeOut.Invoke();
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
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
    }
}