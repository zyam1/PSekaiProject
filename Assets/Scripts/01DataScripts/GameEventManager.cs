using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// 게임 이벤트 타입 정의
[System.Serializable]
public enum GameEventType
{
    ConversationCompleted,      // 대화 완료
    ConversationStarted,        // 대화 시작
    ItemObtained,              // 아이템 획득
    StatChanged,               // 스탯 변경
    WordUnlocked,              // 단어 해금
    WordUsed,                  // 단어 사용
    CharacterMet,              // 캐릭터 만남
    QuestCompleted,            // 퀘스트 완료
    LocationVisited,           // 장소 방문
    TimeChanged,               // 시간 변경
    MoneyChanged,              // 돈 변경
    InventoryChanged,          // 인벤토리 변경
    Custom                     // 사용자 정의 이벤트
}

// 게임 이벤트 데이터
[System.Serializable]
public class GameEventData
{
    public GameEventType eventType;
    public string eventName;                    // 커스텀 이벤트명
    public Dictionary<string, object> parameters = new Dictionary<string, object>();
    public DateTime timestamp;

    public GameEventData(GameEventType type, string name = "", Dictionary<string, object> param = null)
    {
        eventType = type;
        eventName = string.IsNullOrEmpty(name) ? type.ToString() : name;
        parameters = param ?? new Dictionary<string, object>();
        timestamp = DateTime.Now;
    }

    // 파라미터 추가 헬퍼 메소드
    public GameEventData AddParameter(string key, object value)
    {
        parameters[key] = value;
        return this;
    }

    // 파라미터 가져오기
    public T GetParameter<T>(string key, T defaultValue = default(T))
    {
        if (parameters.ContainsKey(key) && parameters[key] is T)
        {
            return (T)parameters[key];
        }
        return defaultValue;
    }
}

// 게임 이벤트 매니저 (싱글톤)
public class GameEventManager : MonoBehaviour
{
    public static GameEventManager instance;

    [Header("디버그 설정")]
    public bool enableEventLog = true;
    public bool enableDetailedLog = false;

    // 이벤트 구독자들
    private Dictionary<GameEventType, List<Action<GameEventData>>> eventListeners = 
        new Dictionary<GameEventType, List<Action<GameEventData>>>();

    // 이벤트 히스토리 (디버그용)
    private List<GameEventData> eventHistory = new List<GameEventData>();
    
    // Unity Events (인스펙터에서 설정 가능)
    [Header("Unity Events")]
    public UnityEvent<GameEventData> OnAnyEvent;
    public UnityEvent<GameEventData> OnConversationEvent;
    public UnityEvent<GameEventData> OnWordEvent;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            
            // GameObject 이름 설정
            if (gameObject.name != "[GameEventManager]")
            {
                gameObject.name = "[GameEventManager]";
            }
            
            DontDestroyOnLoad(gameObject);
            InitializeEventSystem();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // 싱글톤 인스턴스 생성 (어디서든 호출 가능)
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void CreateSingleton()
    {
        if (instance == null)
        {
            GameObject eventManagerObj = new GameObject("[GameEventManager]");
            instance = eventManagerObj.AddComponent<GameEventManager>();
        }
    }

    // 이벤트 시스템 초기화
    private void InitializeEventSystem()
    {
        // 기본 이벤트 리스너들 등록
        RegisterGameSystemListeners();
        
        if (enableEventLog)
        {
            Debug.Log("GameEventManager 초기화 완료!");
        }
    }

    // 게임 시스템 리스너들 등록
    private void RegisterGameSystemListeners()
    {
        // 대화 관련 이벤트
        Subscribe(GameEventType.ConversationCompleted, OnConversationCompletedHandler);
        
        // 단어 관련 이벤트  
        Subscribe(GameEventType.WordUnlocked, OnWordUnlockedHandler);
        Subscribe(GameEventType.WordUsed, OnWordUsedHandler);
        
        // 스탯 관련 이벤트
        Subscribe(GameEventType.StatChanged, OnStatChangedHandler);
        
        // 아이템 관련 이벤트
        Subscribe(GameEventType.ItemObtained, OnItemObtainedHandler);
    }

    // =====  이벤트 구독/발행 시스템 =====

    /// <summary>
    /// 이벤트 구독
    /// </summary>
    public void Subscribe(GameEventType eventType, Action<GameEventData> listener)
    {
        if (!eventListeners.ContainsKey(eventType))
        {
            eventListeners[eventType] = new List<Action<GameEventData>>();
        }
        eventListeners[eventType].Add(listener);
    }

    /// <summary>
    /// 이벤트 구독 해제
    /// </summary>
    public void Unsubscribe(GameEventType eventType, Action<GameEventData> listener)
    {
        if (eventListeners.ContainsKey(eventType))
        {
            eventListeners[eventType].Remove(listener);
        }
    }

