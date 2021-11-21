using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//명령 관련 스크립트
public class OrderManager : MonoBehaviour
{
    static public OrderManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(this.gameObject); //씬이 바뀌어도 이 오브젝트는 파괴 ㄴㄴ
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
            Debug.Log("재생성");
        }
    }

    private PlayerManager thePlayer; //이벤트 도중에 키입력 처리 방지, 이벤트 도중에 플레이어의 키 입력을 제한
    private List<MovingObject> characters; //배열은 도중에 변경 불가하므로 리스트로 선언

    void Start()
    {
        thePlayer = FindObjectOfType<PlayerManager>();

    }
    public void PreLoadCharacter() //캐릭터 리스트 채우는 함수
    {
        characters = ToList();
    }
    public List<MovingObject> ToList() //movingobject 들어간 모든 오브젝트 찾아서 list에 넣고 그 list 반환
    {
        List<MovingObject> tempList = new List<MovingObject>();
        MovingObject[] temp = FindObjectsOfType<MovingObject>(); 

        for(int i=0; i<temp.Length; i++)
        {
            tempList.Add(temp[i]);
        }
        return tempList;
    }

    public void NotMove() //움직임 비활성화
    {
        thePlayer.notMove = true;
        Debug.Log("notmove 활성화");
    }

    public void Move() //움직임 활성화
    {
        thePlayer.notMove = false;
        Debug.Log("move 활성화");
    }
    public void SetThorought(string _name) //벽뚫
    {
        for (int i = 0; i < characters.Count; i++) 
        {
            if (_name == characters[i].characterName) 
            {
                characters[i].boxCollider.enabled = false;
            }
        }
    }
    public void SetUnThorought(string _name) //벽안뚫
    {
        for (int i = 0; i < characters.Count; i++) 
        {
            if (_name == characters[i].characterName) 
            {
                characters[i].boxCollider.enabled = true;
            }
        }
    }
    public void SetTransparent(string _name) //사라지게하는 함수
    {
        for (int i = 0; i < characters.Count; i++) 
        {
            if (_name == characters[i].characterName) 
            {
                characters[i].gameObject.SetActive(false);
            }
        }
    }
    public void SetUnTransparent(string _name) //안사라지게하는 함수
    {
        for (int i = 0; i < characters.Count; i++) 
        {
            if (_name == characters[i].characterName) 
            {
                characters[i].gameObject.SetActive(true);
            }
        }
    }
    public void Move(string _name, string _dir) //어떤 캐릭터가 어떤 방향으로 움직일것인가
    {
        for(int i=0; i<characters.Count; i++) 
        {
            if(_name==characters[i].characterName) 
            {
                characters[i].Move(_dir);
            }
        }
    }

    public void Turn(string _name, string _dir) //방향만 바꾸는 함수
    {
        for (int i = 0; i < characters.Count; i++) 
        {
            if (_name == characters[i].characterName) 
            {
                characters[i].animator.SetFloat("DirX", 0f);
                characters[i].animator.SetFloat("DirY", 0f);

                switch (_dir)
                {
                    case "UP":
                        characters[i].animator.SetFloat("DirY", 1f);
                        break;
                    case "DOWN":
                        characters[i].animator.SetFloat("DirY", -1f);
                        break;
                    case "LEFT":
                        characters[i].animator.SetFloat("DirX", -1f);
                        break;
                    case "RIGHT":
                        characters[i].animator.SetFloat("DirX", 1f);
                        break;
                }
            }
        }
    }
}
