using UnityEngine;
using System.Collections;

public class ProgressTracker : MonoBehaviour
{
    public static ProgressTracker Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public IEnumerator TrackPLCProgress(System.Func<bool> checkCondition, float deltaValue = 0.001f)
    {
        // IProgressPopView progressPopView = ReferenceManager.Instance.GetReference<IProgressPopView>();

        float progress = 0f;
        float maxValue = 1f;
        float extensionValue = deltaValue;

        // if (progressPopView != null)
        // {
        //     yield return progressPopView.Show(progress, maxValue);
        // }

        while (checkCondition.Invoke())
        {
            progress += deltaValue;

            if (progress >= maxValue)
            {
                maxValue += extensionValue;
            }

            // if (progressPopView != null)
            // {
            //     yield return progressPopView.Show(progress, maxValue);
            // }

            yield return null;
        }

        // if (progressPopView != null)
        // {
        //     progressPopView.Hide();
        // }

        yield return null;
    }
} 