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

    public Transform ErrorMessageScrollTransform;
    [SerializeField] private int errorMessageCount;
    private Queue<ErrorMessageView> errorMessageQueue = new Queue<ErrorMessageView>();

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

    public void SetErrorText(ErrorMessageType type)
    {
        if (errorMessageQueue.Count > errorMessageCount)
        {
            ErrorMessageView oldMessage = errorMessageQueue.Dequeue();
            oldMessage.Close();
        }

        ErrorMessageView newMessage = PoolManager.Pool.GetErrorMessage(type, ErrorMessageScrollTransform);
        newMessage.transform.SetAsLastSibling();
        newMessage.Open();

        errorMessageQueue.Enqueue(newMessage);
    }
}
