using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaWave : MonoBehaviour
{
    public MeshFilter meshfilter; // 메쉬로 물을 만들기위해
    public int columnCount = 10; //colum을 생성할 갯수
    public float width = 2.0f; //물 표면 너비
    public float height = 1.0f; // 물 표면의 높이
    public float k = 0.025f; // =
    public float m = 1.0f; // =
    public float drag = 0.025f; // =
    public float spread = 0.025f; //물이 얼마나 퍼질지
    public float isLavaPower;
    public float isNotLavaPower;

    private List<LavaColumn> columns = new List<LavaColumn>(); //LavaColumn리스트 만들기

    private void Start()
    {
        playerScr = FindAnyObjectByType<PlayerControllerRope>();
        Setup();
    }

    void Setup()
    {
        columns.Clear();
        float space = width / columnCount;
        for(int i = 0; i< columnCount + 1; i++)
        {
            columns.Add(new LavaColumn(i * space - width * 0.5f, height, k, m, drag));
        }
    }

    internal int? WorldToColumn(Vector2 position)
    {
        float space = width / columnCount;
        int result = Mathf.RoundToInt((position.x + width * 0.5f) / space);
        if(result >= columns.Count || result < 0)
        {
            return null;
        }
        return result;
    }

    PlayerControllerRope playerScr;

    private float minX = -5f;
    private float maxX = 5f;
    private float minY = -5f;
    private float maxY = 5f;
    private void Update()
    {
        //int? column = WorldToColumn(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        //if (Input.GetMouseButtonDown(0)&&column.HasValue)
        //{
        //    columns[column.Value].velocity = power;
        //}
        MeshRenderer renderer = GetComponent<MeshRenderer>();

        float minX = renderer.bounds.min.x;
        float maxX = renderer.bounds.max.x;
        float minY = renderer.bounds.min.y;
        float maxY = renderer.bounds.max.y;

        Vector2 randomPosition = new Vector2(
            Random.Range(minX, maxX),
            Random.Range(minY, maxY)
        );

        int? column = WorldToColumn(playerScr.isLava ? playerScr.transform.position : randomPosition);
        if (playerScr.isLava && column.HasValue)
        {
            Debug.Log("풍덩");
            columns[column.Value].velocity = isLavaPower;
        }
        else
        {
            Debug.Log("풍덩X"+ randomPosition);

            columns[column.Value].velocity = isNotLavaPower;
        }
    }
    private void FixedUpdate()
    {
        for (int i = 0; i< columns.Count; i++)
        {
            columns[i].UpdateColum();
        }
        float[] leftDeltas = new float[columns.Count];
        float[] rightDeltas = new float[columns.Count];

        for (int i = 0; i < columns.Count; i++)
        {
            if(i>0)
            {
                leftDeltas[i] = (columns[i].height - columns[i - 1].height) * spread;
                columns[i - 1].velocity += leftDeltas[i];
            }
            if(i < columns.Count - 1)
            {
                rightDeltas[i] = (columns[i].height - columns[i + 1].height) * spread;
                columns[i + 1].velocity += rightDeltas[i];
            }
            
        }
        for (int i = 0; i < columns.Count; i++)
        {
            if (i > 0)
            {
                columns[i - 1].height += leftDeltas[i];

            }
            if (i < columns.Count - 1)
            {
                columns[i + 1].height += rightDeltas[i];
            }
        }

        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[columns.Count * 2];
        int v = 0;
        for (int i = 0; i < columns.Count; i++)
        {
            vertices[v] = new Vector2(columns[i].xPos, columns[i].height);
            vertices[v + 1] = new Vector2(columns[i].xPos, 0f);

            v += 2;
        }

        int[] traingles = new int[(columns.Count - 1) * 6];
        int t = 0;
        v = 0;
        for (int i = 0; i < columnCount-1; i++)
        {
            traingles[t] = v;
            traingles[t+1] = v+2;
            traingles[t+2] = v+1;
            traingles[t+3] = v+1;
            traingles[t+4] = v+2;
            traingles[t+5] = v+3;

            v += 2;
            t += 6;
        }
        mesh.vertices = vertices;
        mesh.triangles = traingles;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.Optimize();

        meshfilter.mesh = mesh;
    }
    public class LavaColumn
    {
        public float xPos; //x좌표
        public float height; //높이
        public float targetHeight; //목표 높이
        public float k; //용수철 상수
        public float m; //용수철 질량
        public float velocity; //속도
        public float drag; //공기저항

        public LavaColumn(float xPos,float targetHeight, float k, float m, float drag)
        {
            this.xPos = xPos;
            this.height = targetHeight;
            this.targetHeight = targetHeight;
            this.k = k;
            this.m = m;
            this.drag = drag;
        }
        public void UpdateColum()
        {
            float a = -k / m * (height - targetHeight); //용수철 만들기
            velocity += a; //가속도를 계산하고 속도를 추가
            velocity -= drag * velocity; //공기저항 만들기
            height += velocity; //높이에 속도를 더한다.
        }
    }
}
