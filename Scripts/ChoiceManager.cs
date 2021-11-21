using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceManager : MonoBehaviour
{
    public static ChoiceManager instance;

    private void Awake()
    {
        if (instance != null) Destroy(this.gameObject);
        else //instance==null
        {
            DontDestroyOnLoad(this.gameObject); //씬이 바뀌어도 이 오브젝트는 파괴 ㄴㄴ
            instance = this;
        }
    }

    private AudioManager theAudio; //사운드 재생

    private string question; //커스텀 클래스의 question을 여기에 대입
    private List<string> answerList;// 커스텀 클래스의 answers를 여기에 대입

    public GameObject go; //평소에 비활성화 시킬 목적으로 선언(선택창이 항상 떠있으면 곤란하므로), 평소엔 비활성화상태, 필요하면 활성화 시킴(SetActive)

    public Text question_Text;
    public Text[] answer_Text;
    public GameObject[] answer_Panel; //선택 패널 관리용 변수, 선택된 패널만 투명도 조절하에 짙게 설정할거임

    public Animator anim;

    public string keySound;
    public string enterSound;

    public bool choiceIng; //대기, 선택지가 활성화 되면 대기 ()=>!choicelng (waituntil)
    private bool keyInput; //선택키처리 활성화, 비 활성화용 변수

    private int count; //배열의 크기
    private int result; //선택한 선택창, 첫번재 선택했으면 0이 들어감

    private WaitForSeconds waitTime = new WaitForSeconds(0.01f);

    // Start is called before the first frame update
    void Start()
    {
        theAudio = FindObjectOfType<AudioManager>();
        answerList = new List<string>();
        //text 초기화
        for (int i = 0; i < answer_Text.Length; i++)
        {
            answer_Text[i].text = "";
            answer_Panel[i].SetActive(false); //비활성화
        }
        question_Text.text = "";

    }
    public void ShowChoice(Choice _choice) //선택창 활성화
    {
        choiceIng = true;
        go.SetActive(true);
        result = 0;
        question = _choice.question;
        //배열의 크기만큼 answerlist에 들어감
        for (int i = 0; i < _choice.answers.Length; i++)
        {
            answerList.Add(_choice.answers[i]);
            answer_Panel[i].SetActive(true); //배열의 크기만큼 패널도 활성화, 배열의 크기 3이면 선택지 세개만 나옴 그이상 안나옴
            count = i;
        }
        anim.SetBool("Appear", true);
        Selection(); //반복될 때 이상한 창 선택되는 것 방지, 0번째가 선택된것처럼 보이게하기 위해 호출
        StartCoroutine(ChoiceCoroutine()); //animation 실행중에 잠시 대기
    }
    public int GetResult() //분기에 쓰일 result값 추출
    {
        return result;
        //혹은 여기에 go.setActive(false)를 줘도 됨
    }

    public void ExitChoice() //모든 것 초기화
    {
        question_Text.text = "";
        //텍스트 초기화
        for (int i = 0; i <= count; i++)
        {
            answer_Text[i].text = "";
            answer_Panel[i].SetActive(false);
        }
        //리스트 초기화
        answerList.Clear();
        //애니메이션 끔
        anim.SetBool("Appear", false);
        choiceIng = false;
        StartCoroutine(AnimationCoroutine());
    }
    IEnumerator AnimationCoroutine() //사라지는 애니메이션 보존을 위해 대기시간을줌
    {
        yield return new WaitForSeconds(1f); //1초 대기
        go.SetActive(false); //이렇게 하면 사라지는 애니메이션 볼 수 있음
    }
    IEnumerator ChoiceCoroutine() //선택 대기
    {
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(TypingQuestion()); //무조건 실행
        StartCoroutine(TypingAnswer_0()); //이것도 무조건 실행, 최소 한가지의 선택은 존재하므로
        //count=배열 크기
        if (count >= 1)
            StartCoroutine(TypingAnswer_1());
        if (count >= 2)
            StartCoroutine(TypingAnswer_2());
        if (count >= 3)
            StartCoroutine(TypingAnswer_3());

        yield return new WaitForSeconds(0.5f);
        keyInput = true; //키 입력 가능하게 입력 처리 활성화
    }

    IEnumerator TypingQuestion()
    {
        //글자가 한글자씩 나오게 설정
        for (int i = 0; i < question.Length; i++)
        {
            question_Text.text += question[i]; //'가나다라'였으면 가, 나, 다, 라 순서대로 출력
            yield return waitTime;
        }
    }

    IEnumerator TypingAnswer_0() //선택지 1
    {
        //파도타기 처럼 실행되게 만듬
        yield return new WaitForSeconds(0.4f);

        //글자가 한글자씩 나오게 설정
        for (int i = 0; i < answerList[0].Length; i++)
        {
            answer_Text[0].text += answerList[0][i]; //'가나다라'였으면 가, 나, 다, 라 순서대로 출력
            yield return waitTime;
        }
    }
    IEnumerator TypingAnswer_1() //선택지 2
    {
        //파도타기 처럼 실행되게 만듬
        yield return new WaitForSeconds(0.5f);
        //글자가 한글자씩 나오게 설정
        for (int i = 0; i < answerList[1].Length; i++)
        {
            answer_Text[1].text += answerList[1][i]; //'가나다라'였으면 가, 나, 다, 라 순서대로 출력
            yield return waitTime;
        }
    }
    IEnumerator TypingAnswer_2() //선택지 3
    {
        //파도타기 처럼 실행되게 만듬
        yield return new WaitForSeconds(0.6f);
        //글자가 한글자씩 나오게 설정
        for (int i = 0; i < answerList[2].Length; i++)
        {
            answer_Text[2].text += answerList[2][i]; //'가나다라'였으면 가, 나, 다, 라 순서대로 출력
            yield return waitTime;
        }
    }
    IEnumerator TypingAnswer_3() //선택지 4
    {
        //파도타기 처럼 실행되게 만듬
        yield return new WaitForSeconds(0.7f);
        //글자가 한글자씩 나오게 설정
        for (int i = 0; i < answerList[3].Length; i++)
        {
            answer_Text[3].text += answerList[3][i]; //'가나다라'였으면 가, 나, 다, 라 순서대로 출력
            yield return waitTime;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (keyInput)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                theAudio.Play(keySound);
                //count 3일때 방향키 누르면 result->3 2 1 0 3
                if (result > 0) result--; //선택창을 위로 선택하면 result는 줄어듬, result는 현재 선택되어있는 선택창을 의미
                else result = count;
                Selection(); //선택 효과 함수 호출
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                theAudio.Play(keySound);
                //count 3일때 방향키 누르면 result->0 1 2 3 0
                if (result < count) result++;
                else result = 0; //맨 아래상태에서 한번 더누르면 맨 위로 돌아감    
                Selection();
            }
            else if (Input.GetKeyDown(KeyCode.Z)) //z를 눌러 선택
            {
                theAudio.Play(enterSound);
                keyInput = false; //선택했으므로 더이상 키 입력 불필요
                ExitChoice(); //모든 걸 끄는 함수
            }
        }

    }

    //어떤게 선택되었는지 알게 해주는 함수
    public void Selection() //선택되었다는 연출 출력 함수
    {
        //투명도로 선택된 창 표현
        //함수 호출되면 모든 선택창이 투명해짐(0.75)
        Color color = answer_Panel[0].GetComponent<Image>().color; //Image 컴포넌트 가져옴
        color.a = 0.75f;
        for (int i = 0; i <= count; i++) //모든 패널의 투명도를 0.75로 조정
        {
            answer_Panel[i].GetComponent<Image>().color = color; //집어넣음
        }
        color.a = 1f; //선택된 것만 투명하지 않게 만듬
        answer_Panel[result].GetComponent<Image>().color = color; //집어넣음
    }
}
