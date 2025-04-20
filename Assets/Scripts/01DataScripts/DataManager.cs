using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

[System.Serializable]
public class PlayerData
{
    public string playerName; // 유저 이름
    public int money; // 돈 (최대 99999 제한 예정)
    public string marry; // 결혼 상대
    public List<string> inventory = new List<string>(); // 아이템 리스트
    public string lastSavedTime; // 최근 저장 시간
}

[System.Serializable]
public class Stats
{
    public int intelligence;//지능
    public int charisma;//매력
    public int elegance;//기품
    public int morality;//도덕성
    public int fame;//명성(스타성)
    public int stress;//스트레스
    public int strength;//힘
    public int magic;//마력
    public int faith;//신앙
    public int art;//예술
}

[System.Serializable]
public class EndingsData
{
    public List<string> completedEndings = new List<string>(); // 클리어한 엔딩 목록
}

[System.Serializable]
public class DayData
{
    public int year;
    public int month;
    public int week;
    public int day;
    public string weather;
}

[System.Serializable]
public class SaveData
{
    public PlayerData player;
    public Stats stats;
    public DayData date;
}

// 싱글톤 데이터 매니저
public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    public PlayerData nowPlayer = new PlayerData();
    public Stats nowStats = new Stats();
    public DayData currentDate = new DayData();
    public EndingsData endingsData = new EndingsData(); // 완료한 엔딩 목록

    private string path;
    private string saveFilePrefix = "save"; // 개별 슬롯 저장 파일명
    private string endingsFile = "endings.json"; // 완료한 엔딩 저장 파일

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            LoadCompletedEndings(); // 게임 시작 시 완료된 엔딩 불러오기
        }
        else
        {
            Destroy(gameObject);
        }

        path = Application.persistentDataPath + "/";
        Debug.Log("세이브 경로: " + path);
    }

    // ! 슬롯별 저장 (saveX.json)
    public void SaveData(int slot)
    {
        SaveData data = new SaveData
        {
            player = nowPlayer,
            stats = nowStats,
            date = currentDate
        };

        nowPlayer.lastSavedTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path + saveFilePrefix + slot + ".json", json);

        Debug.Log($"슬롯 {slot} 저장 완료! 저장 시간: {nowPlayer.lastSavedTime}");
    }

    // !슬롯별 불러오기 (saveX.json)
    public void LoadData(int slot)
    {
        string filePath = path + saveFilePrefix + slot + ".json";

        if (File.Exists(filePath))
        {
            string data = File.ReadAllText(filePath);
            SaveData loadedData = JsonUtility.FromJson<SaveData>(data);

            nowPlayer = loadedData.player;
            nowStats = loadedData.stats;
            currentDate = loadedData.date;

            Debug.Log($"슬롯 {slot} 불러오기 완료! 마지막 저장 시간: {nowPlayer.lastSavedTime}");
        }
        else
        {
            Debug.LogWarning($"세이브 슬롯 {slot}이 존재하지 않습니다!");
        }
    }

    // !완료한 엔딩을 저장 (endings.json)
    public void SaveCompletedEndings()
    {
        string json = JsonUtility.ToJson(endingsData, true);
        File.WriteAllText(path + endingsFile, json);
        Debug.Log("완료된 엔딩 저장 완료!");
    }

    // ! 완료한 엔딩을 불러오기 (endings.json)
    public void LoadCompletedEndings()
    {
        string filePath = path + endingsFile;
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            endingsData = JsonUtility.FromJson<EndingsData>(json);
            Debug.Log("완료된 엔딩 불러오기 완료!");
        }
        else
        {
            endingsData = new EndingsData(); // 파일이 없으면 새로 생성
            Debug.Log("완료된 엔딩 파일 없음. 새로 생성.");
        }
    }

    // ! 엔딩 해금 (중복 방지)
    public void AddCompletedEnding(string ending)
    {
        if (!endingsData.completedEndings.Contains(ending))
        {
            endingsData.completedEndings.Add(ending);
            SaveCompletedEndings(); // 저장
            Debug.Log($"새 엔딩 '{ending}' 획득! 총 완료된 엔딩: {endingsData.completedEndings.Count}");
        }
    }

    // ! 엔딩 체크 기능
    public int CheckEnding()
    {
        if (nowStats.intelligence >= 85 && nowStats.charisma >= 85 &&
            nowStats.elegance >= 85 && nowStats.morality >= 85 &&
            nowStats.fame >= 85 && nowStats.stress <= 20)
        {
            AddCompletedEnding("스타 엔딩");
            return 10;
        }
        else if (nowStats.magic >= 85 && nowStats.faith <= 20 && nowStats.morality <= 20)
        {
            AddCompletedEnding("마왕 엔딩");
            return 9;
        }
        else if (nowStats.strength >= 80 && nowStats.morality <= 40 &&
                 nowStats.intelligence >= 70 && nowStats.magic >= 80)
        {
            AddCompletedEnding("폭군 엔딩");
            return 8;
        }
        else if (nowStats.elegance >= 80 && nowStats.intelligence >= 75 &&
                 nowStats.morality >= 75 && nowStats.charisma >= 75)
        {
            AddCompletedEnding("왕 엔딩");
            return 7;
        }
        else if (nowStats.strength >= 75 && nowStats.elegance >= 75 &&
                 nowStats.morality >= 75 && nowStats.charisma >= 75)
        {
            AddCompletedEnding("기사 엔딩");
            return 6;
        }

        return 0; // 기본 엔딩
    }

    // ! 인벤토리 관리 (아이템 추가)
    public void AddItem(string item)
    {
        nowPlayer.inventory.Add(item);
        Debug.Log($"아이템 추가: {item}");
    }

    // ! 인벤토리 관리 (아이템 삭제)
    public void RemoveItem(string item)
    {
        if (nowPlayer.inventory.Contains(item))
        {
            nowPlayer.inventory.Remove(item);
            Debug.Log($"아이템 삭제: {item}");
        }
        else
        {
            Debug.LogWarning($"아이템 {item}이 인벤토리에 없습니다.");
        }
    }

    // ! 현재 인벤토리 출력
    public void ShowInventory()
    {
        Debug.Log("현재 인벤토리: " + string.Join(", ", nowPlayer.inventory));
    }

    // ! 버튼을 통해 저장 (슬롯 선택 가능)
    public void SaveGame(int slot)
    {
        SaveData(slot);
        Debug.Log($"[버튼 클릭] 슬롯 {slot}에 저장 완료!");
    }

    // ! 버튼을 통해 불러오기 (슬롯 선택 가능)
    public void LoadGame(int slot)
    {
        LoadData(slot);
        Debug.Log($"[버튼 클릭] 슬롯 {slot}에서 불러오기 완료!");
    }
}
