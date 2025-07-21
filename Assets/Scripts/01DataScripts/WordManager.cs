using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WordManager : MonoBehaviour
{
    [Header("단어 시스템 설정")]
    public bool enableDebugLog = true;
    
    // 마스터 데이터
    private WordMasterData masterData;
    
    // 조건 매니저 참조
    private WordConditionManager conditionManager;
    
    // 캐릭터별 선호 카테고리 (확장 가능)
    private Dictionary<string, List<WordCategory>> characterPreferences;
    
    private void Start()
    {
        InitializeWordSystem();
        SetupCharacterPreferences();
    }

    // 단어 시스템 초기화
    private void InitializeWordSystem()
    {
        // WordConditionManager 참조 가져오기
        conditionManager = GetComponent<WordConditionManager>();
        if (conditionManager == null)
        {
            conditionManager = gameObject.AddComponent<WordConditionManager>();
        }
        
        // JSON에서 마스터 데이터 로드
        StartCoroutine(LoadMasterDataFromJSON());
    }
    
    // JSON에서 마스터 데이터 로드 (코루틴으로 비동기 처리)
    private System.Collections.IEnumerator LoadMasterDataFromJSON()
    {
        // WordConditionManager가 로드될 때까지 대기
        while (conditionManager.GetWordMasterData().Length == 0)
        {
            yield return new UnityEngine.WaitForSeconds(0.1f);
        }
        
        // 마스터 데이터 생성
        masterData = new WordMasterData();
        masterData.allWords.Clear();
        masterData.allWords.AddRange(conditionManager.GetWordMasterData());
        
        if (enableDebugLog)
        {
            Debug.Log($"단어 시스템 초기화 완료! 총 {masterData.allWords.Count}개의 단어가 JSON에서 로드되었습니다.");
        }
    }

    // 캐릭터별 선호 카테고리 설정 (나중에 JSON으로 외부화 가능)
    private void SetupCharacterPreferences()
    {
        characterPreferences = new Dictionary<string, List<WordCategory>>();
        
        // 예시 캐릭터들 (실제 캐릭터 ID에 맞게 수정 필요)
        characterPreferences["char_romantic"] = new List<WordCategory> { WordCategory.Romantic, WordCategory.Gentle };
        characterPreferences["char_intellectual"] = new List<WordCategory> { WordCategory.Intellectual, WordCategory.Mysterious };
        characterPreferences["char_energetic"] = new List<WordCategory> { WordCategory.Energetic, WordCategory.Humorous };
        characterPreferences["char_caring"] = new List<WordCategory> { WordCategory.Caring, WordCategory.Gentle };
    }

    // 단어 획득
    public bool UnlockWord(string wordId, string reason = "")
    {
        if (DataManager.instance.globalWords.unlockedWordIds.Contains(wordId))
        {
            if (enableDebugLog)
                Debug.Log($"이미 획득한 단어입니다: {GetWordById(wordId)?.text}");
            return false;
        }

        var word = GetWordById(wordId);
        if (word == null)
        {
            Debug.LogWarning($"존재하지 않는 단어 ID: {wordId}");
            return false;
        }

        // 전역 데이터에 추가
        DataManager.instance.globalWords.unlockedWordIds.Add(wordId);
        DataManager.instance.globalWords.totalWordsCollected++;
        DataManager.instance.globalWords.lastWordCollected = wordId;

        // 현재 플레이어 데이터에도 추가
        if (!DataManager.instance.nowPlayer.wordData.availableWords.Contains(wordId))
        {
            DataManager.instance.nowPlayer.wordData.availableWords.Add(wordId);
        }

        if (enableDebugLog)
        {
            Debug.Log($"새로운 단어 획득! '{word.text}' ({reason})");
        }

        return true;
    }

    // 단어 사용
    public int UseWord(string wordId, string characterId)
    {
        // 단어 소유 확인
        if (!DataManager.instance.nowPlayer.wordData.availableWords.Contains(wordId))
        {
            Debug.LogWarning($"소유하지 않은 단어를 사용하려 했습니다: {wordId}");
            return 0;
        }

        var word = GetWordById(wordId);
        if (word == null)
        {
            Debug.LogWarning($"존재하지 않는 단어: {wordId}");
            return 0;
        }

        // 효과 계산
        int affectionGain = CalculateWordEffect(word, characterId);
        bool wasEffective = affectionGain > 0;

        // 사용 기록 추가
        var usage = new WordUsage(wordId, characterId, wasEffective, affectionGain);
        DataManager.instance.nowPlayer.wordData.usageHistory.Add(usage);

        // 캐릭터별 호감도 업데이트
        if (!DataManager.instance.nowPlayer.wordData.characterAffection.ContainsKey(characterId))
        {
            DataManager.instance.nowPlayer.wordData.characterAffection[characterId] = 0;
        }
        DataManager.instance.nowPlayer.wordData.characterAffection[characterId] += affectionGain;

        // 캐릭터별 단어 사용 횟수 업데이트
        if (!DataManager.instance.nowPlayer.wordData.characterWordCount.ContainsKey(characterId))
        {
            DataManager.instance.nowPlayer.wordData.characterWordCount[characterId] = 0;
        }
        DataManager.instance.nowPlayer.wordData.characterWordCount[characterId]++;

        // 전역 사용 통계 업데이트
        if (!DataManager.instance.globalWords.totalUsageCount.ContainsKey(wordId))
        {
            DataManager.instance.globalWords.totalUsageCount[wordId] = 0;
        }
        DataManager.instance.globalWords.totalUsageCount[wordId]++;

        if (enableDebugLog)
        {
            Debug.Log($"'{word.text}' 단어를 {characterId}에게 사용! 호감도 {(affectionGain > 0 ? "+" : "")}{affectionGain}");
        }

        return affectionGain;
    }

    // 단어 효과 계산
    private int CalculateWordEffect(Word word, string characterId)
    {
        int baseEffect = word.baseAffectionValue;
        float multiplier = 1.0f;

        // 캐릭터 선호도 확인
        if (characterPreferences.ContainsKey(characterId))
        {
            var preferences = characterPreferences[characterId];
            if (preferences.Contains(word.category))
            {
                multiplier += 0.5f; // 선호 카테고리면 1.5배
            }
            else
            {
                multiplier -= 0.2f; // 비선호면 0.8배
            }
        }

        // 사용 횟수에 따른 감소 효과
        string characterKey = characterId;
        if (DataManager.instance.nowPlayer.wordData.characterWordCount.ContainsKey(characterKey))
        {
            int usageCount = DataManager.instance.nowPlayer.wordData.characterWordCount[characterKey];
            if (usageCount >= 5)
            {
                multiplier -= 0.1f * (usageCount - 4); // 5회 이후부터 효과 감소
            }
        }

        // 희귀도 보너스
        switch (word.rarity)
        {
            case 2: // 레어
                multiplier += 0.3f;
                break;
            case 3: // 에픽
                multiplier += 0.6f;
                break;
        }

        // 최소 효과 보장
        multiplier = Mathf.Max(0.1f, multiplier);

        return Mathf.RoundToInt(baseEffect * multiplier);
    }

    // ID로 단어 찾기
    public Word GetWordById(string wordId)
    {
        return masterData.allWords.FirstOrDefault(w => w.id == wordId);
    }

    // 소유한 단어 목록 반환
    public List<Word> GetOwnedWords()
    {
        var ownedWords = new List<Word>();
        foreach (string wordId in DataManager.instance.nowPlayer.wordData.availableWords)
        {
            var word = GetWordById(wordId);
            if (word != null)
            {
                ownedWords.Add(word);
            }
        }
        return ownedWords.OrderBy(w => w.rarity).ThenBy(w => w.category).ToList();
    }

    // 캐릭터별 호감도 조회
    public int GetCharacterAffection(string characterId)
    {
        if (DataManager.instance.nowPlayer.wordData.characterAffection.ContainsKey(characterId))
        {
            return DataManager.instance.nowPlayer.wordData.characterAffection[characterId];
        }
        return 0;
    }

    // 통계 정보
    public void ShowWordStatistics()
    {
        if (!enableDebugLog) return;

        var playerWords = DataManager.instance.nowPlayer.wordData;
        var globalWords = DataManager.instance.globalWords;

        Debug.Log("=== 단어 시스템 통계 ===");
        Debug.Log($"전체 수집 단어: {globalWords.totalWordsCollected}개");
        Debug.Log($"현재 보유 단어: {playerWords.availableWords.Count}개");
        Debug.Log($"총 사용 횟수: {playerWords.usageHistory.Count}회");

        if (playerWords.characterAffection.Count > 0)
        {
            Debug.Log("캐릭터별 호감도:");
            foreach (var kvp in playerWords.characterAffection)
            {
                Debug.Log($"  {kvp.Key}: {kvp.Value}");
            }
        }
    }

    // 조건부 단어 획득 체크 (이벤트에서 호출)
    public void CheckWordUnlockConditions(string eventType, object[] parameters = null)
    {
        if (conditionManager != null)
        {
            // JSON 기반 조건 시스템 사용
            conditionManager.CheckWordUnlockByEvent(eventType, parameters);
            
            // 상시 해금 가능한 단어들도 체크
            var unlockableWords = conditionManager.GetUnlockableWords();
            foreach (string wordId in unlockableWords)
            {
                var descriptions = conditionManager.GetWordUnlockDescriptions(wordId);
                string reason = descriptions.Count > 0 ? descriptions[0] : "조건 만족";
                UnlockWord(wordId, reason);
            }
        }
        else
        {
            Debug.LogWarning("WordConditionManager가 설정되지 않았습니다!");
        }
    }

    // 디버그 함수들
    [ContextMenu("모든 단어 해금")]
    public void UnlockAllWordsDebug()
    {
        foreach (var word in masterData.allWords)
        {
            UnlockWord(word.id, "디버그");
        }
        Debug.Log("모든 단어가 해금되었습니다!");
    }

    [ContextMenu("단어 통계 출력")]
    public void ShowStatisticsDebug()
    {
        ShowWordStatistics();
    }
}
