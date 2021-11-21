using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//공통 변수를 담아놓는 클래스

public class MovingObject : MonoBehaviour
{
    public string characterName; //캐릭터 리스트에 쓰일 변수

    //공통 변수
    public float speed;
    public int walkCount; //=20
    protected int currentWalkCount;

    private bool notCoroutine = false; //코루틴 중복실행 방지
    protected Vector3 vector; //퍼블릭이지만 인스펙터에 뜨지 않도록 조정

    
    public Queue<string> queue; //FIFO, npc 이동후 애니메이션 계속 실행 되는 것 방지
    public BoxCollider2D boxCollider;
    public LayerMask layerMask; //충돌시 어떤 레이어와의 충돌을 했는지 판단 변수
    public Animator animator; //애니메이션 제어

    public void Move(string _dir, int _frequency=5) //부자연스러운 애니메이션 방지를 위해 freq 변수 추가(기본값 5), 이동 함수
    {
        //함수 실행될 때마다 큐에 쌓이게됨
        queue.Enqueue(_dir);
        if(!notCoroutine) //false일 경우에만 실행(처음 무조건 실행)
        {
            notCoroutine = true; //이 명령문에 의해 중복실행 ㄴㄴ
            StartCoroutine(MoveCoroutine(_dir, _frequency));
        }
        
    }

    IEnumerator MoveCoroutine(string _dir, int _frequency)
    {
        while(queue.Count !=0)
        {
            switch (_frequency)
            {
                case 1:
                    yield return new WaitForSeconds(4f);
                    break;
                case 2:
                    yield return new WaitForSeconds(3f);
                    break;
                case 3:
                    yield return new WaitForSeconds(2f);
                    break;
                case 4:
                    yield return new WaitForSeconds(1f);
                    break;
                case 5:
                    break;
            }

            string direction = queue.Dequeue();
            vector.Set(0, 0, vector.z);
            switch (direction)
            {
                case "UP":
                    vector.y = 1f;
                    break;
                case "DOWN":
                    vector.y = -1f;
                    break;
                case "RIGHT":
                    vector.x = 1f;
                    break;
                case "LEFT":
                    vector.x = -1f;
                    break;
            }

            //animation
            animator.SetFloat("DirX", vector.x);
            animator.SetFloat("DirY", vector.y);

            //충돌 방지
            while(true)
            {
                bool checkCollisionFlag = CheckCollision(); //충돌체크
                if (checkCollisionFlag)
                {
                    animator.SetBool("Walking", false);
                    yield return new WaitForSeconds(1f);
                }
                else //플레이어가 비킬경우
                {
                    break; 
                }

            }

            animator.SetBool("Walking", true);

            //npc와 플레이어간 박스콜라이더로 인해 서로 끼는 현상방지
            //이동시 미리 박스콜라이더의 위치를 이동시킴
            boxCollider.offset = new Vector2(vector.x * 0.7f * speed * walkCount, vector.y * 0.7f * speed * walkCount);

            //이동
            while (currentWalkCount < walkCount)
            {
              
                transform.Translate(vector.x * speed, vector.y * speed, 0); 
                currentWalkCount++;
                if (currentWalkCount == walkCount*0.5f+2) 
                    boxCollider.offset = Vector2.zero; 

                yield return new WaitForSeconds(0.01f); //wait for 1 second   
            }
            currentWalkCount = 0;

            if (_frequency != 5) 
            {
                animator.SetBool("Walking", false);
            }
        }
        animator.SetBool("Walking", false);
        notCoroutine = false;
    }

    protected bool CheckCollision()
    {

        RaycastHit2D hit;
        Vector2 start = new Vector2(transform.position.x + vector.x * speed * walkCount,
                                    transform.position.y + vector.y * speed * walkCount); 

        Vector2 end = start + new Vector2(vector.x * speed, vector.y * speed); 
        boxCollider.enabled = false; 

        hit = Physics2D.Linecast(start, end, layerMask); 
        boxCollider.enabled = true;

        if (hit.transform != null) return true; 
        return false;
    }
}
