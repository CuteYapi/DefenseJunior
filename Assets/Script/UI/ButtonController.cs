using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    public static ButtonController Button { get; private set; }
    private void Awake()
    {
        Button = this;
    }

    public Button UpgradeButton;
    public PlayerCharacter SelectedPlayerCharacter;

    public void SetUpgradeButtonStatus(bool isUpgradable)
    {
        UpgradeButton.interactable = isUpgradable;
    }
}
