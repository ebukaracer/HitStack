using UnityEngine;

/// <summary>
/// Gradually spins or rotates the platform-holder parent over time.
/// </summary>
internal class Spinner : MonoBehaviour
{
    [SerializeField] private float rotateSpeed = 100f;

    private void Update()
    {
        transform.Rotate(rotateSpeed * Time.deltaTime * Vector3.up);
    }
}
