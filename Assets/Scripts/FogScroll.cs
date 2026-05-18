using UnityEngine;
using DG.Tweening;

public class FogScroll : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float scrollSpeedX = 0.5f; 
    [SerializeField] private float scrollSpeedY = 0.1f; 
    [SerializeField] private float resetDistanceX = 10f; 

    private Tween moveTween;
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
        StartFogMovement();
    }

    void StartFogMovement()
    {
        float duration = resetDistanceX / Mathf.Abs(scrollSpeedX);
        Vector3 targetPosition = startPosition + new Vector3(scrollSpeedX * duration, scrollSpeedY * duration, 0);

        moveTween = transform.DOMove(targetPosition, duration).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart); 
    }

    private void OnDestroy()
    {
        if (moveTween != null)
        {
            moveTween.Kill();
        }
    }
}