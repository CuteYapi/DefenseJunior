using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TextController : MonoBehaviour
{
    public static TextController Text { get; private set; }
    private void Awake()
    {
        Text = this;
    }

    public TextMeshProUGUI GoldText;
    public TextMeshProUGUI LifeText;
    public TextMeshProUGUI StageText;

    public Transform MessageScrollTransform;
    [SerializeField] private int messageCount;
    private Queue<MessageView> messageQueue = new Queue<MessageView>();

    public TextMeshProUGUI PlayerNameText;
    public TextMeshProUGUI PlayerCostText;
    public TextMeshProUGUI PlayerDamageText;
    public TextMeshProUGUI PlayerAttackCooldownText;

    public void SetGoldText(int goldAmount)
    {
        GoldText.text = $"Gold : {goldAmount}";
    }

    public void SetLifeText(int lifeAmount)
    {
        LifeText.text = $"Life : {lifeAmount}";
    }

    public void SetStageText(int stageAmount)
    {
        StageText.text = $"{stageAmount} Stage";
    }

    public void SetMessageText(MessageType type)
    {
        if (messageQueue.Count > messageCount)
        {
            MessageView oldMessage = messageQueue.Dequeue();
            oldMessage.Close();
        }

        MessageView newMessage = PoolManager.Pool.GetMessage(type, MessageScrollTransform);
        newMessage.transform.SetAsLastSibling();
        newMessage.Open();

        messageQueue.Enqueue(newMessage);
    }

    public void SetPlayerInformationText(string name, PlayerCharacterData data)
    {
        PlayerNameText.text = name;
        PlayerCostText.text = data.BuildCost.ToString();
        PlayerDamageText.text = data.AttackDamage.ToString();
        PlayerAttackCooldownText.text = data.AttackCooldown.ToString();
    }
}
