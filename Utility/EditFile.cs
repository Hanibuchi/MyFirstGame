using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

public class EditFile : MonoBehaviour
{
    /// <summary>
    /// 任意のクラスをjsonに変換しfilePathに.jsonファイルを生成。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="filePath"></param>
    /// <param name="obj"></param>
    public static void SaveJson<T>(string filePath, T obj)
    {
        if (!filePath.EndsWith(".json"))
        {
            filePath += ".json";
        }
        string json = ToJson(obj);
        File.WriteAllText(filePath, json);
        // Debug.Log($"Data saved to: {filePath}");
    }

    const string compressedJsonExt = ".json.br";
    /// <summary>
    /// 任意のクラスをjsonに変換しbrotli圧縮，filePathに.json.brファイルを生成。
    /// </summary>
    public static void SaveCompressedJson<T>(string filePath, T obj)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        filePath = GetCompressedJsonName(filePath);
        string json = ToJson(obj);
        CompressAndSaveJson(filePath, json);
    }
    /// <summary>
    /// filePathの.json.brファイルを任意のクラスに変換。
    /// </summary>
    public static T LoadCompressedJson<T>(string filePath) where T : class
    {
        string json = ReadAndDecompressJson(filePath);
        if (json == "")
            return null;
        return FromJson<T>(json);
    }

    public static void CompressAndSaveJson(string filePath, string json)
    {
        using FileStream fileStream = new(filePath, FileMode.Create);
        using BrotliStream brotliStream = new(fileStream, CompressionMode.Compress);
        using StreamWriter writer = new(brotliStream, Encoding.UTF8);
        writer.Write(json);
    }

    public static string ReadAndDecompressJson(string filePath)
    {
        filePath = GetCompressedJsonName(filePath);
        if (!File.Exists(filePath))
        {
            return "";
        }
        using FileStream fileStream = new(filePath, FileMode.Open);
        using BrotliStream brotliStream = new(fileStream, CompressionMode.Decompress);
        using StreamReader reader = new(brotliStream, Encoding.UTF8);
        return reader.ReadToEnd();
    }


    public static string GetCompressedJsonName(string fileName)
    {
        if (!fileName.EndsWith(compressedJsonExt))
        {
            fileName += compressedJsonExt;
        }
        return fileName;
    }

    static string ToJson<T>(T obj)
    {
        var setting = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
        return JsonConvert.SerializeObject(obj, setting);
    }

    static T FromJson<T>(string json)
    {
        return JsonConvert.DeserializeObject<T>(json);
    }
}
