using UnityEngine;

internal class CameraController : MonoBehaviour
{
    private float _offset;

    private Vector3 _myPosition;

    [SerializeField] private Transform followTarget;


    private void Awake()
    {
        GetComponentInChildren<Camera>().backgroundColor = ColorGenerator.Instance.GetColor();

        _myPosition = transform.position;
    }

    // Smoothly follows the ball's y-position
    private void FixedUpdate()
    {
        var yPosTarget = followTarget.position.y;

        if (yPosTarget <= _offset)
        {
            _offset = yPosTarget;

            var yNew = Mathf.Lerp(yPosTarget, _offset, _offset / yPosTarget);

            _myPosition.y = yNew;
        }

        transform.position = _myPosition;
    }
}
