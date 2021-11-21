using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 방향키 한번 누르면 특정 픽셀씩 움직이게 구현
/// 왼쪽 시프트키 누르면 달리기, 속도 빨라짐
/// </summary>
public class PlayerManager : MovingObject
{
    static public PlayerManager instance; 

    public string currentMapName; //transferMap 스크립트에 있는 transferMapName 변수의 값을 저장, 맵이동 관련변수
    public string currentSceneName; //세이브 로드시 필요, 로드시 어느 위치에 있는지 알아야하므로 추가, 씬의 위치 기록

    //사운드
    public string walkSound_1;
    public string walkSound_2;
    public string walkSound_3;
    public string walkSound_4;
    private AudioManager theAudio;

    //private SaveNLoad theSaveNLoad; //세이브로드

    public float runSpeed;
    private float applyRunspeed;
    private bool applyRunFlag = false;
    private bool canMove = true;
    public bool transferMap = true;

    public bool notMove = false;

    private bool attacking = false; //공격 여부
    public float attackDelay; //공격 딜레이
    private float currentAttackDelay; //현재 공격 딜레이, 연속공격하면 안되게 제한

    private void Awake()
    {
        if(instance ==null)
        {
            DontDestroyOnLoad(this.gameObject); 
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
            Debug.Log("재생성");
        }
    }

    void Start()
    {
        queue = new Queue<string>();
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        theAudio = FindObjectOfType<AudioManager>();
    }


    IEnumerator MoveCoroutine() //움직임 담당 couroutine
    {
        while (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0 && !notMove && !attacking) //공격중엔 못움직이게
        {
            if (Input.GetKey(KeyCode.LeftShift)) //push left shift button, 빨리 달리기
            {
                applyRunspeed = runSpeed;
                applyRunFlag = true;
            }
            else
            {
                applyRunspeed = 0; //다시 떼면 복원
                applyRunFlag = false;
            }

            vector.Set(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), transform.position.z); 
            if (vector.x != 0) vector.y = 0; //상좌 하우 같은 동시키입력 방지
            //animation
            animator.SetFloat("DirX", vector.x);
            animator.SetFloat("DirY", vector.y);

            //충돌 방지
            bool checkCollisionFlag = base.CheckCollision();
            if (checkCollisionFlag) break;

            animator.SetBool("Walking", true);

            //사운드 실행
            int temp = Random.Range(1, 4); //사운드 네개 추가했으면 1,4
            switch (temp)
            {
                case 1:
                    theAudio.Play(walkSound_1);
                    break;
                case 2:
                    theAudio.Play(walkSound_2);
                    break;
                case 3:
                    theAudio.Play(walkSound_3);
                    break;
                case 4:
                    theAudio.Play(walkSound_4);
                    break;
            }

            //npc와 플레이어간 박스콜라이더로 인해 서로 끼이거나 튕기는 현상방지
            boxCollider.offset = new Vector2(vector.x * 0.7f * speed * walkCount, vector.y * 0.7f * speed * walkCount);

            //이동
            while (currentWalkCount < walkCount)
            {
                transform.Translate(vector.x * (speed + applyRunspeed), vector.y * (speed + applyRunspeed), 0);
                if (applyRunFlag) currentWalkCount++;
                currentWalkCount++;
                if (currentWalkCount == 12) boxCollider.offset = Vector2.zero; 
                yield return new WaitForSeconds(0.01f);
            }

            currentWalkCount = 0;
        }
        animator.SetBool("Walking", false);
        canMove = true; 

    }
    // Update is called once per frame
    void Update()
    {
        if (canMove && !notMove && !attacking) 
        {

            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            {
                canMove = false;
                StartCoroutine(MoveCoroutine());

            }
        }
        if(!notMove && !attacking) //움직일 수 있고 공격할 수 있으면
        {
            if(Input.GetKeyDown(KeyCode.Space)) //스페이스바로 공격
            {
                currentAttackDelay = attackDelay;
                attacking = true; 
                animator.SetBool("Attacking", true); 

            }
        }

        if(attacking) //공격 활성화 되면
        {
            currentAttackDelay -= Time.deltaTime; 
            if(currentAttackDelay<=0) 
            {
                animator.SetBool("Attacking", false); 
                attacking = false;
            }
        }

    }
}
