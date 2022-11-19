using UnityEngine;

internal class CollisionPlayer : CheckCollision
{
    private Rigidbody _rb;

    private PlayerProperties _playerProperties;

    private float _mass;

    [SerializeField] private float gravity = 1500;

    [SerializeField] private float bounceSpeed = .35f;

    [SerializeField] private float bounceForce = .4f;

    private void Awake()
    {
        _playerProperties = GetComponent<PlayerProperties>();

        _rb = GetComponent<Rigidbody>();

        _mass = _rb.mass;
    }

    public float BounceHeight => gravity * _mass * Time.fixedDeltaTime;

    private void FixedUpdate()
    {
        if (HasClicked)
            DeepBounce();
        else
            BounceDown();
    }

    public override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);

        BounceUp();

        _playerProperties.OnPlayerBounce(HasClicked);
    }


    private void BounceUp()
    {
        _rb.velocity = new Vector3(_rb.velocity.x, BounceHeight * bounceSpeed, _rb.velocity.z);
    }

    private void BounceDown()
    {
        _rb.AddForce(new Vector3(_rb.position.x, -BounceHeight, _rb.position.z));
    }

    private void DeepBounce()
    {
        _rb.velocity = new Vector3(_rb.velocity.x, -BounceHeight * bounceForce, _rb.velocity.z);
    }
}