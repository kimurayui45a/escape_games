//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
//{
//    static T instance;
//    static bool isQuitting = false;

//    public static T Instance => GetOrCreateInstance();

//    static T GetOrCreateInstance()
//    {
//        if (isQuitting)
//        {
//            // アプリケーション終了時はアクセスできないようにする
//            return null;
//        }
//        if (instance == null)
//        {
//            instance = FindFirstObjectByType<T>();
//            if (instance == null)
//            {
//                var obj = new GameObject(typeof(T).ToString());
//                instance = obj.AddComponent<T>();
//                DontDestroyOnLoad(obj);
//            }
//        }
//        return instance;
//    }
//    protected virtual void OnCreateSingleton()
//    {
//        // シングルトンが作成時に一度だけ呼ばれる
//        // AwakeやStartだと意図しない処理が発生する場合がある
//    }

//    protected virtual void OnDestroySingleton()
//    {
//        // シングルトンが削除されるときに呼ばれる
//        // onDestroyを直接使用するとinstanceされたもの以外でも処理はいるためシングルトンから呼び出す
//    }

//    void Awake()
//    {
//        if (instance != null && instance != this)
//        {
//            // 既にインスタンスが存在する場合は破棄する
//            Destroy(gameObject);
//        }
//        else
//        {
//            // インスタンスがない場合は生成処理
//            instance = this as T;
//            DontDestroyOnLoad(instance.gameObject);

//            OnCreateSingleton();
//        }
//    }
//    void OnDestroy()
//    {
//        if (instance == this as T)
//        {
//            OnDestroySingleton();
//            instance = null;
//        }
//    }

//    void OnApplicationQuit()
//    {
//        isQuitting = true;
//    }

//}
