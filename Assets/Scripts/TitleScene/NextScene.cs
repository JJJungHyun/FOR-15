using UnityEngine;
using UnityEngine.SceneManagement;

public class NextScene : MonoBehaviour
{
    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        Debug.Log("다음 씬");
    }
    public void OnClickStart()
    {
        // 스크립트가 붙은 SceneTransitioner 인스턴스를 찾아 씬 전환 명령
        //SceneLoad.Instance.ChangeScene("SimpleScene");
    }
}
