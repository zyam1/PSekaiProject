using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;


public class AnimationStatEvent : MonoBehaviour
{
    public GameObject[] animeImages;  // 10개의 UI Image (Animator 없이 처리)
    public GameObject AnimePanel;     // AnimePanel 자체
    public float animationDuration = 2.5f; // 각 애니메이션 재생 시간
    public GameObject RightUI;
    public GameObject resetButton;
    public GameObject skipButton;
    public GameObject statSumPanel;
    Dictionary<string, int> previousStats;//이전 딕셔너리

    //! 래거시 스텟관련 딕셔너리
    // Dictionary<int, Dictionary<string, int>> activityStatEffects = new()
    // {
    //     { 1,  new() { { "intelligence", 20 }, { "faith", -70 }, { "elegance", -70 } } }, // 금기서 성공
    //     { 2,  new() { { "money", 1 }, { "strength", 1 } } },//푸드코트 알바
    //     { 3,  new() { { "elegance", 2 }, { "charisma", 1 } } },// 발레 성공
    //     { 4,  new() { { "art", 1 }, { "fame", 1 } ,{ "stress", 1 }} },//피아노연주
    //     { 5,  new() { { "magic", 2 }, { "intelligence", 1 } ,{ "stress", 1 }} },//마법연구
    //     { 6,  new() { { "strength", 2 }, { "elegance", 1 },{ "stress", 1 } } },//검술성공
    //     { 7,  new() { { "intelligence", 1 }, { "elegance", 1 } } },//책읽기
    //     { 8,  new() { { "charisma", 1 }, { "fame", 1 } } },//공연
    //     { 9,  new() { { "faith", 2 }, { "morality", 1 } } },//기도
    //     { 10, new() { { "strength", 1 }, { "stress", -2 } } },//산책봄
    //     { 11, new() { { "strength", 1 }, { "stress", -2 } } },//산책
    //     { 12, new() { { "strength", 1 }, { "stress", -2 } } },//산책
    //     { 21, new() { { "faith", 1 }, { "morality", 2 },{ "stress", 1 } } }, // 봉사활동 성공

    //     // 실패한 버전 (스케줄에 실패 번호가 들어감)
    //     { 14, new() { { "elegance", 1  } } },             // 발레 실패
    //     { 15, new() { { "art", 1 }, { "stress", 3 } } },  // 피아노 연주 알바 실패
    //     { 16, new() { { "magic", 1 },{ "stress", 1 } } },                 // 마법 연구 실패
    //     { 17, new() { { "strength", 1 },{ "stress", 1 } } },             // 검술 실패
    //     { 18, new() { { "intelligence", 1 },{ "stress", -1 } } },          // 독서 실패
    //     { 19, new() { { "charisma", 1 }, { "fame", 1 }, { "stress", 1 } } }, // 공연 실패
    //     { 20, new() { { "faith", 1 } } }, // 기도 실패
    //     { 22, new() { { "morality", 1 },{ "stress", 1 } } }, // 봉사활동 실패 
    // };
    //! 스탯 관련 딕셔너리
    Dictionary<int, Dictionary<string, int>> activityStatEffects = new()
{
    // 특수
    { 1, new() { { "intelligence", 40 }, { "faith", -100 }, { "elegance", -100 } } }, // 금기서

    // 일거리
    { 2, new() { { "money", 10 }, { "strength", 5 }, { "stress", 10 } } }, // 푸드코트 알바

    // 예술/공연
    { 3, new() { { "elegance", 8 }, { "charisma", 5 }, { "stress", 2 } ,{ "money", -5 }} }, // 발레
    { 4, new() { { "art", 6 }, { "fame", 6 }, { "stress", 3 }, { "money", 5 } } }, // 피아노 연주
    { 8, new() { { "charisma", 6 }, { "fame", 6 }, { "stress", 3 }, { "money", 5 } } }, // 공연

    // 학문
    { 5, new() { { "magic", 8 }, { "intelligence", 5 }, { "stress", 3 },{ "money", -5 } } }, // 마법연구
    { 7, new() { { "intelligence", 6 }, { "elegance", 4 }, { "stress", 1 } } }, // 독서

    // 전투
    { 6, new() { { "strength", 10 }, { "elegance", 3 }, { "stress", 4 } } }, // 검술

    // 신앙/도덕
    { 9, new() { { "faith", 7 }, { "morality", 4 }, { "stress", -1 } } }, // 기도
    { 21, new() { { "faith", 5 }, { "morality", 8 }, { "stress", 2 } } }, // 봉사활동

    // 회복 활동
    { 10, new() { { "strength", 3 }, { "stress", -10 } } }, // 산책봄
    { 11, new() { { "strength", 3 }, { "stress", -10 } } }, // 산책
    { 12, new() { { "strength", 3 }, { "stress", -10 } } }, // 산책

    // 실패
    { 14, new() { { "elegance", 2 }, { "stress", 4 },{ "money", -5 } } },  // 발레 실패
    { 15, new() { { "art", 2 }, { "stress", 6 } } },       // 피아노 실패
    { 16, new() { { "magic", 3 }, { "stress", 4 },{ "money", -5 } } },     // 마법 실패
    { 17, new() { { "strength", 3 }, { "stress", 4 } } },  // 검술 실패
    { 18, new() { { "intelligence", 2 }, { "stress", 0 } } }, // 독서 실패
    { 19, new() { { "charisma", 2 }, { "fame", 2 }, { "stress", 4 } } }, // 공연 실패
    { 20, new() { { "faith", 3 }, { "stress", 1 } } },      // 기도 실패
    { 22, new() { { "morality", 3 }, { "stress", 3 } } },   // 봉사 실패
};

