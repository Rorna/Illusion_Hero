using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//커스텀 클래스
[System.Serializable]
public class Choice 
{
    public string question; //질문
    public string[] answers; //답변(최대 1~4허용)
}
