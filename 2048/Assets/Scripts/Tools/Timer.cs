using DG.Tweening;

public static class Timer
{
    public static Tweener SetTimer(float time, System.Action onComplete)
    {
        var currentTime = time;
        return DOTween.To(() => currentTime, newTime => currentTime = newTime, 0, time).OnComplete(() =>
        {
            onComplete?.Invoke();
        }).SetEase(Ease.Linear);
    }

    public static TimerHandler SetBackwardTimer(float time, System.Action onComplete, System.Action<float> onUpdateNormalized)
    {
        var currentTime = time;
        var handler = new TimerHandler();
        handler.RemainTime = time;

        handler.Timer = DOTween.To(() => currentTime, newTime => currentTime = newTime, 0, time)
        .OnUpdate(() =>
        {
            var normalizedValue = currentTime / time;
            handler.RemainTime = normalizedValue * time;
            onUpdateNormalized?.Invoke(normalizedValue);
        })
        .OnComplete(() =>
        {
            onComplete?.Invoke();
        })
        .SetEase(Ease.Linear);

        return handler;
    }

    public class TimerHandler
    {
        public Tweener Timer;
        public float RemainTime;
    }
}
