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


    public DialogueRunner dialogueRunner;
    void Start()
    {
        skipButton.SetActive(false);
    }
    public void PlayScheduleAnimation(List<int> schedule)
    {
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

                yield return new WaitForSeconds(animationDuration);

                targetImage.SetActive(false);
            }
        }

        AnimePanel.SetActive(false);  // 애니메이션 종료 후 패널 비활성화
        RightUI.SetActive(true);

        schedule.Clear();  // 스케줄 초기화
        Debug.Log("스케줄 초기화 완료");
    }


}
