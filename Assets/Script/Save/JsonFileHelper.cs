using System;
using System.IO;
using System.Text;
using UnityEngine;

/// <summary>
/// 指定したクラスを Json 形式の外部ファイルとして保存・ロードするヘルパークラス
/// 保存先: Application.persistentDataPath
/// </summary>
public static class JsonFileHelper
{
    // 任意: サブフォルダにまとめたい場合
    private const string FolderName = "SaveData";

    /// <summary>
    /// 指定したキーのデータファイルが存在しているか確認
    /// </summary>
    public static bool ExistsData(string key)
    {
        return File.Exists(GetFilePath(key));
    }

    /// <summary>
    /// 指定されたオブジェクトのデータを Json ファイルとしてセーブ
    /// （途中で落ちても壊れにくいよう、一時ファイル→リネーム、バックアップを作る）
    /// </summary>
    public static void SaveObjectData<T>(string key, T obj)
    {
        try
        {
            string json = JsonUtility.ToJson(obj, prettyPrint: true);

            string path = GetFilePath(key);
            string dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            string tmpPath = path + ".tmp";
            string bakPath = path + ".bak";

            // 一時ファイルへ書き込み
            File.WriteAllText(tmpPath, json, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

            // 既存があればバックアップへ退避
            if (File.Exists(path))
            {
                // 既存bakを潰してから作り直す（運用方針次第で変更可）
                if (File.Exists(bakPath)) File.Delete(bakPath);
                File.Move(path, bakPath);
            }

            // tmpを本番へ
            File.Move(tmpPath, path);

            // 成功したらbakを消す（残したいなら消さない）
            if (File.Exists(bakPath)) File.Delete(bakPath);
        }
        catch (Exception e)
        {
            Debug.LogError($"[JsonFileHelper] Save failed. key='{key}' error={e}");
        }
    }

    /// <summary>
    /// 指定されたオブジェクトのデータをロード
    /// 存在しない/失敗した場合は defaultValue を返す
    /// </summary>
    public static T LoadObjectData<T>(string key, T defaultValue = default)
    {
        string path = GetFilePath(key);

        if (!File.Exists(path))
        {
            return defaultValue;
        }

        try
        {
            string json = File.ReadAllText(path, Encoding.UTF8);
            return JsonUtility.FromJson<T>(json);
        }
        catch (Exception e)
        {
            Debug.LogError($"[JsonFileHelper] Load failed. key='{key}' path='{path}' error={e}");
            return defaultValue;
        }
    }

    /// <summary>
    /// 指定されたキーのデータを削除
    /// </summary>
    public static void RemoveObjectData(string key)
    {
        string path = GetFilePath(key);
        if (File.Exists(path)) File.Delete(path);

        string bakPath = path + ".bak";
        if (File.Exists(bakPath)) File.Delete(bakPath);

        Debug.Log($"[JsonFileHelper] Deleted: key='{key}'");
    }

    /// <summary>
    /// セーブフォルダ配下を全削除（FolderNameを使っている前提）
    /// </summary>
    public static void AllClearSaveData()
    {
        string dir = GetSaveDirectory();
        if (Directory.Exists(dir)) Directory.Delete(dir, recursive: true);
        Debug.Log("[JsonFileHelper] All save data deleted.");
    }

    // -------------------------
    // 内部ユーティリティ
    // -------------------------

    private static string GetSaveDirectory()
    {
        return Path.Combine(Application.persistentDataPath, FolderName);
    }

    private static string GetFilePath(string key)
    {
        // key をファイル名として安全な形にする（OS的に不正な文字を置換）
        string safeName = SanitizeFileName(key);
        return Path.Combine(GetSaveDirectory(), safeName + ".json");
    }

    private static string SanitizeFileName(string name)
    {
        foreach (char c in Path.GetInvalidFileNameChars())
        {
            name = name.Replace(c, '_');
        }
        return name;
    }
}