    //! 스텟 데이터 메모리에 올리는 함수
    void ApplySingleStatEffect(int act)
    {
        if (!activityStatEffects.ContainsKey(act)) return;

        foreach (var stat in activityStatEffects[act])
        {
            int value = stat.Value;

            switch (stat.Key)
            {
                case "intelligence":
                    DataManager.instance.nowStats.intelligence += value;
                    if (DataManager.instance.nowStats.intelligence < 0)
                        DataManager.instance.nowStats.intelligence = 0;
                    break;
                case "charisma":
                    DataManager.instance.nowStats.charisma += value;
                    if (DataManager.instance.nowStats.charisma < 0)
                        DataManager.instance.nowStats.charisma = 0;
                    break;
                case "elegance":
                    DataManager.instance.nowStats.elegance += value;
                    if (DataManager.instance.nowStats.elegance < 0)
                        DataManager.instance.nowStats.elegance = 0;
                    break;
                case "morality":
                    DataManager.instance.nowStats.morality += value;
                    if (DataManager.instance.nowStats.morality < 0)
                        DataManager.instance.nowStats.morality = 0;
                    break;
                case "fame":
                    DataManager.instance.nowStats.fame += value;
                    if (DataManager.instance.nowStats.fame < 0)
                        DataManager.instance.nowStats.fame = 0;
                    break;
                case "stress":
                    DataManager.instance.nowStats.stress += value;
                    if (DataManager.instance.nowStats.stress < 0)
                        DataManager.instance.nowStats.stress = 0;
                    break;
                case "strength":
                    DataManager.instance.nowStats.strength += value;
                    if (DataManager.instance.nowStats.strength < 0)
                        DataManager.instance.nowStats.strength = 0;
                    break;
                case "magic":
                    DataManager.instance.nowStats.magic += value;
                    if (DataManager.instance.nowStats.magic < 0)
                        DataManager.instance.nowStats.magic = 0;
                    break;
                case "faith":
                    DataManager.instance.nowStats.faith += value;
                    if (DataManager.instance.nowStats.faith < 0)
                        DataManager.instance.nowStats.faith = 0;
                    break;
                case "art":
                    DataManager.instance.nowStats.art += value;
                    if (DataManager.instance.nowStats.art < 0)
                        DataManager.instance.nowStats.art = 0;
                    break;
                case "money":
                    DataManager.instance.nowPlayer.money += value;
                    if (DataManager.instance.nowPlayer.money < 0)
                        DataManager.instance.nowPlayer.money = 0;
                    break;
            }
        }
    }





