using System.Collections.Generic;
using UnityEngine;

public class TweenSequence
{
    private List<TweenSequenceItem> items = new List<TweenSequenceItem>();
    private int currentIndex = 0;
    private bool isPlaying = false;

    private class TweenSequenceItem
    {
        public Tween Tween { get; private set; }
        public bool Join { get; private set; }

        public TweenSequenceItem(Tween tween, bool join)
        {
            Tween = tween;
            Join = join;
        }
    }

    public TweenSequence Append(Tween tween)
    {
        items.Add(new TweenSequenceItem(tween, false));
        return this;
    }

    public TweenSequence Join(Tween tween)
    {
        if (items.Count == 0)
        {
            return Append(tween);
        }
        items.Add(new TweenSequenceItem(tween, true));
        return this;
    }

    public void Start()
    {
        isPlaying = true;
        currentIndex = 0;
        PlayCurrent();
    }

    private void PlayCurrent()
    {
        if (currentIndex >= items.Count)
        {
            Complete();
            return;
        }

        var current = items[currentIndex];
        current.Tween.OnComplete(() =>
        {
            if (!current.Join)
            {
                currentIndex++;
                PlayCurrent();
            }
        }).Start();

        // 如果是 Join 的動畫，同時播放下一個
        if (current.Join)
        {
            currentIndex++;
            PlayCurrent();
        }
    }

    private void Complete()
    {
        isPlaying = false;
    }
} 