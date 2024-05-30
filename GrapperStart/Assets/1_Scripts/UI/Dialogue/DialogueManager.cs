using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class DialogueManager : MonoBehaviour
{
    Dictionary<int, string[]> talkData; //��ȭ ������
    Dictionary<int, Sprite> portraitData; //�ʻ�ȭ ������
    public Sprite[] portraitArr; //�ʻ�ȭ ������ ��ȣ

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
        talkData.Add(1000, new string[] { "�ȳ��ϼ���!:1",
            "���̰� �ȳ��ϼ���!:0",
            "�߰�����:1",
            "�Ⱦ��:0",
            "�׷���:1",
            "���ְԵ弼��:0",
            "�����ؿ�:1" });

        talkData.Add(2000, new string[] { " " }); //NPC���� �׳� ��ȣ�ۿ� ���� �� ������ �߰����ָ� �� 2000�� ������Ʈ id�� ��Ÿ��

        //����Ʈ�� ��ȭ(obj id + quest id + questIndex(������ȣ))
        //����Ʈ�� ��ȭ(obj id + quest id)

        //����Ʈ�� ��ȭ(obj id + quest id) (obj id + quest id + questIndex)�� ��ȣ�� ����
        talkData.Add(1000 + 10, new string[] { "��? �̷����� �����?!?!:2", "�����! \n�� �� �����ּ���:0", "�� ���� ���� �輼��?:2",
            "���� �� �𸣰ھ��. \n���� �ƹ����� �� �̻��ؼ���:0", "���� ��� �ص帮�� �ɱ��?:2", "���͵��� ���踦 ������ �־��. \n�װ��� ���ش� �ֽø� ���� ���� ����� �˷��帱�Կ�!:0",
            "��! ��ø� ��ٸ�����:2" });
        talkData.Add(2000 + 10 + 1, new string[] { "�ȳ�:1", "�ȳ��ϼ���? \nȤ�� ���踦 ���� �� �ִ� ���� �ƽó���?:2", "����? ������ ���� �ϳ� �������־�:1", "Ȥ�� ��� �����̳���?:2",
            "���Ͱ� ������ �ֱ淡 \n ��ġ��� ���� �ϴ� ������ �־�:1", "Ȥ�� ������ �ֽ� �� �ֳ���?:2", "��.. ���� �ʿ������� �ʾ����ϱ� ����\n ������������:1", "�����մϴ�:2" });


        //20�� ����Ʈ �������� ������ ã�Ұ� �״��� ������Ʈ�� ��ȭ ����
        talkData.Add(1000 + 20, new string[] { "�������� ã���ּ���. \n��Ź�����:0", });
        talkData.Add(2000 + 20, new string[] { "���� ��ã�Ҵ�? \n���� ���������� �������� �� ���� �־���:1", }); //����Ʈ �Ϸ� ���� ��ȭ�� �ɾ��� ��
        talkData.Add(300 + 20, new string[] { "�����۸� ã�Ҵ�", }); //300�� ������ ������Ʈ
        talkData.Add(1000 + 20 + 1, new string[] { "�����۸� ã���༭ ������!:0", "�� ��������!:2", }); //���踦 ã�� �Ŀ� 

        //30�� ����Ʈ 
        talkData.Add(1000 + 30, new string[] { "���踦 ã���༭ ������!:0", "�� ��������!:2", });

        //�ʻ�ȭ ����
        portraitData.Add(1000 + 0, portraitArr[0]); //0�� �ε����� ����� �ʻ�ȭ�� id = 100�� mapping
        portraitData.Add(1000 + 1, portraitArr[1]); //1�� �ε����� ����� �ʻ�ȭ�� id = 101�� mapping
    }

    public string GetTalk(int id, int talkIndex) //Object�� id , string�迭�� index
    {
        //1. �ش� ����Ʈ id���� ����Ʈindex(����)�� �ش��ϴ� ��簡 ����
        if (!talkData.ContainsKey(id))
        {

            //�ش� ����Ʈ ��ü�� ��簡 ���� �� -> �⺻ ��縦 �ҷ��� (��, ���� �ڸ� �κ� ���� )
            if (!talkData.ContainsKey(id - id % 10))
                return GetTalk(id - id % 100, talkIndex);//GET FIRST TALK

            //
            else
                return GetTalk(id - id % 10, talkIndex);//GET FIRST QUEST TALK
        }

        //2. �ش� ����Ʈ id���� ����Ʈindex(����)�� �ش��ϴ� ��簡 ����
        if (talkIndex == talkData[id].Length)
            return null;
        else
            return talkData[id][talkIndex]; //�ش� ���̵��� �ش�

    }

    public Sprite GetPortrait(int id, int portraitIndex) //�ʻ�ȭ ��������
    {
        //id�� NPC�ѹ� , portraitIndex : ǥ����ȣ(?)
        return portraitData[id + portraitIndex];
    }
}
