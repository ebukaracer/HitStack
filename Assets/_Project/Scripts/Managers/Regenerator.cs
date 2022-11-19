using System.Collections;
using UnityEngine;
using UnityEngine.Events;

internal class Regenerator : MonoBehaviour
{
    private Coroutine _countdown;

    private const float Max = 100f, Min = 0f;
    private readonly float _initialAmount = Max;

    private float _currentAmount = Min;
    private bool _isDecreasing;

    public event UnityAction<float> OnManaChanged;
    public event UnityAction<float> OnPowerupDecrease;
    public event UnityAction<bool> OnUsePowerup;

    [SerializeField, Range(0f, 1f),
     Tooltip("A higher value will decrease the power-up fill image instantly when power-up is attained")]
    private float decreaseRateOnPowerup = .25f;


    /// <summary>
    /// Called when good-stacks are being destroyed.
    /// </summary>
    public void ModifyMana(float rate)
    {
        _currentAmount += rate;

        _currentAmount = Mathf.Clamp(_currentAmount, Min, Max);

        // 0-1
        var changePercent = _currentAmount / _initialAmount;

        // Notify listeners
        OnManaChanged?.Invoke(changePercent);
    }

    private void Update()
    {
        // If previously not on power-up or when player is not clicking,
        // decreases the power-up gauge overtime.
        if (_isDecreasing)
            return;

        if (_currentAmount < Max) return;

        // When the power-up gauge is full, starts a coroutine that decreases it .
        _isDecreasing = true;

        if (_countdown != null)
            StopCoroutine(_countdown);

        _countdown = StartCoroutine(nameof(CountDown));

        // True
        OnUsePowerup?.Invoke(_isDecreasing);
    }

    // Once the power-up has reached it's limit, starts decreasing instantly.
    private IEnumerator CountDown()
    {
        var current = Max;

        while (current > Min)
        {
            current -= (decreaseRateOnPowerup * 100) * Time.deltaTime;

            // 1-0
            var counter = current / Max;

            OnPowerupDecrease?.Invoke(counter);

            yield return 0;
        }

        _currentAmount = Min;

        _isDecreasing = false;

        OnUsePowerup?.Invoke(_isDecreasing);
    }
}
