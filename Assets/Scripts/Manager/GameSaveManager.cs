using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class GameSaveManager : MonoBehaviour
{
    public static GameSaveManager Instance;
    private string saveFilePath;

    // 타이틀에서 '이어하기'를 눌렀는지 확인하는 플래그
    public bool isContinueGame = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            saveFilePath = Path.Combine(Application.persistentDataPath, "autosave.json");
        }
        else { Destroy(gameObject); }
    }

    // 데이터가 있는지 확인 (버튼 활성화용)
    public bool HasSaveData() => File.Exists(saveFilePath);

    public void SaveGame()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        Character character = player.GetComponent<Character>();
        if (character == null) return;

        GameData data = new GameData
        {
            playerPosition = player.transform.position,
            strength = character.Strength.BaseValue,
            defense = character.Defense.BaseValue,
            currentHp = character.Health.CurrentValue,
            maxHp = character.Health.BaseValue,
            currentHunger = character.Hunger.CurrentValue,
            maxHunger = character.Hunger.BaseValue
        };

        File.WriteAllText(saveFilePath, JsonUtility.ToJson(data, true));
        Debug.Log("자동 저장 완료: " + System.DateTime.Now.ToString("HH:mm:ss"));
    }

    public void LoadGame()
    {
        if (!HasSaveData()) return;

        string json = File.ReadAllText(saveFilePath);
        GameData data = JsonUtility.FromJson<GameData>(json);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = data.playerPosition;
            player.GetComponent<Character>().LoadFromData(data);
        }
    }
}