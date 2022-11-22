using System;
using UnityEngine;

internal class RaycastPlayer : MonoBehaviour, IPlayerState, IStackController
{
    private PlayerProperties _playerProperties;
    private Rigidbody _playerRb;

    private Ray _ray;
    private Vector3 _initialScale;
    private Vector3 _finalScale;

    private float _rbMass;
    private bool _hasClicked;
    private bool _isRaycast;
    private bool _isGameover;
    private bool _isCompleted;

    public event Action<GameObject> OnHitBadStack;
    public event Action<GameObject> OnHitGoodStack;
    public event Action<GameObject> OnHitCheckpoint;

    [Header("RAYCAST SETTINGS"), Tooltip("Default value serves best result")]
    // Initial point for the raycast
    [SerializeField] private Transform hitTransform;
    [SerializeField] private LayerMask layerMask;

    [SerializeField] private float radius;
    [SerializeField] private float maxDistance;

    [Header("BOUNCE SETTINGS"), Tooltip("Default value serves best result"), Space(5)]
    [SerializeField] private float bounceHeight = .95f;
    [SerializeField] private float gravity = 1500f;
    [SerializeField] private float bounceForce = 7.5f;

    [Header("ANIMATION SETTINGS"), Tooltip("Default value serves best result"), Space(5)]
    [SerializeField] private float squashStrength = .15f;
    [SerializeField] private float squashSpeed = 15f;


    private void Awake()
    {
        _ray = new Ray();

        _playerRb = GetComponent<Rigidbody>();
        _playerProperties = GetComponent<PlayerProperties>();

        _rbMass = _playerRb.mass;
        _initialScale = transform.localScale;
    }

    private void Start()
    {
        GameManager.OnCurrentState += GameManager_OnCurrentState;
    }

    private void GameManager_OnCurrentState(GameState state)
    {
        switch (state)
        {
            case GameState.GameOver:
                _playerRb.velocity = Vector3.zero;
                _isGameover = true;
                break;

            case GameState.Completed:
                _playerRb.velocity = Vector3.zero;
                _isCompleted = true;
                break;
        }
    }

    private void FixedUpdate()
    {
        if (_isGameover)
            return;

        if (_hasClicked && !_isCompleted)
            DeepBounceOnClick();
        else
        {
            var rbPos = _playerRb.position;

            // Applies a downward force overtime
            _playerRb.AddForce(new Vector3(rbPos.x, -gravity * _rbMass * Time.fixedDeltaTime, rbPos.z));

            // Squash();
        }

        CheckRaycast();
    }

    // TODO: Improve on this
    // Shrinks ball overtime when the mouse input is not triggered
    private void Squash()
    {
        _finalScale = _initialScale;

        _finalScale.y += squashStrength * (Mathf.Sin(Time.time * squashSpeed));

        transform.localScale = _finalScale;
    }

    // TODO: Fix sporadic double collisions
    private void CheckRaycast()
    {
        _ray.origin = hitTransform.position;
        _ray.direction = -hitTransform.up;

        _isRaycast = Physics.SphereCast(_ray, radius, out var hitInfo, maxDistance, layerMask, QueryTriggerInteraction.Ignore);

        if (!_isRaycast) return;

        if (!_hasClicked)
            _playerRb.velocity = new Vector3(_playerRb.velocity.x, GetBounceHeight(), _playerRb.velocity.z);

        _playerProperties.OnPlayerBounce(_hasClicked);

        if (_isCompleted)
            return;

        CheckForTag(hitInfo.collider.gameObject);
    }

    private void CheckForWin(GameObject found)
    {
        OnHitCheckpoint?.Invoke(found);
    }

    private void CheckForTag(GameObject found)
    {
        if (_hasClicked)
        {
            if (found.CompareTag("Good"))
                OnHitGoodStack?.Invoke(found);

            else if (found.CompareTag("Bad"))
                OnHitBadStack?.Invoke(found);
        }

        if (found.CompareTag("Checkpoint"))
            CheckForWin(found);
    }

    private float GetBounceHeight()
    {
        return (bounceHeight / 2) * gravity * Time.fixedDeltaTime;
    }

    public void StartBounce(bool hasClicked)
    {
        _hasClicked = hasClicked;
    }

    private void DeepBounceOnClick()
    {
        var rbVelocity = _playerRb.velocity;

        rbVelocity = new Vector3(rbVelocity.x, -bounceForce, rbVelocity.z);

        _playerRb.velocity = rbVelocity;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(hitTransform.position, radius * 2f);
    }

    private void OnDisable()
    {
        GameManager.OnCurrentState -= GameManager_OnCurrentState;
    }
}