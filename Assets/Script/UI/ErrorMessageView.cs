using TMPro;
using UnityEngine;
using System.Collections;

public class ErrorMessageView : MonoBehaviour
{
    public TextMeshProUGUI Text;
    public float[] WaitTime;

    private Coroutine coAutoClose;
    private Color textColor;

    public void Open()
    {
        Text.color = Color.black;

        gameObject.SetActive(true);
        coAutoClose = StartCoroutine(AutoClose());
    }

    public void Close()
    {
        if (coAutoClose != null)
        {
            StopCoroutine(coAutoClose);
            coAutoClose = null;
        }

        gameObject.SetActive(false);
    }

    public void SetText(string text)
    {
        Text.text = text;
    }

    private IEnumerator AutoClose()
    {
        yield return new WaitForSeconds(WaitTime[0]);

        textColor = Text.color;
        while (Text.color.a > 0)
        {
            Text.color = new Color(textColor.r, textColor.g, textColor.b, Text.color.a - 0.1f);
            yield return new WaitForSeconds(WaitTime[1]);
        }

        Close();
    }
}
