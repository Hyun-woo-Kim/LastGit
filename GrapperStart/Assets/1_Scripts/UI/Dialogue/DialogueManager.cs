using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class DialogueManager : MonoBehaviour
{
    Dictionary<int, string[]> talkData; //대화 데이터
    Dictionary<int, Sprite> portraitData; //초상화 데이터
    public Sprite[] portraitArr; //초상화 데이터 번호

    public static DialogueManager D_instance;
    void Awake()
    {
        if (D_instance == null)
        {
            D_instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        talkData = new Dictionary<int, string[]>();
        portraitData = new Dictionary<int, Sprite>();
        GenerateData();
    }

    void GenerateData()
    {

        //id = 1000 : npc
        talkData.Add(1000, new string[] { "안녕하세요!:1",
            "아이고 안녕하세요!:0",
            "잘가세요:1",
            "싫어요:0",
            "그래요:1",
            "맛있게드세요:0",
            "감사해요:1" });

        talkData.Add(2000, new string[] { " " }); //NPC마다 그냥 상호작용 했을 때 내용을 추가해주면 됨 2000은 오브젝트 id를 나타냄

        //퀘스트용 대화(obj id + quest id + questIndex(순서번호))
        //퀘스트용 대화(obj id + quest id)

        //퀘스트용 대화(obj id + quest id) (obj id + quest id + questIndex)를 번호로 저장
        talkData.Add(1000 + 10, new string[] { "엥? 이런곳에 사람이?!?!:2", "저기요! \n저 좀 도와주세요:0", "왜 여기 갇혀 계세요?:2",
            "저도 잘 모르겠어요. \n저희 아버지가 좀 이상해서요:0", "제가 어떻게 해드리면 될까요?:2", "몬스터들이 열쇠를 가지고 있어요. \n그것을 구해다 주시면 제가 좋은 기술을 알려드릴게요!:0",
            "네! 잠시만 기다리세요:2" });
        talkData.Add(2000 + 10 + 1, new string[] { "안녕:1", "안녕하세요? \n혹시 열쇠를 구할 수 있는 곳을 아시나요?:2", "열쇠? 열쇠라면 나도 하나 가지고있어:1", "혹시 어디서 얻으셨나요?:2",
            "몬스터가 가지고 있길래 \n 해치우고 나서 일단 가지고 있어:1", "혹시 저에게 주실 수 있나요?:2", "뭐.. 별로 필요하지는 않았으니까 좋아\n 가져가도록해:1", "감사합니다:2" });


        //20번 퀘스트 아이템을 얻으면 찾았고 그다음 오브젝트와 대화 가능
        talkData.Add(1000 + 20, new string[] { "아이템을 찾아주세요. \n부탁드려요:0", });
        talkData.Add(2000 + 20, new string[] { "아직 못찾았니? \n내가 마지막으로 봤을때는 문 옆에 있었어:1", }); //퀘스트 완료 전에 대화를 걸었을 때
        talkData.Add(300 + 20, new string[] { "아이템를 찾았다", }); //300이 아이템 오브젝트
        talkData.Add(1000 + 20 + 1, new string[] { "아이템를 찾아줘서 고마워요!:0", "별 말씀을요!:2", }); //열쇠를 찾은 후에 

        //30번 퀘스트 
        talkData.Add(1000 + 30, new string[] { "열쇠를 찾아줘서 고마워요!:0", "별 말씀을요!:2", });

        //초상화 생성
        portraitData.Add(1000 + 0, portraitArr[0]); //0번 인덱스에 저장된 초상화를 id = 100과 mapping
        portraitData.Add(1000 + 1, portraitArr[1]); //1번 인덱스에 저장된 초상화를 id = 101과 mapping
    }

    public string GetTalk(int id, int talkIndex) //Object의 id , string배열의 index
    {
        //1. 해당 퀘스트 id에서 퀘스트index(순서)에 해당하는 대사가 없음
        if (!talkData.ContainsKey(id))
        {

            //해당 퀘스트 자체에 대사가 없을 때 -> 기본 대사를 불러옴 (십, 일의 자리 부분 제거 )
            if (!talkData.ContainsKey(id - id % 10))
                return GetTalk(id - id % 100, talkIndex);//GET FIRST TALK

            //
            else
                return GetTalk(id - id % 10, talkIndex);//GET FIRST QUEST TALK
        }

        //2. 해당 퀘스트 id에서 퀘스트index(순서)에 해당하는 대사가 있음
        if (talkIndex == talkData[id].Length)
            return null;
        else
            return talkData[id][talkIndex]; //해당 아이디의 해당

    }

    public Sprite GetPortrait(int id, int portraitIndex) //초상화 가져오기
    {
        //id는 NPC넘버 , portraitIndex : 표정번호(?)
        return portraitData[id + portraitIndex];
    }
}
