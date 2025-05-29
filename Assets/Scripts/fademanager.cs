using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour
{
    public Image fadeImage;

    public IEnumerator FadeOut(float duration)
    {
        Color color = fadeImage.color;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, t / duration);
            fadeImage.color = color;
            yield return null;
        }
    }
}