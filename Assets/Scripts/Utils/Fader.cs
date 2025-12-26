using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Fader : MonoBehaviour
{
    private GameObject _fader;
    private Image _faderImage;

    private void Awake()
    {
        _fader = transform.GetChild(0).gameObject;
        _faderImage = _fader.GetComponent<Image>();

        if (!_faderImage)
        {
            Debug.LogError("Fader: Fader Image component is missing!");
            enabled = false;
            return;
        }
    }

    public void FadeInScreen(Action onFadeComplete = null)
    {
        StopAllCoroutines();
        _fader.SetActive(true);
        StartCoroutine(SetColorAlphaValue(true, onFadeComplete));
    }

    public void FadeOutScreen(Action onFadeComplete = null)
    {
        StopAllCoroutines();
        _fader.SetActive(true);
        StartCoroutine(SetColorAlphaValue(false, onFadeComplete));
    }

    private void SetAlpha(float alpha)
    {
        Color newColor = _faderImage.color;
        newColor.a = alpha;
        _faderImage.color = newColor;
    }

    /// <summary>
    /// スクリーンをフェードアウト
    /// </summary>
    private IEnumerator SetColorAlphaValue(bool isFadeIn = true, Action onFadeComplete = null)
    {
        // set initial alpha
        SetAlpha(isFadeIn ? 1f : 0f);

        // change alpha
        while (isFadeIn ? _faderImage.color.a > 0f : _faderImage.color.a < 1f)
        {
            float newAlpha = _faderImage.color.a + (isFadeIn ? -.1f : .1f);
            SetAlpha(Mathf.Clamp01(newAlpha));

            yield return new WaitForSeconds(.04f);
        }

        if (isFadeIn) _fader.SetActive(false);
        onFadeComplete?.Invoke();
    }
}