    public DialogueRunner dialogueRunner;
    void Start()
    {
        skipButton.SetActive(false);
    }
    public void PlayScheduleAnimation(List<int> schedule)
    {
        previousStats = new Dictionary<string, int>
        {
            { "intelligence", DataManager.instance.nowStats.intelligence },
            { "charisma", DataManager.instance.nowStats.charisma },
            { "elegance", DataManager.instance.nowStats.elegance },
            { "morality", DataManager.instance.nowStats.morality },
            { "fame", DataManager.instance.nowStats.fame },
            { "stress", DataManager.instance.nowStats.stress },
            { "strength", DataManager.instance.nowStats.strength },
            { "magic", DataManager.instance.nowStats.magic },
            { "faith", DataManager.instance.nowStats.faith },
            { "art", DataManager.instance.nowStats.art },
            { "month", DataManager.instance.currentDate.month},
            { "year", DataManager.instance.currentDate.year},
        };
        if (!AnimePanel.activeSelf)
        {
            AnimePanel.SetActive(true); // AnimePanel 활성화
        }

        StartCoroutine(PlayAnimationSequence(schedule));

    }
    public void ClickSkipButton()
    {
        animationDuration = 2.5f;
        SetAnimationSpeed(1f);
        resetButton.SetActive(true);
        skipButton.SetActive(false);
    }
    public void ClickResetButton()
    {
        animationDuration = 1f;
        SetAnimationSpeed(2f / animationDuration);
        resetButton.SetActive(false);
        skipButton.SetActive(true);
    }
    void SetAnimationSpeed(float speedMultiplier)
    {
        foreach (var image in animeImages)
        {
            Animator animator = image.GetComponent<Animator>();
            if (animator != null)
            {
                animator.speed = speedMultiplier;
            }
        }
    }
    IEnumerator PlayAnimationSequence(List<int> schedule)
    {
        AnimePanel.SetActive(true);

        // 모든 이미지를 비활성화 (초기화)
        foreach (var image in animeImages)
        {
            image.SetActive(false);
        }

        // 스케줄에 맞는 애니메이션 활성화
        for (int i = 0; i < schedule.Count; i++)
        {
            int scheduleNumber = schedule[i];

            if (scheduleNumber >= 1 && scheduleNumber <= animeImages.Length)
            {
                GameObject targetImage = animeImages[scheduleNumber - 1];
                int currentDay = (i % 7) + 1;
                dialogueRunner.VariableStorage.SetValue("$day", currentDay);
                targetImage.SetActive(true); if (dialogueRunner.IsDialogueRunning)
                {
                    dialogueRunner.Stop();
                }
                dialogueRunner.StartDialogue($"AnimeDialog{scheduleNumber}");

                Animator animator = targetImage.GetComponent<Animator>();
                if (animator != null)
                {
                    animator.Play(animator.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, 0);
                }
                ApplySingleStatEffect(scheduleNumber);

                yield return new WaitForSeconds(animationDuration);
                //여기에 날짜 변경 함수 
                if (DataManager.instance.currentDate.month == 12)
                {
                    //12월 일경우 년도 변경, 달 1월로 변경
                    DataManager.instance.currentDate.year++;
                    DataManager.instance.currentDate.month = 1;
                }
                else
                {
                    //나머지의 경우 그냥 다음달로
                    DataManager.instance.currentDate.month++;
                }


                targetImage.SetActive(false);
            }
        }
        //!여기가 모든애니메이션 실행완료 시점
        //패널이 비활성화 되기 전에 객체 정보 수정
        //!변경된 딕셔너리
        Dictionary<string, int> currentStats = new()
        {
            { "intelligence", DataManager.instance.nowStats.intelligence },
            { "charisma", DataManager.instance.nowStats.charisma },
            { "elegance", DataManager.instance.nowStats.elegance },
            { "morality", DataManager.instance.nowStats.morality },
            { "fame", DataManager.instance.nowStats.fame },
            { "stress", DataManager.instance.nowStats.stress },
            { "strength", DataManager.instance.nowStats.strength },
            { "magic", DataManager.instance.nowStats.magic },
            { "faith", DataManager.instance.nowStats.faith },
            { "art", DataManager.instance.nowStats.art },
            { "month", DataManager.instance.currentDate.month},
            { "year", DataManager.instance.currentDate.year},
        };
        DataManager.instance.SaveData(1);
        statSumPanel.GetComponent<StatSumPanel>().ShowStatChanges(previousStats, currentStats);
        AnimePanel.SetActive(false);  // 애니메이션 종료 후 패널 비활성화
        RightUI.SetActive(true);

        schedule.Clear();  // 스케줄 초기화
        Debug.Log("스케줄 초기화 완료");
    }


}
