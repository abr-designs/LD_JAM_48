using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class FadeUI : MonoBehaviour
{
    public enum POSITION
    {
        TOP,
        LEFT,
        RIGHT,
        BOTTOM,
        MIDDLE
    }

    //====================================================================================================================//

    private const string TITLE = "OddJobs.com";
    private static FadeUI _instance;
    
    public static bool IsFading { get; private set; }

    [SerializeField]
    private RectTransform fadeTransform;

    [SerializeField]
    private TMP_Text titleText;

    [SerializeField]
    private Vector2 typeTimeRange;

    private RectTransform _canvasRect;

    //Unity Functions
    //====================================================================================================================//

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        _canvasRect = GetComponentInParent<Canvas>().transform as RectTransform;
        
        //fadeTransform.anchoredPosition = GetPosition(_canvasRect, POSITION.LEFT);
    }

    //====================================================================================================================//

    public static void FadeScreen(Action onFadedCallback, Action onFadeComplete, in string overrideTitle = "")
    {
        if (IsFading) return;

        _instance?.StartCoroutine(_instance?.GenericFadeCoroutine(onFadedCallback,onFadeComplete, overrideTitle));
    }

    private IEnumerator GenericFadeCoroutine(Action onFadedCallback, Action onFadeComplete, string overrideTitle)
    {
        IsFading = true;
        
        titleText.text = string.Empty;
        fadeTransform.gameObject.SetActive(true);

        yield return new WaitForSeconds(Random.Range(0.1f, 0.25f));

        //yield return StartCoroutine(GenericFadeCoroutine(_canvasRect, fadeTransform, POSITION.MIDDLE, POSITION.MIDDLE));
        yield return new WaitForSeconds(1f);
        
        
        yield return new WaitForSeconds(Random.Range(0.1f, 0.3f));
        
        yield return StartCoroutine(TypingCoroutine(overrideTitle));
        
        onFadedCallback?.Invoke();

        yield return new WaitForSeconds(Random.Range(1f, 2f));
        
        //yield return StartCoroutine(GenericFadeCoroutine(_canvasRect, fadeTransform,POSITION.MIDDLE, POSITION.MIDDLE));
        yield return new WaitForSeconds(1f);
        
        fadeTransform.gameObject.SetActive(false);
        IsFading = false;
        
        onFadeComplete?.Invoke();
    }
    

    public static IEnumerator GenericFadeCoroutine(RectTransform canvasRect, RectTransform target,POSITION start, POSITION end, float seconds = 1)
    {
        var startPosition = GetPosition(canvasRect, start);
        var endPosition = GetPosition(canvasRect, end);

        float t = 0;
        while (t / seconds < 1f)
        {
            target.anchoredPosition = Vector2.Lerp(startPosition, endPosition, t / seconds);
            
            t += Time.deltaTime;
            
            yield return null;
        }
    }

    private IEnumerator TypingCoroutine(string overrideTitle)
    {
        var characters = string.IsNullOrEmpty(overrideTitle) ? TITLE.ToCharArray() : overrideTitle.ToCharArray();
        var index = 0;

        while (index < characters.Length)
        {
            yield return new WaitForSeconds(Random.Range(typeTimeRange.x, typeTimeRange.y));

            titleText.text += characters[index];
            AudioController.PlaySound(AudioController.SFX.BEEP, 0.4f);

            index++;
        }
    }

    private static Vector2 GetPosition(in RectTransform canvasRect, in POSITION position)
    {
        switch (position)
        {
            case POSITION.TOP:
                return new Vector2(0f, canvasRect.sizeDelta.y);
            case POSITION.LEFT:
                return new Vector2(-canvasRect.sizeDelta.x, 0);
            case POSITION.RIGHT:
                return new Vector2(canvasRect.sizeDelta.x, 0);
            case POSITION.BOTTOM:
                return new Vector2(0f, -canvasRect.sizeDelta.y);
            case POSITION.MIDDLE:
                return Vector2.zero;
            default:
                throw new ArgumentOutOfRangeException(nameof(position), position, null);
        }
    }
}
