using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class FlexibleAnimationScript : MonoBehaviour
{
    public enum MoveDirection
    {
        Right,
        Left,
        Up,
        Down,
        Forward,
        Back
    }

    public MoveDirection moveDirection = MoveDirection.Right;
    public float distance = 1f;
    public float duration = 1f;
    public bool pingPong = true;
    public AnimationCurve movementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

#if UNITY_EDITOR
    [SerializeField]
    private bool previewInEditor = true;
#endif

    private Vector3 startPosition;
    public bool IsAnimating { get; private set; }
    private Coroutine animationCoroutine;
    private MoveDirection defaultMoveDirection;

    private void Start()
    {
        startPosition = transform.position;
        defaultMoveDirection = moveDirection;
    }

    public void StartAnimation()
    {
        if (!IsAnimating)
        {
#if UNITY_EDITOR
            if (previewInEditor)
            {
                startPosition = transform.position;
            }
#endif

            IsAnimating = true;
            Vector3 direction = GetDirectionVector();
            if (animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine);
                animationCoroutine = null;
            }
            animationCoroutine = StartCoroutine(AnimateMovement(direction));
        }
    }

    public void StartAnimationWithUp()
    {
        moveDirection = MoveDirection.Up;
        StartAnimation();
    }

    public void StartAnimationWithDown()
    {
        moveDirection = MoveDirection.Down;
        StartAnimation();
    }

    public void StartAnimationWithForward()
    {
        moveDirection = MoveDirection.Forward;
        StartAnimation();
    }

    public void StartAnimationWithBack()
    {
        moveDirection = MoveDirection.Back;
        StartAnimation();
    }

    public void StartAnimationWithLeft()
    {
        moveDirection = MoveDirection.Left;
        StartAnimation();
    }

    public void StartAnimationWithRight()
    {
        moveDirection = MoveDirection.Right;
        StartAnimation();
    }

    public void StartAnimationWithInverse()
    {
        switch (moveDirection)
        {
            case MoveDirection.Right:
                moveDirection = MoveDirection.Left;
                break;
            case MoveDirection.Left:
                moveDirection = MoveDirection.Right;
                break;
            case MoveDirection.Up:
                moveDirection = MoveDirection.Down;
                break;
            case MoveDirection.Down:
                moveDirection = MoveDirection.Up;
                break;
            case MoveDirection.Forward:
                moveDirection = MoveDirection.Back;
                break;
            case MoveDirection.Back:
                moveDirection = MoveDirection.Forward;
                break;
        }
        StartAnimation();
    }

    public void StartAnimationWithRightOnce()
    {
        if (moveDirection == MoveDirection.Right) return;
        moveDirection = MoveDirection.Right;
        StartAnimation();
    }

    public void StartAnimationWithLeftOnce()
    {
        if (moveDirection == MoveDirection.Left) return;
        moveDirection = MoveDirection.Left;
        StartAnimation();
    }

    public void StopAnimation()
    {
        if (IsAnimating)
        {
            IsAnimating = false;
            if (animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine);
                animationCoroutine = null;
            }
            transform.position = startPosition;
        }
    }

    private IEnumerator AnimateMovement(Vector3 direction)
    {
        Vector3 endPosition = startPosition + direction * distance;

        while (IsAnimating)
        {
            yield return MoveToPosition(startPosition, endPosition);
            if (!pingPong) break;
            yield return MoveToPosition(endPosition, startPosition);
        }

        IsAnimating = false;
    }

    private IEnumerator MoveToPosition(Vector3 start, Vector3 end)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            float curveValue = movementCurve.Evaluate(t);
            transform.position = Vector3.Lerp(start, end, curveValue);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = end;
    }

    public void SetMovementParameters(MoveDirection newDirection, float newDistance, float newDuration, bool newPingPong)
    {
        moveDirection = newDirection;
        distance = newDistance;
        duration = newDuration;
        pingPong = newPingPong;
    }

    public void TogglePreviewAnimation()
    {
        IsAnimating = !IsAnimating;
        if (!IsAnimating)
        {
            transform.position = startPosition;
        }
    }

    private Vector3 GetDirectionVector()
    {
        switch (moveDirection)
        {
            case MoveDirection.Right:
                return Vector3.right;
            case MoveDirection.Left:
                return Vector3.left;
            case MoveDirection.Up:
                return Vector3.up;
            case MoveDirection.Down:
                return Vector3.down;
            case MoveDirection.Forward:
                return Vector3.forward;
            case MoveDirection.Back:
                return Vector3.back;
            default:
                return Vector3.right;
        }
    }
}