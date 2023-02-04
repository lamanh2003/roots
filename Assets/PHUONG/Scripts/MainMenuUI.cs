using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;
using Controller;

public class MainMenuUI : MonoBehaviour
{
    public RectTransform title1;
    public RectTransform title2;
    public RectTransform columnPink;
    public RectTransform columnBlue;
    public RectTransform columnYellow;
    public RectTransform columnGreen;
    public RectTransform columnRed;
    public Button playBtn;
    public Button quitBtn;
    public RectTransform aboutUsRect;
    public RectTransform aboutUsDescriptionPanel;

    public AudioSource sfxAudioSource;
    public AudioSource musicAudioSource;
    public AudioClip textEnterClip;
    public AudioClip buttonHoverClip;
    public List<AudioClip> musicClips;
    public Color textHoverColor;
    
    private void Awake()
    {
        playBtn.onClick.AddListener(OnClickPlayButton);
        quitBtn.onClick.AddListener(OnClickQuitButton);
    }

    private void Start()
    {
        musicAudioSource.PlayOneShot(musicClips[Random.Range(0, musicClips.Count)]);
        Sequence s1 = DOTween.Sequence();
        s1.Append(columnPink.DOScaleY(1, 1f).SetEase(Ease.InOutQuad));
        s1.Join(columnBlue.DOScaleY(1, 1f).SetEase(Ease.InOutQuad).SetDelay(0.15f));
        s1.Join(columnYellow.DOScaleY(1, 1f).SetEase(Ease.InOutQuad).SetDelay(0.3f));
        s1.AppendInterval(0.15f);
        s1.Join(title1.DOAnchorPosX(35f, 1f).SetEase(Ease.OutBounce));
        s1.Join(title2.DOAnchorPosX(35f, 1f).SetEase(Ease.OutBounce).OnStart(delegate
        {
            sfxAudioSource.PlayOneShot(textEnterClip);
        }));
        
        s1.Append(columnGreen.DOScaleY(1, 1f).SetEase(Ease.InOutQuad));
        s1.Join(columnRed.DOScaleY(1, 1f).SetEase(Ease.InOutQuad).SetDelay(0.15f));
        s1.Join(playBtn.GetComponent<RectTransform>().DOAnchorPosX(275f, 1f).SetEase(Ease.OutBounce).OnStart(delegate
        {
            sfxAudioSource.PlayOneShot(textEnterClip);
        }));
        s1.Join(quitBtn.GetComponent<RectTransform>().DOAnchorPosX(275f, 1f).SetEase(Ease.OutBounce).SetDelay(0.15f));
        s1.Join(aboutUsRect.DOAnchorPosY(35f, 1f).SetEase(Ease.InOutQuad).SetDelay(0.15f));
    }

    private void OnClickPlayButton()
    {
        GameController.Singleton.soundController.PlayClickSound();
        SceneManager.LoadSceneAsync("GameScene");
    }

    private void OnClickQuitButton()
    {
            GameController.Singleton.soundController.PlayClickSound();
            Application.Quit();
    }

    public void OnPlayButtonMouseEnter()
    {
        sfxAudioSource.PlayOneShot(buttonHoverClip);
        playBtn.GetComponentInChildren<TextMeshProUGUI>().color = textHoverColor;
    }
    
    public void OnPlayButtonMouseExit()
    {
        playBtn.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
    }
    
    public void OnQuitButtonMouseEnter()
    {
        sfxAudioSource.PlayOneShot(buttonHoverClip);
        quitBtn.GetComponentInChildren<TextMeshProUGUI>().color = textHoverColor;
    }
    
    public void OnQuitButtonMouseExit()
    {
        quitBtn.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
    }

    public void OnAboutUsButtonMouseEnter()
    {
        sfxAudioSource.PlayOneShot(buttonHoverClip);
        aboutUsRect.DOShakePosition(0.3f, 3.5f);
        aboutUsDescriptionPanel.DOScaleX(1f, 0.45f);
        aboutUsDescriptionPanel.DOScaleY(1f, 0.55f);
    }
    
    public void OnAboutUsButtonMouseExit()
    {
        aboutUsDescriptionPanel.DOScaleX(0f, 0.45f);
        aboutUsDescriptionPanel.DOScaleY(0f, 0.55f);
    }
}
