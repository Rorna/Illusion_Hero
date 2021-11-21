using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//이벤트 발생시 행동 스크립트
//콜라이더에 닿은 상태서 캐릭터가 위로 향하고 z키를 누른상태에서 발동

public class Event1 : MonoBehaviour
{
    //대화 부분, 설정하고 싶은 개수만큼 선언
    public Dialogue dialogue_1;
    public Dialogue dialogue_2;

    //필요에 따라 아래처럼 모듈 붙여놓으면 됨
    private DialogueManager theDM;
    private OrderManager theOrder; //npc 이동제어용
    private PlayerManager thePlayer; //캐릭터의 방향이 앞을 바라볼때 작동 되도록 제어(DirY값 이용)
    private FadeManager theFade; //반짝임 효과 사용 위한 변수

    private bool flag; //한번만 실행되게 제어용 변수

    void Start()
    {
        theDM = FindObjectOfType<DialogueManager>();
        theOrder = FindObjectOfType<OrderManager>();
        thePlayer = FindObjectOfType<PlayerManager>();
        theFade = FindObjectOfType<FadeManager>();
    }
    //getkey, getkeydown 차이점: getkey는 누르고있어도 적용됨
    private void OnTriggerStay2D(Collider2D collision) //박스콜라이더 안에 있으면 계속 실행되는 함수
    {
        if(!flag&&Input.GetKey(KeyCode.Z) &&thePlayer.animator.GetFloat("DirY")==1) //캐릭터가 위로향하고 z키를 누른상태에서 발동
        {
            flag = true;
            StartCoroutine(EventCoroutine());
        }
    }
    IEnumerator EventCoroutine()
    {
        theOrder.PreLoadCharacter(); //리스트 채워줌
        theOrder.NotMove(); //이벤트 동안에는 움직이면 안됨

        theDM.ShowDialogue(dialogue_1);
        //대화가 끝날때까지 기다렸다가 대화가 끝나면 이동시킴
        yield return new WaitUntil(() => !theDM.talking);

        //중간에 이동시킴
        //오른쪽으로 두칸이동 위쪽으로 한칸 이동시킴
        theOrder.Move("player", "RIGHT");
        theOrder.Move("player", "RIGHT");
        theOrder.Move("player", "UP");

        //위에서 큐가 세개 생김
        yield return new WaitUntil(()=>thePlayer.queue.Count==0); //이동하고나서 잠시 대기후 다시 대화 시작

        //대화가 시작될 때 번개침
        theFade.Flash(); //반짝반짝
        theDM.ShowDialogue(dialogue_2);
        yield return new WaitUntil(() => !theDM.talking);

        theOrder.Move();
    }
}
