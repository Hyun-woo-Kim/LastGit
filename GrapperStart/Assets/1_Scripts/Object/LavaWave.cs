using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaWave : MonoBehaviour
{
    public MeshFilter meshfilter; // �޽��� ���� ���������
    public int columnCount = 10; //colum�� ������ ����
    public float width = 2.0f; //�� ǥ�� �ʺ�
    public float height = 1.0f; // �� ǥ���� ����
    public float k = 0.025f; // =
    public float m = 1.0f; // =
    public float drag = 0.025f; // =
    public float spread = 0.025f; //���� �󸶳� ������
    public float isLavaPower;
    public float isNotLavaPower;

    private List<LavaColumn> columns = new List<LavaColumn>(); //LavaColumn����Ʈ �����

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
            Debug.Log("ǳ��");
            columns[column.Value].velocity = isLavaPower;
        }
        else
        {
            Debug.Log("ǳ��X"+ randomPosition);

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
        public float xPos; //x��ǥ
        public float height; //����
        public float targetHeight; //��ǥ ����
        public float k; //���ö ���
        public float m; //���ö ����
        public float velocity; //�ӵ�
        public float drag; //��������

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
            float a = -k / m * (height - targetHeight); //���ö �����
            velocity += a; //���ӵ��� ����ϰ� �ӵ��� �߰�
            velocity -= drag * velocity; //�������� �����
            height += velocity; //���̿� �ӵ��� ���Ѵ�.
        }
    }
}
