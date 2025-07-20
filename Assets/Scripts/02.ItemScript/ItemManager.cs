using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//----------------------------싱글톤 (전역 사용 함수 제작)----------------------------------------------------------
//아이템 관련 함수 모음 
public class ItemManager : MonoBehaviour
{


    public static ItemManager instance;//아이템 매니저 관련 인스턴스 (싱글톤)ItemManager.instance.DoSomething(); 으로 외부 접근

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
