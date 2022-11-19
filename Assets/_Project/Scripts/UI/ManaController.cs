using Racer.Utilities;
using UnityEngine;
using UnityEngine.UI;

internal class ManaController : MonoBehaviour
{
    private Color _powerupColor = Color.red, _defaultColor;

    [SerializeField] private Regenerator regenerator;
    [SerializeField] private GameObject manaImageRoot;
    [SerializeField] private Image manaImage;


    private void Awake()
    {
        manaImage.fillAmount = 0;

        _defaultColor = manaImage.color;
    }

    private void Start()
    {
        regenerator.OnManaChanged += Regenerator_OnManaChanged;
        regenerator.OnPowerupDecrease += Regenerator_OnPowerupDecrease;
        regenerator.OnUsePowerup += Regenerator_OnUsePowerup;
    }

    private void Regenerator_OnUsePowerup(bool value)
    {
        manaImage.color = value ? _powerupColor : _defaultColor;
    }

    private void Regenerator_OnPowerupDecrease(float amount)
    {
        // Decreases the fill image(1-0) when the player has attained power-up.
        manaImage.fillAmount = amount;
    }

    private void Regenerator_OnManaChanged(float amount)
    {
        if (amount <= .05f)
        {
            manaImageRoot.ToggleActive(false);

            return;
        }

        // Increases the fill image(0-1) when the player has not attained power-up.
        manaImageRoot.ToggleActive(true);

        manaImage.fillAmount = amount;
    }

    private void OnDisable()
    {
        regenerator.OnManaChanged -= Regenerator_OnManaChanged;
        regenerator.OnPowerupDecrease -= Regenerator_OnPowerupDecrease;
        regenerator.OnUsePowerup -= Regenerator_OnUsePowerup;
    }
}
