using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

// JSON 구조 정의
[System.Serializable]
public class WordCondition
{
    public string type;          // "event", "stat", "item", "random", "multiple"
    public string eventName;     // 이벤트 이름
    public string statName;      // 스탯 이름
    [SerializeField] public string operator_field;   // 연산자 (">=", "<=", "==", ">", "<") - 'operator'는 예약어이므로 operator_field 사용
    public int value;            // 비교값
    public string itemName;      // 아이템 이름
    public float probability;    // 확률 (0.0 ~ 1.0)
    public string description;   // 조건 설명
    public WordCondition[] requirements; // 복합 조건용
}

[System.Serializable]
public class WordUnlockRule
{
    public string wordId;
    public WordCondition[] conditions;
}

[System.Serializable]
public class WordConfigData
{
    public WordUnlockRule[] wordUnlockConditions;
    public Word[] wordMasterData;
}

// 단어 획득 조건 관리 시스템
public class WordConditionManager : MonoBehaviour
{
    [Header("설정")]
    public bool enableDebugLog = true;
    
    private WordConfigData configData;
    private Dictionary<string, WordUnlockRule> conditionRules;

    private void Start()
    {
        LoadWordConfig();
    }

    // JSON 설정 파일 로드
    private void LoadWordConfig()
    {
        string configPath = Path.Combine(Application.streamingAssetsPath, "WordConfig.json");
        
        try
        {
            if (File.Exists(configPath))
            {
                string json = File.ReadAllText(configPath);
                configData = JsonUtility.FromJson<WordConfigData>(json);
                
                // 빠른 조회를 위한 Dictionary 생성
                conditionRules = new Dictionary<string, WordUnlockRule>();
                foreach (var rule in configData.wordUnlockConditions)
                {
                    conditionRules[rule.wordId] = rule;
                }

                if (enableDebugLog)
                {
                    Debug.Log($"단어 설정 로드 완료! {configData.wordUnlockConditions.Length}개 조건, {configData.wordMasterData.Length}개 단어");
                }
            }
            else
            {
                Debug.LogError($"WordConfig.json 파일을 찾을 수 없습니다: {configPath}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"WordConfig.json 로드 실패: {e.Message}");
        }
    }

    // 이벤트 기반 단어 해금 체크
    public void CheckWordUnlockByEvent(string eventName, object[] parameters = null)
    {
        if (configData == null) return;

        foreach (var rule in configData.wordUnlockConditions)
        {
            foreach (var condition in rule.conditions)
            {
                if (CheckCondition(condition, eventName, parameters))
                {
                    DataManager.instance.UnlockWord(rule.wordId, condition.description);
                }
            }
        }
    }

    // 특정 단어의 해금 조건 체크
    public bool CheckWordUnlockCondition(string wordId, string eventName = "", object[] parameters = null)
    {
        if (!conditionRules.ContainsKey(wordId)) return false;

        var rule = conditionRules[wordId];
        foreach (var condition in rule.conditions)
        {
            if (CheckCondition(condition, eventName, parameters))
            {
                return true;
            }
        }
        return false;
    }

    // 개별 조건 체크
    private bool CheckCondition(WordCondition condition, string eventName = "", object[] parameters = null)
    {
        switch (condition.type.ToLower())
        {
            case "event":
                return !string.IsNullOrEmpty(eventName) && eventName == condition.eventName;

            case "stat":
                return CheckStatCondition(condition);

            case "item":
                return CheckItemCondition(condition);

            case "random":
                return CheckRandomCondition(condition, eventName);

            case "multiple":
                return CheckMultipleConditions(condition, eventName, parameters);

            default:
                Debug.LogWarning($"알 수 없는 조건 타입: {condition.type}");
                return false;
        }
    }

    // 스탯 조건 체크
    private bool CheckStatCondition(WordCondition condition)
    {
        var stats = DataManager.instance.nowStats;
        int currentValue = 0;

        // 스탯 이름으로 현재 값 가져오기
        switch (condition.statName.ToLower())
        {
            case "intelligence": currentValue = stats.intelligence; break;
            case "charisma": currentValue = stats.charisma; break;
            case "elegance": currentValue = stats.elegance; break;
            case "morality": currentValue = stats.morality; break;
            case "fame": currentValue = stats.fame; break;
            case "stress": currentValue = stats.stress; break;
            case "strength": currentValue = stats.strength; break;
            case "magic": currentValue = stats.magic; break;
            case "faith": currentValue = stats.faith; break;
            case "art": currentValue = stats.art; break;
            default:
                Debug.LogWarning($"알 수 없는 스탯: {condition.statName}");
                return false;
        }

        // 연산자에 따른 비교
        string operatorStr = condition.operator_field ?? ">="; // 기본값 설정
        
        switch (operatorStr)
        {
            case ">=": return currentValue >= condition.value;
            case "<=": return currentValue <= condition.value;
            case "==": return currentValue == condition.value;
            case ">":	return currentValue > condition.value;
            case "<":	return currentValue < condition.value;
            default:
                Debug.LogWarning($"알 수 없는 연산자: '{operatorStr}' (기본값 '>=' 사용)");
                return currentValue >= condition.value; // 기본 비교로 폴백
        }
    }

    // 아이템 조건 체크
    private bool CheckItemCondition(WordCondition condition)
    {
        return DataManager.instance.nowPlayer.inventory.Contains(condition.itemName);
    }

    // 확률 조건 체크
    private bool CheckRandomCondition(WordCondition condition, string eventName)
    {
        // 이벤트가 일치하고 확률 조건을 만족하는 경우
        if (!string.IsNullOrEmpty(condition.eventName) && eventName != condition.eventName)
            return false;

        return UnityEngine.Random.value <= condition.probability;
    }

    // 복합 조건 체크 (모든 조건을 만족해야 함)
    private bool CheckMultipleConditions(WordCondition condition, string eventName, object[] parameters)
    {
        if (condition.requirements == null) return false;

        foreach (var req in condition.requirements)
        {
            if (!CheckCondition(req, eventName, parameters))
                return false;
        }
        return true;
    }

    // 현재 해금 가능한 단어들 체크
    public List<string> GetUnlockableWords()
    {
        var unlockableWords = new List<string>();
        
        foreach (var rule in configData.wordUnlockConditions)
        {
            // 이미 해금된 단어는 제외
            if (DataManager.instance.globalWords.unlockedWordIds.Contains(rule.wordId))
                continue;

            // 조건 체크 (이벤트 없이도 해금 가능한 것들)
            foreach (var condition in rule.conditions)
            {
                if (condition.type == "stat" || condition.type == "item")
                {
                    if (CheckCondition(condition))
                    {
                        unlockableWords.Add(rule.wordId);
                        break;
                    }
                }
            }
        }

        return unlockableWords;
    }

    // 설정에서 단어 데이터 가져오기
    public Word[] GetWordMasterData()
    {
        return configData?.wordMasterData ?? new Word[0];
    }

    // 특정 단어의 해금 조건 설명 가져오기
    public List<string> GetWordUnlockDescriptions(string wordId)
    {
        var descriptions = new List<string>();
        
        if (conditionRules.ContainsKey(wordId))
        {
            var rule = conditionRules[wordId];
            foreach (var condition in rule.conditions)
            {
                if (!string.IsNullOrEmpty(condition.description))
                {
                    descriptions.Add(condition.description);
                }
            }
        }

        return descriptions;
    }

    // 디버그: 모든 조건 출력
    [ContextMenu("단어 조건 출력")]
    public void PrintAllWordConditions()
    {
        if (configData == null)
        {
            Debug.Log("설정 데이터가 로드되지 않았습니다.");
            return;
        }

        Debug.Log("=== 단어 해금 조건 목록 ===");
        foreach (var rule in configData.wordUnlockConditions)
        {
            var word = configData.wordMasterData.FirstOrDefault(w => w.id == rule.wordId);
            string wordText = word?.text ?? "알 수 없음";
            
            Debug.Log($"단어: {wordText} (ID: {rule.wordId})");
            foreach (var condition in rule.conditions)
            {
                Debug.Log($"  - {condition.description} (타입: {condition.type})");
            }
        }
    }
}
