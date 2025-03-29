using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SettingsData
{
    // ゲーム設定
    public string language = "jp";

    // 音声設定
    public float masterVolume = 0.5f;        // 主音声（ボイス）
    public float sfxVolume = 0.5f;           // 効果音（SE）
    public float bgmVolume = 0.5f;           // 音楽（BGM）

    // 映像設定
    public Vector2Int resolution = new(1920, 1080);     // 解像度（例: 1920x1080）
    public bool fullscreen = true;           // 全画面
    public bool vSync = true;                // 垂直同期
    public SettingsManager.ParticleQualityType particleQuality = SettingsManager.ParticleQualityType.Middle; // パーティクル効果（品質）
    public SettingsManager.BlurQualityType blurQuality = SettingsManager.BlurQualityType.Middle; // ブラーの質（品質）
    public bool frameRateLimit = true; // フレームレートの上限の有無
    public float screenScale = 0.5f; // 画面スケール
    public float brightness = 0.5f; // 明るさ

    // 演出設定
    public bool showDamage = true; // ダメージ表記
    public SettingsManager.ScreenShakeType screenShake = SettingsManager.ScreenShakeType.Middle;// 画面振動度
}
