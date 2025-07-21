using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class ConversationEvent : MonoBehaviour
{
    public GameObject YarnUI;
    public DialogueRunner dialogueRunner;
    public GameObject RightUnderUI;
    public GameObject LeftUI;
    
    private string selectedDialog; // 현재 선택된 대화 ID
    // Start is called before the first frame update
    void Start()
    {
        YarnUI.SetActive(false);
    }
    // 초기 대화 설정 
    private List<string> randomDialogNodes = new List<string>
    {
        "OneConversationDialog1",
        "OneConversationDialog2",
        "OneConversationDialog3",
        "OneConversationDialog4"
    };
    public void OnLeftUIClick()
    {
        Debug.Log("LeftUI 클릭 → 대화 실행");

        if (dialogueRunner.IsDialogueRunning)
        {
            Debug.Log("이미 대화가 진행 중입니다.");
            return; // 중복 실행 방지
        }

        YarnUI.SetActive(true);

        // 랜덤 대사 선택
        selectedDialog = randomDialogNodes[UnityEngine.Random.Range(0, randomDialogNodes.Count)];

        // 실행 
        RightUnderUI.SetActive(false);
        dialogueRunner.StartDialogue(selectedDialog);
        StartCoroutine(AutoEndDialogue());
    }

    IEnumerator AutoEndDialogue()
    {
        yield return new WaitForSeconds(2f); // 3초 대기
        if (dialogueRunner.IsDialogueRunning) // 대화가 진행 중일 경우에만 종료
        {
            OnDialogueComplete();
            Debug.Log("3초 후 대화 강제 종료");
        }
    }
    public void OnDialogueComplete()
    {
        Debug.Log("대화 종료 → YarnUI 비활성화");
        YarnUI.SetActive(false);
        RightUnderUI.SetActive(true);

        // 대화 강제 종료 (혹시 남아있을 경우)
        if (dialogueRunner.IsDialogueRunning)
        {
            dialogueRunner.Stop();
        }
        
        // 대화 완료 이벤트 발생 (이제 모든 대화 관련 로직은 GameEventManager가 처리)
        GameEventManager.TriggerConversationCompleted("general", selectedDialog ?? "unknown");
        
        Debug.Log("대화 완료 이벤트 발생!");
    }
    // Update is called once per frame
    void Update()
    {
        if (YarnUI.activeSelf) // YarnUI가 활성화 상태일 경우
        {
            if (Input.anyKeyDown) // 아무 키 입력 감지
            {
                OnDialogueComplete(); // 대화 종료
            }




        }
    }
}
