using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberSystem : MonoBehaviour
{
    private AudioManager theAudio;
    public string key_sound; //방향키 사운드
    public string enter_sound; //결정키 사운드
    public string cancel_sound; //오답 && 취소키 사운드
    public string correct_sound; //정답 사운드
    public int moveX; //superObject의 x값을 얼마만큼 이동시킬지, 30정도가 적당, 50은 우측으로 쏠림

    private int count; //배열의 크기 몇 자릿수, 숫자 1000이 들어오면 -> 자릿수는 3
    private int selectedTextBox; //선택된 자릿수. 어디가 선택됐는지 확인
    private int result; //플레이어가 선택한 값.
    private int correctNumber; //정답

    private string tempNumber;

    public GameObject superObject; //SuperObject 이동, 화면 가운데 정렬을 위한 녀석
    public GameObject[] panel; //패널들 활성화 및 비활성화 관여
    public Text[] Number_Text;

    public Animator anim;

    public bool activated; //대기를 위한 변수 return new waitUntil, 패스워드 작업이 끝날때까지 대기, 입력이 끝나면 false
    private bool keyInput; //키처리 활성화, 비활성화
    private bool correctFlag; //정답인지 아닌지 여부, result와 correctNumber가 일치하면 true

    // Use this for initialization
    void Start()
    {
        theAudio = FindObjectOfType<AudioManager>();
    }

    public void ShowNumber(int _correctNumber) //넘버 시작함수
    {
        correctNumber = _correctNumber;
        activated = true; //대기 시작
        correctFlag = false; //처음부터 정답일리 없으니 false로 초기화

        string temp = correctNumber.ToString(); 

        //활성화
        for (int i = 0; i < temp.Length; i++)
        {
            count = i;
            panel[i].SetActive(true); 
            Number_Text[i].text = "0"; 
        }

        superObject.transform.position = new Vector3(superObject.transform.position.x + (moveX * count),
                                                     superObject.transform.position.y,
                                                     superObject.transform.position.z);

        selectedTextBox = 0; 
        result = 0; 
        SetColor(); 
        anim.SetBool("Appear", true); 
        keyInput = true; 
    }


    public bool GetResult() //결과 반환
    {
        return correctFlag; //true면 다음상황 진행, false면 재시도
    }

    public void SetNumber(string _arrow) //방향키 눌렀을때 숫자 바꾸는 함수
    {
        
        int temp = int.Parse(Number_Text[selectedTextBox].text); 

        if (_arrow == "DOWN")
        {
            if (temp == 0) temp = 9;
            else temp--;
        }
        else if (_arrow == "UP")
        {
            if (temp == 9) temp = 0;
            else temp++;
        }
        
        Number_Text[selectedTextBox].text = temp.ToString(); //int->string
    }

    public void SetColor() //선택된 것 활성화 하는함수, 알파값으로 조정
    {
        Color color = Number_Text[0].color; 
        color.a = 0.3f;
        //모든 텍스트 반투명하게
        for (int i = 0; i <= count; i++)
        {
            Number_Text[i].color = color;
        }
        //선택된 것만 짙게
        color.a = 1f;
        Number_Text[selectedTextBox].color = color;
    }

    // Update is called once per frame
    void Update()
    {
        //키입력
        if (keyInput)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow)) 
            {
                theAudio.Play(key_sound);
                SetNumber("DOWN");
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow)) 
            {
                theAudio.Play(key_sound);
                SetNumber("UP");
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow)) 
            {
                theAudio.Play(key_sound);
                if (selectedTextBox < count) 
                    selectedTextBox++; 
                else 
                    selectedTextBox = 0; 
                SetColor();
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow)) 
            {
                theAudio.Play(key_sound);
                if (selectedTextBox > 0) 
                    selectedTextBox--;
                else
                    selectedTextBox = count; 
                SetColor();
            }
            else if (Input.GetKeyDown(KeyCode.Z)) //결정키(Z)
            {
                theAudio.Play(key_sound);
                keyInput = false; 
                StartCoroutine(OXCoroutine());

            }
            else if (Input.GetKeyDown(KeyCode.X)) //취소키(X)
            {
                theAudio.Play(key_sound);
                keyInput = false; 
                StartCoroutine(ExitCoroutine());
            }

        }
    }

    IEnumerator OXCoroutine() //플레이어의 값과 정답 판별하는 코루틴
    {
        //반투명 되었던 것들 전부 되돌림(보이게)
        Color color = Number_Text[0].color;
        color.a = 1f;

        //각각의 숫자들을 삽입
        for (int i = count; i >= 0; i--) //끝번호부터의 삽입을 위해 반대로 루프 돌림
        {
            Number_Text[i].color = color;
            tempNumber += Number_Text[i].text; //끝번호부터 넣음
        }

        //z키를 누르는 순간 모든 숫자들의 색이 변하고 1초 대기(연출을 위해)
        yield return new WaitForSeconds(1f); //색이 변하고 나서 비교 실행

        result = int.Parse(tempNumber); //강제 형변환 string->int

        //비교
        if (result == correctNumber) //정답
        {
            theAudio.Play(correct_sound);
            correctFlag = true;
        }
        else //오답
        {
            theAudio.Play(cancel_sound);
            correctFlag = false;
        }
        Debug.Log("우리가 낸 답 = " + result + "  정답 = " + correctNumber);
        StartCoroutine(ExitCoroutine()); //판별 끝났으면 종료

    }
    IEnumerator ExitCoroutine() //끝내는 코루틴
    {
        //모든 요소 초기화
        result = 0;
        tempNumber = "";
        anim.SetBool("Appear", false);

        yield return new WaitForSeconds(0.1f); //애니메이션 끝날 때까지 대기

        //활성화 된 것들 차례차례 비활성화
        for (int i = 0; i <= count; i++)
        {
            panel[i].SetActive(false);
        }

        //아까 가운데 정렬시켰던 것 다시 원복(-)
        superObject.transform.position = new Vector3(superObject.transform.position.x - (moveX * count),
                                                     superObject.transform.position.y,
                                                     superObject.transform.position.z);

        activated = false;
    }
}
