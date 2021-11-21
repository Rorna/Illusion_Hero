using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//슬라임 움직임, 공격 관련 스크립트
public class SlimeController : MovingObject
{

    public float attackDelay; //공격 유예, 공격 애니메이션에서 대기를 만든 시간에 맞게 설정

    public float inter_MoveWaitTime; //이동 하기 전에 대기 시간, 3이면 3초 대기후 한번 움직임
    private float current_interMWT; //그 대기시간 실질적 계산하는 변수

    public string atkSound; //공격사운드

    private Vector2 PlayerPos; //플레이어의 좌표값, 이 값으로 공격

    private int random_int; //랜덤 값으로 움직임
    private string directrion; //Move함수 파라미터 값 UP, DOWN LEFT, RIGHT, 함수를통해 이 값에 데이터를 넣을거임
    
    public GameObject healthBar; //체력바 반전용

    // Start is called before the first frame update
    void Start()
    {
        queue = new Queue<string>(); 
        current_interMWT = inter_MoveWaitTime; 
    }

    // Update is called once per frame
    void Update()
    {
        current_interMWT -= Time.deltaTime; 

        if(current_interMWT<=0)
        {
            current_interMWT = inter_MoveWaitTime; //다시 반복될수있도록 다시 넣음 초기화

            if (NearPlayer()) //근처 플레이어 있으면
            {
                Flip(); //공격
                return;
            }
            RandomDirection(); //direction 삽입

            if (base.CheckCollision()) //true==앞에 뭔가있을경우
            {
                return; //이동안함
            }
            base.Move(directrion);
        }
    }

    private void Flip() //공격 실행 함수, 플레이어 위치에 따라 왼쪽 오른쪽 공격(애니메이션 반전시킴)
    {
        Vector3 flip = transform.localScale; 

        if (PlayerPos.x > this.transform.position.x) 
            flip.x = -1f;
        else 
            flip.x = 1f;
        this.transform.localScale = flip;

        healthBar.transform.localScale = flip; 

        animator.SetTrigger("Attack"); 
        StartCoroutine(WaitCoroutine()); 
    }

    IEnumerator WaitCoroutine() //공격중 대기 코루틴, 회피판정
    {
        yield return new WaitForSeconds(attackDelay); 
        AudioManager.instance.Play(atkSound);
        //마지막으로 근처에 플레이어가 있는지 한번 더 검사
        if(NearPlayer())
        {
            PlayerStat.instance.Hit(GetComponent<EnemyStat>().atk);
        }
    }

    private bool NearPlayer()
    {
        PlayerPos = PlayerManager.instance.transform.position;

        if (Mathf.Abs(Mathf.Abs(PlayerPos.x) - Mathf.Abs(this.transform.position.x))<=speed*walkCount*1.01f) 
        {
            if (Mathf.Abs(Mathf.Abs(PlayerPos.y) - Mathf.Abs(this.transform.position.y)) <= speed * walkCount*0.5f) 
            {
                return true;
            }
        }

        if (Mathf.Abs(Mathf.Abs(PlayerPos.y) - Mathf.Abs(this.transform.position.y)) <= speed * walkCount * 1.01f) 
        {
            if (Mathf.Abs(Mathf.Abs(PlayerPos.x) - Mathf.Abs(this.transform.position.x)) <= speed * walkCount * 0.5f) 
            {
                return true;
            }
        }

        //플레이어가 근처에 없으면
        return false;
    }

    private void RandomDirection() //랜덤값에 따라 direction 데이터 삽입
    {
        vector.Set(0, 0, vector.z);
        random_int = Random.Range(0, 4);
        switch(random_int)
        {
            case 0:
                vector.y = 1f;
                directrion = "UP";
                break;
            case 1:
                vector.y = -1f;
                directrion = "DOWN";
                break;
            case 2:
                vector.x = 1f;
                directrion = "RIGHT";
                break;
            case 3:
                vector.x = -1f;
                directrion = "LEFT";
                break;
        }
    }

}
