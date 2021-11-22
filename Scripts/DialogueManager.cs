using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public GameObject go; //dialogue manager

    public Text text;
    public SpriteRenderer rendererSprite; 
    public SpriteRenderer rendererDialogueWindow;

    //Dialogue 클래스내의 배열들을 여기에 넣을거임
    //대화마다 문장의 크기나 길이가 달라지기 때문에 list로 선언
    private List<string> listSentences;
    private List<Sprite> listSprites;
    private List<Sprite> listDialogueWindows;

    private int count; //대화 진행 상황 카운트, 위치

    public Animator animSprite; //sprite animation 통제
    public Animator animDialogueWindow; //dialoguewindow animation 통제

    //글자 출력 사운드
    public string typeSound;
    public string enterSound;

    private AudioManager theAudio;
    //private OrderManager theOrder;

    public bool talking=false; //대화가 이루어지지 않을때, z키 입력 방지
    private bool keyActivated = false; //키 연타시 오류 방지
    private bool OnlyText = false;

    // Start is called before the first frame update
    void Start()
    {
        count = 0;
        text.text = "";
        listSentences = new List<string>();
        listSprites = new List<Sprite>();
        listDialogueWindows = new List<Sprite>();
        theAudio = FindObjectOfType<AudioManager>();
    }
    

    public void ShowText(string[] _sentences) //텍스트만 출력하고 싶을 때
    {
        talking = true;
        OnlyText = true;

        for (int i = 0; i < _sentences.Length; i++)
        {
            listSentences.Add(_sentences[i]);
        }
        StartCoroutine(StartTextCoroutine()); //이게 시작한다는 것은 대화가 시작한다는 의미
    }

    public void ShowDialogue(Dialogue dialogue) //인수로 받은 dialogue를 위의 리스트에 삽입
    {
        talking = true;
        OnlyText = false;

        //삽입
        for (int i=0; i<dialogue.sentences.Length; i++)
        {
            listSentences.Add(dialogue.sentences[i]);
            listSprites.Add(dialogue.sprites[i]);
            listDialogueWindows.Add(dialogue.dialogueWindows[i]);
        }
        //추출(대화 시작)
        //카메라 밖에 있던 이미지들 활성화 시킴
        animSprite.SetBool("Appear", true);
        animDialogueWindow.SetBool("Appear", true);
        StartCoroutine(StartDialogueCoroutine()); //이게 시작한다는 것은 대화가 시작한다는 의미
    }
    public void ExitDialogue() //대화창 삭제 함수
    {
        //초기화
        text.text = "";
        count = 0;
        listSentences.Clear();
        listSprites.Clear();
        listDialogueWindows.Clear();
        //비활성화
        animSprite.SetBool("Appear", false);
        animDialogueWindow.SetBool("Appear", false);
        talking = false;
    }

    IEnumerator StartTextCoroutine() //only text
    {
        keyActivated = true; //활성화되면 키를 잠가놔서 문장이 전부 출력되고 나서 키가 활성화 되도록 함
        //text 출력
        for (int i = 0; i < listSentences[count].Length; i++) //count번째 문장의 길이만큼 i번 반복, 글자 출력
        {
            ////count 번째문장이 가나다라...이면 
            //[count][i]는 count번째 문장에서 i번째, 가, 나, 다...순서대로 텍스트에 입력됨
            text.text += listSentences[count][i]; //한글자씩 출력.
            if (i % 7 == 1)
            {
                theAudio.Play(typeSound);
            }
            yield return new WaitForSeconds(0.01f);
        }
    }

    IEnumerator StartDialogueCoroutine() 
    {
        if(count>0)
        {
            //대사바(누가말하고 있는지 알려주는창)도 교체
            if (listDialogueWindows[count] != listDialogueWindows[count - 1])
            {
                //사라졌다가 다시 나타나게
                animSprite.SetBool("Change", true); //sprite교체 애니메이션(투명도로 조절) 활성화
                animDialogueWindow.SetBool("Appear", false);
                yield return new WaitForSeconds(0.2f);
                rendererDialogueWindow.GetComponent<SpriteRenderer>().sprite = listDialogueWindows[count]; //윈도우 교체
                rendererSprite.GetComponent<SpriteRenderer>().sprite = listSprites[count]; //스프라이트 교체
                animDialogueWindow.SetBool("Appear", true);
                animSprite.SetBool("Change", false);
            }
            else //대사바가 동일할 경우
            {
                //sprite만 교체
                if (listSprites[count] != listSprites[count - 1]) //두 sprite 이미지가 다르면 교체
                {
                    animSprite.SetBool("Change", true); //sprite교체 애니메이션(투명도로 조절) 활성화
                    yield return new WaitForSeconds(0.1f);
                    //그냥 renderersprite.sprite=listSprites[count]로 해도 됨
                    //(getComponent를 이용해 그 컴포넌트의 속성을 이용할 수 있음)
                    rendererSprite.GetComponent<SpriteRenderer>().sprite = listSprites[count]; //스프라이트 교체
                    animSprite.SetBool("Change", false);
                }
                else //대사바, 스프라이트 일치할 경우
                {
                    yield return new WaitForSeconds(0.05f); //바로 텍스트 출력 방지, 약간의 여유를 둠

                }
            }
                
        }
        else //count==0
        {
            //첫 이미지는 무조건 교체가 이루어져야하므로
            rendererDialogueWindow.GetComponent<SpriteRenderer>().sprite = listDialogueWindows[count]; //윈도우 교체
            rendererSprite.GetComponent<SpriteRenderer>().sprite = listSprites[count]; //스프라이트 교체
        }

        keyActivated = true; //활성화되면 키를 잠가놔서 문장이 전부 출력되고 나서 키가 활성화 되도록 함
        //text 출력
        for (int i = 0; i < listSentences[count].Length; i++) //count번째 문장의 길이만큼 i번 반복, 글자 출력
        {
            ////count 번째문장이 가나다라...이면 
            //[count][i]는 count번째 문장에서 i번째, 가, 나, 다...순서대로 텍스트에 입력됨
            text.text += listSentences[count][i]; //한글자씩 출력.
            if(i%7==1)
            {
                theAudio.Play(typeSound);
            }
            yield return new WaitForSeconds(0.01f);
        }
    }
    // Update is called once per frame
    void Update()
    { 
        go.SetActive(true);
        if (talking && keyActivated) //대화중일때만 'z'키 활성화
        {
            //'z'키를 눌렀을때 다음 문장 출력
            if (Input.GetKeyDown(KeyCode.Z))
            {
                keyActivated = false; //연타해도 키 안먹음
                count++; //다음문장으로 위치변경
                text.text = ""; //문장 나오는 도중 다음문장이 바로 옆에 나와버리면 안되니 과거문장 초기화
                theAudio.Play(enterSound);

                if (count == listSentences.Count) //모든 문장을 다읽었다면, 위에서 count++이었으므로 -1뺌
                {
                    //모든 코루틴 정지
                    StopAllCoroutines();
                    //대화창 삭제해야함
                    ExitDialogue();
                }
                else //다음 문장이 있다면
                {
                    StopAllCoroutines();
                    //텍스트만인지 아닌지
                    if(OnlyText) StartCoroutine(StartTextCoroutine());
                    else StartCoroutine(StartDialogueCoroutine()); //읽는도중에 z키를 누르는경우에 대비

                }

            }
        }
        
    }
}
