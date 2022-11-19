using UnityEngine;
using Random = UnityEngine.Random;

internal class ChildStack : MonoBehaviour
{
    private MeshCollider _collider;
    private MeshRenderer _renderer;
    private Rigidbody _rb;

    [SerializeField] private float destroyOffset = .5f;

    private void Awake()
    {
        _collider = GetComponent<MeshCollider>();
        _renderer = GetComponent<MeshRenderer>();
        _rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Applies an explosion force to the stack.
    /// Disables the stack thereafter.
    /// </summary>
    public void ApplyExpForce(Transform parent)
    {
        _collider.isTrigger = true;

        _rb.isKinematic = false;

        _rb.constraints = RigidbodyConstraints.None;


        // relative position of the parent stack
        var forcePoint = parent.position;

        // x position of the parent stack
        var parentXPos = forcePoint.x;

        // center of the platform parts
        var xPos = _renderer.bounds.center.x;

        // making objects move either left or right after breakage
        var subDirection = (parentXPos - xPos < 0) ? Vector3.right : Vector3.left;

        var direction = (Vector3.up * destroyOffset + subDirection).normalized;

        float force = Random.Range(10, 15);

        _rb.AddForceAtPosition(direction * force, forcePoint, ForceMode.Impulse);

        // May toggle inactive instead.
        Destroy(_rb.transform.parent.gameObject, 1.2f);

        /*
         Randomized destroy Position
        Vector3 randomPos = new Vector3
        (
            Random.Range(.5f, 1),
            Random.Range(.5f, 1),
            Random.Range(.5f, 1)
        );

         childStack.AddExplosionForce(1000f, childStack.position - randomPos, 5f);
        */
    }
}