using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NameInputHandler : SingletonDestroy<NameInputHandler>
{
    [Header("References")]
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private Image inputFieldBg;
    [SerializeField] private RectTransform rectTransform;

    [Header("Error Effect Settings")]
    [SerializeField] private Color errorColor = new Color(1f, 0.5f, 0.5f);
    [SerializeField] private float shakeDuration = 0.25f;
    [SerializeField] private float shakeStrength = 0.2f;

    private Vector2 originalPos;
    private Color originalColor;

    private void Awake()
    {
        if (nameInputField == null)
            nameInputField = GetComponent<TMP_InputField>();

        if (inputFieldBg == null)
            inputFieldBg = GetComponent<Image>();

        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();
    }

    void Start()
    {
        this.Init();
    }

    private void OnEnable()
    {
        this.Init();
    }

    protected virtual void Init()
    {
        originalPos = rectTransform.anchoredPosition;
        originalColor = inputFieldBg.color;
        if (string.IsNullOrEmpty(PlayerPrefs.GetString("PlayerName"))) return;
        nameInputField.text = PlayerPrefs.GetString("PlayerName");
    }

    public virtual void OnEndEdit()
    {
        string playerName = nameInputField.text;
        if (!string.IsNullOrEmpty(playerName))
        {
            LeaderboardController.Instance.RegisterPlayer(playerName);
            return;
        }
        nameInputField.text = PlayerPrefs.GetString("PlayerName");
    }

    public void PlayErrorEffect()
    {
        StopAllCoroutines();
        StartCoroutine(ErrorEffectCoroutine());
    }

    private IEnumerator ErrorEffectCoroutine()
    {
        if (inputFieldBg != null)
            inputFieldBg.color = errorColor;

        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeStrength;
            rectTransform.anchoredPosition = originalPos + new Vector2(x, 0);

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        rectTransform.anchoredPosition = originalPos;
        inputFieldBg.color = originalColor;
    }
}
