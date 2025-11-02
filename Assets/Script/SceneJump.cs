// SceneJump.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneJump : MonoBehaviour
{
    // インスペクタで名前を入れておく（Build Settingsの名前と一致させる）
    [SerializeField] string NatureScene = "NatureScene";
    [SerializeField] string RoomScene = "RoomScene";
    [SerializeField] string MarblesGame = "MarblesGame";

    public void GoNatureScene() => Load(NatureScene);
    public void GoRoomScene() => Load(RoomScene);
    public void GoMarblesGame() => Load(MarblesGame);

    public void Load(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName)) return;
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
