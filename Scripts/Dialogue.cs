using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//custom class

[System.Serializable]
public class Dialogue 
{
    [TextArea(1,2)] //문장 입력 칸 크기 조절, 2칸
    public string[] sentences; //스토리의 대사
    public Sprite[] sprites; //스프라이트
    public Sprite[] dialogueWindows; //누가 대화하는지 대화창
}
