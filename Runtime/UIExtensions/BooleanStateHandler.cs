using UnityEngine;
using UnityEngine.Events;

public class BooleanStateHandler : MonoBehaviour
{
    [System.Serializable]
    public class BooleanStateEvent : UnityEvent<bool> { }

    public BooleanStateEvent OnStateChanged;
    public UnityEvent OnStateTrue;
    public UnityEvent OnStateFalse;

    private bool currentState = false;

    public void SetState(bool newState)
    {
        if (currentState != newState)
        {
            currentState = newState;
            OnStateChanged.Invoke(currentState);

            if (currentState)
            {
                OnStateTrue.Invoke();
            }
            else
            {
                OnStateFalse.Invoke();
            }
        }
    }

    public void CheckState()
    {
        if (currentState)
        {
            OnStateTrue.Invoke();
        }
        else
        {
            OnStateFalse.Invoke();
        }
    }

    public bool GetCurrentState()
    {
        return currentState;
    }
}