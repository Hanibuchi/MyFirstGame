using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SettingsUI : UIPageBase
{
    // ゲーム設定
    [SerializeField] TMP_Dropdown language;

    // 音声設定
    [SerializeField] Slider masterVolume;
    [SerializeField] TMP_Text mvSliderText;
    [SerializeField] Slider sfxVolume;
    [SerializeField] TMP_Text sfxvSliderText;
    [SerializeField] Slider bgmVolume;
    [SerializeField] TMP_Text bgmvSliderText;

    // 映像設定
    [SerializeField] TMP_Dropdown resolution;
    [SerializeField] Toggle fullscreen;
    [SerializeField] Toggle vSync;
    [SerializeField] Slider particleQuality;
    [SerializeField] TMP_Text pqSliderText;
    [SerializeField] Slider blurQuality;
    [SerializeField] TMP_Text bqSliderText;
    [SerializeField] Toggle frameRateLimit;
    [SerializeField] Slider screenScale;
    [SerializeField] TMP_Text ssSliderText;
    [SerializeField] Slider brightness;
    [SerializeField] TMP_Text bSliderText;

    // 演出設定
    [SerializeField] Toggle showDamage;
    [SerializeField] Slider screenShakeInstensity;
    [SerializeField] TMP_Text screenShakeSliderText;

    protected override void Awake()
    {
        base.Awake();
        language.onValueChanged.AddListener(SetLanguage);

        masterVolume.onValueChanged.AddListener(SetMasterVolume);
        SettingsManager.Instance.OnMasterVolumeChanged += SetMVST;
        sfxVolume.onValueChanged.AddListener(SetSFXVolume);
        SettingsManager.Instance.OnSFXVolumeChanged += SetSFXVST;
        bgmVolume.onValueChanged.AddListener(SetBGMVolume);
        SettingsManager.Instance.OnBGMVolumeChanged += SetBGMVST;

        resolution.onValueChanged.AddListener(SetResolution);
        fullscreen.onValueChanged.AddListener(SettingsManager.Instance.SetFullscreen);
        vSync.onValueChanged.AddListener(SettingsManager.Instance.SetVSync);
        particleQuality.onValueChanged.AddListener(SetParticleQuality);
        SettingsManager.Instance.OnParticleQualityChanged += SetPQST;
        blurQuality.onValueChanged.AddListener(SetBlurQuality);
        SettingsManager.Instance.OnBlurQualityChanged += SetBQST;
        frameRateLimit.onValueChanged.AddListener(SettingsManager.Instance.SetFrameRateLimit);
        screenScale.onValueChanged.AddListener(SettingsManager.Instance.SetScreenScale);
        brightness.onValueChanged.AddListener(SetBrightness);
        SettingsManager.Instance.OnBrightnessChanged += SetBST;

        showDamage.onValueChanged.AddListener(SettingsManager.Instance.SetShowDamage);
        screenShakeInstensity.onValueChanged.AddListener(SetScreenShake);
        SettingsManager.Instance.OnScreenShakeChanged += SetScreenShakeST;
    }

    public override void Show()
    {
        base.Show();
        SetValues();
    }
    protected override void OnCloseCompleted()
    {
        base.OnCloseCompleted();
        if (MyInputSystem.Instance.State != MyInputSystem.StateType.None)
        {
            MyInputSystem.Instance.CancelRebinding();
        }
    }
    void SetValues()
    {
        RegisterLanguage();

        masterVolume.value = Map0to1To0to20(SettingsManager.Instance.MasterVolume);
        SetMVST(SettingsManager.Instance.MasterVolume);
        sfxVolume.value = Map0to1To0to20(SettingsManager.Instance.SFXVolume);
        SetSFXVST(SettingsManager.Instance.SFXVolume);
        bgmVolume.value = Map0to1To0to20(SettingsManager.Instance.BGMVolume);
        SetBGMVST(SettingsManager.Instance.BGMVolume);

        RegisterResolution();
        fullscreen.isOn = SettingsManager.Instance.Fullscreen;
        vSync.isOn = SettingsManager.Instance.VSync;
        particleQuality.value = (int)SettingsManager.Instance.ParticleQuality;
        SetPQST(SettingsManager.Instance.ParticleQuality);
        blurQuality.value = (int)SettingsManager.Instance.BlurQuality;
        SetBQST(SettingsManager.Instance.BlurQuality);
        frameRateLimit.isOn = SettingsManager.Instance.FrameRateLimit;
        screenScale.value = SettingsManager.Instance.ScreenScale;
        brightness.value = Map0to1To0to20(SettingsManager.Instance.Brightness);
        SetBST(SettingsManager.Instance.Brightness);

        showDamage.isOn = SettingsManager.Instance.ShowDamage;
        screenShakeInstensity.value = (int)SettingsManager.Instance.ScreenShake;
        SetScreenShakeST(SettingsManager.Instance.ScreenShake);
    }

    void RegisterLanguage()
    {
        // ここでlanguageの登録をする。
    }
    public void SetLanguage(int num)
    {
        // ここでリストを使う。
        string language = num switch
        {
            0 => "ja",
            _ => "en",
        };
        SettingsManager.Instance.SetLanguage(language);
    }

    public void SetMasterVolume(float num)
    {
        SettingsManager.Instance.SetMasterVolume(Map0to20To0to1(num));
    }
    // Set○○STはSliderTextに値をセットするメソッド。SettingsManagerのActionに登録して値を変える。
    void SetMVST(float num)
    {
        mvSliderText.text = ((int)(num * 100)).ToString();
    }
    public void SetSFXVolume(float num)
    {
        SettingsManager.Instance.SetSFXVolume(Map0to20To0to1(num));
    }
    void SetSFXVST(float num)
    {
        sfxvSliderText.text = ((int)(num * 100)).ToString();
    }
    public void SetBGMVolume(float num)
    {
        SettingsManager.Instance.SetBGMVolume(Map0to20To0to1(num));
    }
    void SetBGMVST(float num)
    {
        bgmvSliderText.text = ((int)(num * 100)).ToString();
    }

    void RegisterResolution()
    {
        // ここで解像度の登録をする。
    }
    public void SetResolution(int num)
    {
        Vector2Int resolution = num switch
        {
            _ => new(1960, 1080),
        };
        SettingsManager.Instance.SetResolution(resolution);
    }
    public void SetParticleQuality(float num)
    {
        SettingsManager.ParticleQualityType type = num switch
        {
            0 => SettingsManager.ParticleQualityType.Low,
            1 => SettingsManager.ParticleQualityType.Middle,
            2 => SettingsManager.ParticleQualityType.High,
            _ => SettingsManager.ParticleQualityType.Middle,
        };
        SettingsManager.Instance.SetParticleQuality(type);
    }
    void SetPQST(SettingsManager.ParticleQualityType type)
    {
        pqSliderText.text = type.ToString();
    }
    public void SetBlurQuality(float num)
    {
        SettingsManager.BlurQualityType type = num switch
        {
            0 => SettingsManager.BlurQualityType.Low,
            1 => SettingsManager.BlurQualityType.Middle,
            2 => SettingsManager.BlurQualityType.High,
            _ => SettingsManager.BlurQualityType.Middle,
        };
        SettingsManager.Instance.SetBlurQuality(type);
    }
    void SetBQST(SettingsManager.BlurQualityType type)
    {
        bqSliderText.text = type.ToString();
    }
    public void SetBrightness(float num)
    {
        SettingsManager.Instance.SetBrightness(Map0to20To0to1(num));
    }
    void SetBST(float num)
    {
        bSliderText.text = ((int)(num * 100)).ToString();
    }
    public void SetScreenShake(float num)
    {
        SettingsManager.ScreenShakeType type = num switch
        {
            0 => SettingsManager.ScreenShakeType.Low,
            1 => SettingsManager.ScreenShakeType.Middle,
            2 => SettingsManager.ScreenShakeType.High,
            _ => SettingsManager.ScreenShakeType.Middle,
        };
        SettingsManager.Instance.SetScreenShake(type);
    }
    void SetScreenShakeST(SettingsManager.ScreenShakeType type)
    {
        screenShakeSliderText.text = type.ToString();
    }

    /// <summary>
    /// 0から20の値を0から1までの値に変換する。
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    float Map0to20To0to1(float num)
    {
        return num * 0.05f;
    }
    float Map0to1To0to20(float num)
    {
        return num * 20f;
    }
}
