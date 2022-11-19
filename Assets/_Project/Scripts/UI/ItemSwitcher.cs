using Racer.SaveSystem;
using Racer.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

internal class ItemSwitcher : MonoBehaviour
{
    private int _previousItemIndex;
    private TextMeshProUGUI _itemUnlockText;

    [SerializeField, Tooltip("Score points the player achieves in other to unlock a new item")]
    private int[] levelPoints;

    /// <summary>
    /// The order in which the item-buttons are assigned to this list matters.
    /// This is to ensure that the correct ball is being saved/retrieved from the preferences.
    /// The indexes match the index of the how the items are assigned in: <see cref="GameController.AvailablePlayerIndex"/>
    /// </summary>
    [Space(5), SerializeField] private Button[] itemButtons;
    [Space(5), SerializeField] private GameObject[] playerItems;

    [Space(5), SerializeField] private RectTransform indicator;
    [Space(5), SerializeField] private Image unlockIco;

    [SerializeField] private float indicatorPosOffset;


    private void Awake()
    {
        InitUnlockedItems();

        _previousItemIndex = SaveSystem.GetData<int>("PlayerItem");

        SetIndicatorPosition(_previousItemIndex);
    }

    /// <summary>
    /// Unlocks a new player Item.
    /// Default Item[0] is unlocked by default.
    /// </summary>
    private void InitUnlockedItems()
    {
        var instance = UIController.Instance;

        for (int i = 0; i < itemButtons.Length; i++)
        {
            _itemUnlockText = itemButtons[i].GetComponentInChildren<TextMeshProUGUI>();

            if (instance.PreLevelCount >= levelPoints[i])
            {
                itemButtons[i].interactable = true;
                _itemUnlockText.text = "Unlocked";

                if (SaveSystem.GetData<bool>($"ItemButton_{i}")) continue;

                // Add the unlocked item to the save profile if absent.
                SaveSystem.SaveData($"ItemButton_{i}", true);

                unlockIco.IsEnabled(true);
            }
            else
            {
                itemButtons[i].interactable = false;
                _itemUnlockText.text = "Locked";

                SaveSystem.SaveData($"ItemButton_{i}", false);
            }
        }
    }

    public void UseItem(int index)
    {
        if (!itemButtons[index].interactable ||
            _previousItemIndex == index) return;

        SetIndicatorPosition(index);
        SaveSystem.SaveData("PlayerItem", index);
        unlockIco.IsEnabled(false);

        playerItems[index].ToggleActive(true);
        playerItems[_previousItemIndex].ToggleActive(false);
        _previousItemIndex = index;
    }

    /// <summary>
    /// Updates the pointer/Indicator position near the current item being selected.
    /// </summary>
    private void SetIndicatorPosition(int index) =>
        indicator.position = itemButtons[index].transform.position + Vector3.down * indicatorPosOffset;
}