    /// <summary>
    /// 이벤트 발행
    /// </summary>
    public void Publish(GameEventData eventData)
    {
        // 히스토리에 추가
        eventHistory.Add(eventData);
        
        // 최근 100개만 유지
        if (eventHistory.Count > 100)
        {
            eventHistory.RemoveAt(0);
        }

        // 로그 출력
        if (enableEventLog)
        {
            string logMessage = $"[이벤트] {eventData.eventType}: {eventData.eventName}";
            if (enableDetailedLog && eventData.parameters.Count > 0)
            {
                logMessage += $" | 파라미터: {string.Join(", ", eventData.parameters)}";
            }
            Debug.Log(logMessage);
        }

        // Unity Events 호출
        OnAnyEvent?.Invoke(eventData);
        
        if (eventData.eventType == GameEventType.ConversationCompleted || 
            eventData.eventType == GameEventType.ConversationStarted)
        {
            OnConversationEvent?.Invoke(eventData);
        }
        
        if (eventData.eventType == GameEventType.WordUnlocked || 
            eventData.eventType == GameEventType.WordUsed)
        {
            OnWordEvent?.Invoke(eventData);
        }

        // 구독자들에게 이벤트 전달
        if (eventListeners.ContainsKey(eventData.eventType))
        {
            foreach (var listener in eventListeners[eventData.eventType])
            {
                try
                {
                    listener?.Invoke(eventData);
                }
                catch (Exception e)
                {
                    Debug.LogError($"이벤트 핸들러 실행 중 오류: {e.Message}");
                }
            }
        }
    }

    // ===== 편의 메소드들 =====

    /// <summary>
    /// 간단한 이벤트 발행
    /// </summary>
    public static void TriggerEvent(GameEventType eventType, string eventName = "", Dictionary<string, object> parameters = null)
    {
        if (instance != null)
        {
            var eventData = new GameEventData(eventType, eventName, parameters);
            instance.Publish(eventData);
        }
    }

    /// <summary>
    /// 대화 완료 이벤트 발행
    /// </summary>
    public static void TriggerConversationCompleted(string characterId = "general", string dialogId = "")
    {
        var parameters = new Dictionary<string, object>
        {
            ["characterId"] = characterId,
            ["dialogId"] = dialogId
        };
        TriggerEvent(GameEventType.ConversationCompleted, "conversation_completed", parameters);
    }

    // ===== 이벤트 핸들러들 =====

    private void OnConversationCompletedHandler(GameEventData eventData)
    {
        string characterId = eventData.GetParameter<string>("characterId", "general");
        
        // DataManager에 대화 횟수 증가 요청
        if (DataManager.instance != null)
        {
            DataManager.instance.IncrementConversationCount(characterId);
        }
    }

    private void OnWordUnlockedHandler(GameEventData eventData)
    {
        string wordId = eventData.GetParameter<string>("wordId", "");
        if (enableEventLog)
        {
            Debug.Log($"단어 해금 이벤트 처리: {wordId}");
        }
    }

    private void OnWordUsedHandler(GameEventData eventData)
    {
        string wordId = eventData.GetParameter<string>("wordId", "");
        string targetId = eventData.GetParameter<string>("targetId", "");
        int affectionGain = eventData.GetParameter<int>("affectionGain", 0);
        
        if (enableEventLog)
        {
            Debug.Log($"단어 사용 이벤트 처리: {wordId} -> {targetId} (+{affectionGain})");
        }
    }

    private void OnStatChangedHandler(GameEventData eventData)
    {
        string statName = eventData.GetParameter<string>("statName", "");
        int newValue = eventData.GetParameter<int>("newValue", 0);
        int oldValue = eventData.GetParameter<int>("oldValue", 0);
        
        // 스탯 변경에 따른 단어 해금 체크
        if (DataManager.instance != null)
        {
            DataManager.instance.CheckWordUnlock($"stat_{statName}_changed", newValue, oldValue);
        }
    }

    private void OnItemObtainedHandler(GameEventData eventData)
    {
        string itemName = eventData.GetParameter<string>("itemName", "");
        
        // 아이템 획득에 따른 단어 해금 체크
        if (DataManager.instance != null)
        {
            DataManager.instance.CheckWordUnlock("item_obtained", itemName);
        }
    }

    // ===== 디버그 메소드들 =====

    [ContextMenu("이벤트 히스토리 출력")]
    public void PrintEventHistory()
    {
        Debug.Log("=== 최근 이벤트 히스토리 ===");
        for (int i = eventHistory.Count - 1; i >= Mathf.Max(0, eventHistory.Count - 10); i--)
        {
            var evt = eventHistory[i];
            Debug.Log($"{evt.timestamp:HH:mm:ss} - {evt.eventType}: {evt.eventName}");
        }
    }

    [ContextMenu("이벤트 통계")]
    public void PrintEventStatistics()
    {
        var stats = new Dictionary<GameEventType, int>();
        foreach (var evt in eventHistory)
        {
            if (!stats.ContainsKey(evt.eventType))
                stats[evt.eventType] = 0;
            stats[evt.eventType]++;
        }

        Debug.Log("=== 이벤트 발생 통계 ===");
        foreach (var kvp in stats)
        {
            Debug.Log($"{kvp.Key}: {kvp.Value}회");
        }
    }
}
