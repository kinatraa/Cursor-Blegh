using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerStatsHUD : UIHud
{
    [Header("Health Bar")]
    public List<RectTransform> heartIcons = new List<RectTransform>();

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI comboText;
    
    private Coroutine _comboCoroutine;
    private Vector3 _comboOriginalScale;
    private Quaternion _comboOriginalRotation;
    
    private void Awake()
    {
        _comboOriginalScale = comboText.rectTransform.localScale;
        _comboOriginalRotation = comboText.rectTransform.localRotation;
    }
    
    public void SetupHealthBar(int maxHp)
    {
        for (int i = 0; i < heartIcons.Count; i++)
        {
            if (i < maxHp)
            {
                heartIcons[i].gameObject.SetActive(true);
                heartIcons[i].transform.GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                heartIcons[i].gameObject.SetActive(false);
            }
        }
    }
    
    public void UpdateHealthBar(int hp)
    {
        for (int i = 0; i < heartIcons.Count; i++)
        {
            if (i < hp)
            {
                heartIcons[i].transform.GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                heartIcons[i].transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }

    public void UpdateScoreText(int score)
    {
        scoreText.text = $"Score {score}";
    }

    public void UpdateWaveText(int wave)
    {
        waveText.text = $"Wave {wave}";
    }

    public void UpdateComboText(int combo)
    {
        if (combo <= 0)
        {
            comboText.gameObject.SetActive(false);
            return;
        }
        
        comboText.gameObject.SetActive(true);
        comboText.text = $"Combo x{combo}";
        if (_comboCoroutine != null)
        {
            StopCoroutine(_comboCoroutine);
            _comboCoroutine = null;
        }
        _comboCoroutine = StartCoroutine(IEComboAnimation());
    }
    
    private IEnumerator IEComboAnimation(float duration = 0.1f)
    {
        Vector3 originalScale = _comboOriginalScale;
        Quaternion originalRotation = _comboOriginalRotation;

        Vector3 targetScale = originalScale * 1.2f;
        Quaternion targetRotation = Quaternion.Euler(0, 0, -15);

        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float lerp = t / duration;

            comboText.rectTransform.localScale = Vector3.Lerp(originalScale, targetScale, lerp);
            comboText.rectTransform.localRotation = Quaternion.Lerp(originalRotation, targetRotation, lerp);

            yield return null;
        }

        t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float lerp = t / duration;

            comboText.rectTransform.localScale = Vector3.Lerp(targetScale, originalScale, lerp);
            comboText.rectTransform.localRotation = Quaternion.Lerp(targetRotation, originalRotation, lerp);

            yield return null;
        }

        comboText.rectTransform.localScale = originalScale;
        comboText.rectTransform.localRotation = originalRotation;
    }

    public override void Reset()
    {
        base.Reset();
        comboText.gameObject.SetActive(false);
        
        comboText.rectTransform.localScale = _comboOriginalScale;
        comboText.rectTransform.localRotation = _comboOriginalRotation;
    }
}
