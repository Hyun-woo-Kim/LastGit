using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialogue 
{
    public string name;

    [TextArea(3, 10)]
    public string[] sentences; //다음 버튼을 누르면 다음대화 이어지게
}
