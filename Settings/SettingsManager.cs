using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using MyGame;
using UnityEngine;
using Zenject;

public class SettingsManager : MonoBehaviour, IInitializableSettingsManager
{
    string SettingDataPath => Path.Combine(ApplicationManager.ConfigDirectoryPath, "SettingsData");
    public static SettingsManager Instance { get; private set; }
    [Inject]IKeyBindingsController m_keyBindingsController;

    [SerializeField] SettingsData settingsData;

    public string Lnaguage => settingsData.language;
    public event Action<string> OnLanguageChanged;
    public float MasterVolume => settingsData.masterVolume;
    public event Action<float> OnMasterVolumeChanged;
    public float SFXVolume => settingsData.sfxVolume;
    public event Action<float> OnSFXVolumeChanged;
    public float BGMVolume => settingsData.bgmVolume;
    public event Action<float> OnBGMVolumeChanged;

    public Vector2Int Resolution => settingsData.resolution;
    public event Action<Vector2Int> OnResolutionChanged;
    public bool Fullscreen => settingsData.fullscreen;
    public event Action<bool> OnFullscreenChanged;
    public bool VSync => settingsData.vSync;
    public event Action<bool> OnVSyncChanged;
    public ParticleQualityType ParticleQuality => settingsData.particleQuality;
    public event Action<ParticleQualityType> OnParticleQualityChanged;
    public BlurQualityType BlurQuality => settingsData.blurQuality;
    public event Action<BlurQualityType> OnBlurQualityChanged;
    public bool FrameRateLimit => settingsData.frameRateLimit;
    public event Action<bool> OnFrameRateLimitChanged;
    public float ScreenScale => settingsData.screenScale;
    public event Action<float> OnScreenScaleChanged;
    public float Brightness => settingsData.brightness;
    public event Action<float> OnBrightnessChanged;

    public bool ShowDamage => settingsData.showDamage;
    public event Action<bool> OnShowDamageChanged;
    public ScreenShakeType ScreenShake => settingsData.screenShake;
    public event Action<ScreenShakeType> OnScreenShakeChanged;

    public void OnAppStart()
    {
        Instance = this;
        settingsData = new();
        // Debug.Log($"KeyBindingsController: {m_keyBindingsController}");
        // Debug.Log($"KeyBindingsController is null: {m_keyBindingsController == null}");
    }

    public IKeyBindingsController GetKeyBindingsController()
    {
        return m_keyBindingsController;
    }

    /// <summary>
    /// 設定をセーブ。戻るキーを押したときに実行されるようにする。なおキーバインドは含まれない。
    /// </summary>
    public void Save()
    {
        EditFile.SaveCompressedJson(SettingDataPath, settingsData);
    }

    public void SetLanguage(string language)
    {
        settingsData.language = language;
        OnLanguageChanged?.Invoke(language);
    }

    public void SetMasterVolume(float volume)
    {
        settingsData.masterVolume = volume;
        OnMasterVolumeChanged?.Invoke(volume);
    }

    public void SetSFXVolume(float volume)
    {
        settingsData.sfxVolume = volume;
        OnSFXVolumeChanged?.Invoke(volume);
    }

    public void SetBGMVolume(float volume)
    {
        settingsData.bgmVolume = volume;
        OnBGMVolumeChanged?.Invoke(volume);
    }

    public void SetResolution(Vector2Int resolution)
    {
        settingsData.resolution = resolution;
        OnResolutionChanged?.Invoke(resolution);
    }

    public void SetFullscreen(bool isOn)
    {
        settingsData.fullscreen = isOn;
        OnFullscreenChanged?.Invoke(isOn);
    }
    public void SetVSync(bool isOn)
    {
        settingsData.vSync = isOn;
        OnVSyncChanged?.Invoke(isOn);
    }
    public void SetParticleQuality(ParticleQualityType type)
    {
        settingsData.particleQuality = type;
        OnParticleQualityChanged?.Invoke(type);
    }
    public void SetBlurQuality(BlurQualityType type)
    {
        settingsData.blurQuality = type;
        OnBlurQualityChanged?.Invoke(type);
    }
    public void SetFrameRateLimit(bool isOn)
    {
        settingsData.frameRateLimit = isOn;
        OnFrameRateLimitChanged?.Invoke(isOn);
    }
    public void SetScreenScale(float screenScale)
    {
        settingsData.screenScale = screenScale;
        OnScreenScaleChanged?.Invoke(screenScale);
    }
    /// <summary>
    /// UIで5%区切りにする
    /// </summary>
    /// <param name="brightness"></param>
    public void SetBrightness(float brightness)
    {
        settingsData.brightness = brightness;
        OnBrightnessChanged?.Invoke(brightness);
    }
    public void SetShowDamage(bool isOn)
    {
        settingsData.showDamage = isOn;
        OnShowDamageChanged?.Invoke(isOn);
    }
    public void SetScreenShake(ScreenShakeType type)
    {
        settingsData.screenShake = type;
        OnScreenShakeChanged?.Invoke(type);
    }

    public enum ParticleQualityType
    {
        Low,
        Middle,
        High,
    }
    public enum BlurQualityType
    {
        Low,
        Middle,
        High,
    }
    public enum ScreenShakeType
    {
        Low,
        Middle,
        High,
    }
}
