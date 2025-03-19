using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimationStatEvent : MonoBehaviour
{
    public GameObject[] animeImages;  // 10개의 UI Image (Animator 없이 처리)
    public GameObject AnimePanel;     // AnimePanel 자체
    public float animationDuration = 3f; // 각 애니메이션 재생 시간
    public GameObject RightUI;



    public void PlayScheduleAnimation(List<int> schedule)
    {
        if (!AnimePanel.activeSelf)
        {
            AnimePanel.SetActive(true); // AnimePanel 활성화
        }

        StartCoroutine(PlayAnimationSequence(schedule));

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
        foreach (int scheduleNumber in schedule)
        {
            if (scheduleNumber >= 1 && scheduleNumber <= animeImages.Length)
            {
                GameObject targetImage = animeImages[scheduleNumber - 1];
                targetImage.SetActive(true);

                // Animator 실행 (애니메이션 재생)
                Animator animator = targetImage.GetComponent<Animator>();
                if (animator != null)
                {
                    animator.Play(animator.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, 0);
                }

                yield return new WaitForSeconds(animationDuration); // 2초 후 다음 애니메이션 실행

                targetImage.SetActive(false); // 다음 애니메이션을 위해 비활성화
            }
        }

        AnimePanel.SetActive(false);  // 애니메이션 종료 후 패널 비활성화
        RightUI.SetActive(true);
        schedule.Clear();

    }

}
