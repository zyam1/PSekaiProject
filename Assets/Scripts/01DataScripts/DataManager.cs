using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

//  플레이어 데이터 (이름, 돈, 결혼 상대, 인벤토리, 저장 시간 포함)
[System.Serializable]
public class PlayerData
{
    public string playerName;       // 유저 이름
    public int money;               // 돈 (최대 99999 제한 예정)
    public string marry;            // 결혼 상대
    public List<string> inventory = new List<string>(); // 아이템 리스트
    public string lastSavedTime;    // 최근 저장 시간
}

//  스탯 데이터 (지능, 매력, 체력 등)
[System.Serializable]
public class Stats
{
    public int intelligence;
    public int charisma;
    public int elegance;
    public int morality;
    public int fame;
    public int stress;
    public int strength;
    public int magic;
    public int faith;
    public int art;
}

//  엔딩 데이터 (도달한 엔딩 리스트)
[System.Serializable]
public class EndingData
{
    public List<string> completedEndings = new List<string>(); // 클리어한 엔딩 목록
}

//  날짜 데이터 (게임 날짜, 날씨 등)
[System.Serializable]
public class DayData
{
    public int year;
    public int month;
    public int week;
    public int day;
    public string weather;
}

//  전체 저장 데이터 (플레이어, 스탯, 엔딩, 날짜 포함)
[System.Serializable]
public class SaveData
{
    public PlayerData player;
    public Stats stats;
    public EndingData endings;
    public DayData date;
}

//  싱글톤 데이터 매니저 (데이터 저장 & 로드)
public class DataManager : MonoBehaviour
{
    public static DataManager instance; // 싱글톤 인스턴스

    public PlayerData nowPlayer = new PlayerData();
    public Stats nowStats = new Stats();
    public EndingData nowEnding = new EndingData();
    public DayData currentDate = new DayData();

    private string path;
    private string filePrefix = "save"; // 세이브 파일 이름 (예: save1.json, save2.json)

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        path = Application.persistentDataPath + "/";
        Debug.Log("세이브 경로: " + path);
    }

    //  세이브 함수 (슬롯 지정 가능)
    public void SaveData(int slot)
    {
        SaveData data = new SaveData
        {
            player = nowPlayer,
            stats = nowStats,
            endings = nowEnding,
            date = currentDate
        };

        //  저장한 날짜/시간 기록
        nowPlayer.lastSavedTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path + filePrefix + slot + ".json", json);

        Debug.Log($"슬롯 {slot} 저장 완료! 저장 시간: {nowPlayer.lastSavedTime}");
    }

    //  데이터 불러오기 (슬롯 지정 가능)
    public void LoadData(int slot)
    {
        string filePath = path + filePrefix + slot + ".json";

        if (File.Exists(filePath))
        {
            string data = File.ReadAllText(filePath);
            SaveData loadedData = JsonUtility.FromJson<SaveData>(data);

            nowPlayer = loadedData.player;
            nowStats = loadedData.stats;
            nowEnding = loadedData.endings;
            currentDate = loadedData.date;

            Debug.Log($"슬롯 {slot} 불러오기 완료! 마지막 저장 시간: {nowPlayer.lastSavedTime}");
        }
        else
        {
            Debug.LogWarning($"세이브 슬롯 {slot}이 존재하지 않습니다!");
        }
    }

    //  엔딩 체크 함수 (최고 엔딩 우선)
    public int CheckEnding()
    {
        if (nowStats.intelligence >= 85 && nowStats.charisma >= 85 &&
            nowStats.elegance >= 85 && nowStats.morality >= 85 &&
            nowStats.fame >= 85 && nowStats.stress <= 20)
        {
            nowEnding.completedEndings.Add("스타 엔딩");
            return 10;
        }
        else if (nowStats.magic >= 85 && nowStats.faith <= 20 && nowStats.morality <= 20)
        {
            nowEnding.completedEndings.Add("마왕 엔딩");
            return 9;
        }
        else if (nowStats.strength >= 80 && nowStats.morality <= 40 &&
                 nowStats.intelligence >= 70 && nowStats.magic >= 80)
        {
            nowEnding.completedEndings.Add("폭군 엔딩");
            return 8;
        }
        else if (nowStats.elegance >= 80 && nowStats.intelligence >= 75 &&
                 nowStats.morality >= 75 && nowStats.charisma >= 75)
        {
            nowEnding.completedEndings.Add("왕 엔딩");
            return 7;
        }
        else if (nowStats.strength >= 75 && nowStats.elegance >= 75 &&
                 nowStats.morality >= 75 && nowStats.charisma >= 75)
        {
            nowEnding.completedEndings.Add("기사 엔딩");
            return 6;
        }

        return 0; // 기본 엔딩
    }

    //  인벤토리 관리 (아이템 추가)
    public void AddItem(string item)
    {
        nowPlayer.inventory.Add(item);
        Debug.Log($"아이템 추가: {item}");
    }

    //  인벤토리 관리 (아이템 삭제)
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

    //  현재 인벤토리 출력
    public void ShowInventory()
    {
        Debug.Log("현재 인벤토리: " + string.Join(", ", nowPlayer.inventory));
    }

    //  버튼을 통해 저장 (슬롯 1, 2, 3 선택 가능)
    public void SaveGame(int slot)
    {
        SaveData(slot);
        Debug.Log($"[버튼 클릭] 슬롯 {slot}에 저장 완료!");
    }

    //  버튼을 통해 불러오기 (슬롯 1, 2, 3 선택 가능)
    public void LoadGame(int slot)
    {
        LoadData(slot);
        Debug.Log($"[버튼 클릭] 슬롯 {slot}에서 불러오기 완료!");
    }

    //  세이브 & 로드 테스트 (키 입력 시 저장 & 불러오기 실행)
    void Update()
    {

    }
}
