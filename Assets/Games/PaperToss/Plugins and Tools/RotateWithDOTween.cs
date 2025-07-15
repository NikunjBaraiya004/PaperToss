using DG.Tweening;
using UnityEngine;

public class RotateWithDOTween : MonoBehaviour
{
    [SerializeField] float angle = 45f;   // How far left/right to rotate
    [SerializeField] float speed = 2f;    // Time to rotate from left to right

    void Start()
    {
        // Rotate Y to +angle and -angle back and forth
        transform.DOLocalRotate(new Vector3(transform.localEulerAngles.x, angle, 0), speed)
                 .SetEase(Ease.Linear)
                 .SetLoops(-1, LoopType.Incremental);
    }
}
