using System;
using System.Collections.Generic;
using UnityEngine;

// 단어 타입 (확장 가능)
[System.Serializable]
public enum WordType
{
    Emotion,    // 감정: 사랑, 행복, 슬픔
    Object,     // 사물: 야채, 꽃, 음식
    State,      // 상태: 감기, 피곤, 건강
    Action,     // 행동: 산책, 요리, 공부
    Nature,     // 자연: 바다, 하늘, 별
    Abstract    // 추상: 꿈, 희망, 미래
}

// 단어 카테고리 (캐릭터 선호도와 연관)
[System.Serializable]
public enum WordCategory
{
    Romantic,   // 로맨틱
    Intellectual, // 지적
    Humorous,   // 유머러스
    Caring,     // caring/배려
    Energetic,  // 활동적
    Mysterious, // 신비로운
    Gentle      // 온화한
}

// 개별 단어 데이터
[System.Serializable]
public class Word
{
    public string id;                   // 단어 고유 ID (예: "word_love")
    public string text;                 // 실제 단어 텍스트 (예: "사랑")
    public WordType type;               // 단어 타입
    public WordCategory category;       // 단어 카테고리
    public int baseAffectionValue;      // 기본 호감도 수치
    public string description;          // 단어 설명
    public int rarity;                  // 희귀도 (1:일반, 2:레어, 3:에픽)
    public string unlockCondition;      // 획득 조건 설명

    public Word()
    {
        // 기본 생성자
    }

    public Word(string id, string text, WordType type, WordCategory category, int affection, string desc = "", int rarity = 1)
    {
        this.id = id;
        this.text = text;
        this.type = type;
        this.category = category;
        this.baseAffectionValue = affection;
        this.description = desc;
        this.rarity = rarity;
        this.unlockCondition = "";
    }
}

// 단어 사용 기록
[System.Serializable]
public class WordUsage
{
    public string wordId;           // 사용한 단어 ID
    public string characterId;      // 사용 대상 캐릭터 ID
    public string usedTime;         // 사용 시간 (DateTime을 string으로 저장)
    public bool wasEffective;       // 효과가 있었는지 여부
    public int affectionGained;     // 획득한 호감도

    public WordUsage()
    {
        // 기본 생성자
    }

    public WordUsage(string wordId, string characterId, bool effective, int affection)
    {
        this.wordId = wordId;
        this.characterId = characterId;
        this.usedTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        this.wasEffective = effective;
        this.affectionGained = affection;
    }
}

// 전역 단어 데이터 (모든 세이브 슬롯에서 공유)
[System.Serializable]
public class GlobalWordsData
{
    public List<string> unlockedWordIds = new List<string>();           // 획득한 단어 ID 목록
    public Dictionary<string, int> totalUsageCount = new Dictionary<string, int>(); // 전체 사용 통계
    public int totalWordsCollected;                                     // 총 수집한 단어 수
    public string lastWordCollected;                                    // 마지막으로 수집한 단어 ID

    public GlobalWordsData()
    {
        unlockedWordIds = new List<string>();
        totalUsageCount = new Dictionary<string, int>();
        totalWordsCollected = 0;
        lastWordCollected = "";
    }
}

// 플레이어별 단어 데이터 (세이브 슬롯별로 저장)
[System.Serializable]
public class PlayerWordsData
{
    public List<WordUsage> usageHistory = new List<WordUsage>();            // 현재 플레이에서의 사용 기록
    public Dictionary<string, int> characterAffection = new Dictionary<string, int>(); // 캐릭터별 호감도
    public Dictionary<string, int> characterWordCount = new Dictionary<string, int>(); // 캐릭터별 단어 사용 횟수
    public List<string> availableWords = new List<string>();               // 현재 플레이에서 사용 가능한 단어들

    public PlayerWordsData()
    {
        usageHistory = new List<WordUsage>();
        characterAffection = new Dictionary<string, int>();
        characterWordCount = new Dictionary<string, int>();
        availableWords = new List<string>();
    }
}

// 단어 마스터 데이터 (게임에 존재하는 모든 단어 정보)
[System.Serializable]
public class WordMasterData
{
    public List<Word> allWords = new List<Word>();

    public WordMasterData()
    {
        allWords = new List<Word>();
        // JSON에서 단어 데이터를 로드하므로 하드코딩 제거
    }
}
