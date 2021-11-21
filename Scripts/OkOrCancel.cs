using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//선택창 관리 스크립트
public class OkOrCancel : MonoBehaviour
{
    private AudioManager theAudio;
    public string key_sound;
    public string enter_sound;
    public string cancel_sound;

    public GameObject up_Panel; //선택되면 비활성화됨
    public GameObject down_Panel; //"

    public Text up_Text; //소비 사용 장착 등의 텍스트 사용위한 변수
    public Text down_Text; //마찬가지

    public bool activated; //창 활성화 여부
    private bool keyInput; //필요할 때만 키입력 이루어지도록
    private bool result = true; //플레이어의 선택, 어떤게 선택되었는지 나타냄, 처음에 창을 키면 무조건 '사용'칸을 가리키니 true

    // Start is called before the first frame update
    void Start()
    {
        theAudio = FindObjectOfType<AudioManager>();
    }


    public void Selected() //어떤 항목이 선택되었는지 보여주는 함수
    {
        theAudio.Play(key_sound);
        result = !result;

        if (result) //선택
        {
            up_Panel.gameObject.SetActive(false);
            down_Panel.gameObject.SetActive(true);
        }
        else //취소
        {
            up_Panel.gameObject.SetActive(true);
            down_Panel.gameObject.SetActive(false);
        }
    }

    public void ShowTwoChoice(string _upText, string _downText) //선택창 화면 띄우는 함수
    {
        activated = true;
        result = true;
        up_Text.text = _upText;
        down_Text.text = _downText;

        //위가 선택된것처럼 보여줌
        up_Panel.gameObject.SetActive(false);
        down_Panel.gameObject.SetActive(true);

        StartCoroutine(ShowTwoChoiceCoroutine());
    }

    public bool GetResult() //플레이어가 선택한 결과값 반환
    {
        return result;
    }

    IEnumerator ShowTwoChoiceCoroutine() //중복키 처리 방지
    {
        yield return new WaitForSeconds(0.01f);
        keyInput = true;
    }
    // Update is called once per frame
    void Update()
    {
        if(keyInput) //키입력처리
        {
            if(Input.GetKeyDown(KeyCode.DownArrow))
            {
                Selected();
            }
            else if(Input.GetKeyDown(KeyCode.UpArrow))
            {
                Selected();
            }
            else if (Input.GetKeyDown(KeyCode.Z))
            {
                theAudio.Play(enter_sound);
                keyInput = false; 
                activated = false; 
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                theAudio.Play(cancel_sound);
                keyInput = false;
                activated = false;
                result = false; 
            }
        }
    }
}
