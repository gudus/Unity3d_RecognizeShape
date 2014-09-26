using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class DrawLine : MonoBehaviour
{
    public GameObject Prefab;
    
    public float NetStep = 1;
    public int NetLineSize = 1;
    public Texture2D NetLineTexture;
    public float DrawLineSize = 0.1f;
    public Color DrawColor = Color.green;

    public float DrawSensitivity = 1f;
    public float SizeInterpolateScaleBezie = 1f;
    public int IStep = 1;

    private bool NetDraw = false;
    private LineRenderer line;
    private bool isMousePressed;
    private List<Vector3> pointsList;
    private Vector3 mousePos;

    // Structure for line points
    struct myLine
    {
        public Vector3 StartPoint;
        public Vector3 EndPoint;
    };
    //    -----------------------------------    
    void Awake()
    {
        // Create line renderer component and set its property
        line = gameObject.AddComponent<LineRenderer>();
        line.material = new Material(Shader.Find("Particles/Additive"));
        line.SetVertexCount(0);
        line.SetWidth(DrawLineSize, DrawLineSize);
        line.SetColors(DrawColor, DrawColor);
        line.useWorldSpace = true;
        isMousePressed = false;
        pointsList = new List<Vector3>();
    }
    //    -----------------------------------    
    void Update()
    {
        // If mouse button down, remove old line and set its color to green
        if (Input.GetMouseButtonDown(0))
        {
            isMousePressed = true;
            line.SetVertexCount(0);
            pointsList.RemoveRange(0, pointsList.Count);
            line.SetColors(DrawColor, DrawColor);
            NetDraw = false;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isMousePressed = false;
            NetDraw = true;
            checkShape(pointsList);
        }
        // Drawing line when mouse is moving(presses)
        if (isMousePressed)
        {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            if (!pointsList.Contains(mousePos))
            {
                pointsList.Add(mousePos);
                line.SetVertexCount(pointsList.Count);
                line.SetPosition(pointsList.Count - 1, (Vector3)pointsList[pointsList.Count - 1]);

                if (isLineCollide())
                {
                    isMousePressed = false;
                    line.SetColors(Color.red, Color.red);
                }

                
            }
        }
    }
    //    -----------------------------------    
    //  Following method checks is currentLine(line drawn by last two points) collided with line 
    //    -----------------------------------    
    private bool isLineCollide()
    {
        if (pointsList.Count < 2)
            return false;
        int TotalLines = pointsList.Count - 1;
        myLine[] lines = new myLine[TotalLines];
        if (TotalLines > 1)
        {
            for (int i = 0; i < TotalLines; i++)
            {
                lines[i].StartPoint = (Vector3)pointsList[i];
                lines[i].EndPoint = (Vector3)pointsList[i + 1];
            }
        }
        for (int i = 0; i < TotalLines - 1; i++)
        {
            myLine currentLine;
            currentLine.StartPoint = (Vector3)pointsList[pointsList.Count - 2];
            currentLine.EndPoint = (Vector3)pointsList[pointsList.Count - 1];
            if (isLinesIntersect(lines[i], currentLine))
                return true;
        }
        return false;
    }
    //    -----------------------------------    
    //    Following method checks whether given two points are same or not
    //    -----------------------------------    
    private bool checkPoints(Vector3 pointA, Vector3 pointB)
    {
        return (pointA.x == pointB.x && pointA.y == pointB.y);
    }
    //    -----------------------------------    
    //    Following method checks whether given two line intersect or not
    //    -----------------------------------    
    private bool isLinesIntersect(myLine L1, myLine L2)
    {
        if (checkPoints(L1.StartPoint, L2.StartPoint) ||
            checkPoints(L1.StartPoint, L2.EndPoint) ||
            checkPoints(L1.EndPoint, L2.StartPoint) ||
            checkPoints(L1.EndPoint, L2.EndPoint))
            return false;

        return ((Mathf.Max(L1.StartPoint.x, L1.EndPoint.x) >= Mathf.Min(L2.StartPoint.x, L2.EndPoint.x)) &&
               (Mathf.Max(L2.StartPoint.x, L2.EndPoint.x) >= Mathf.Min(L1.StartPoint.x, L1.EndPoint.x)) &&
               (Mathf.Max(L1.StartPoint.y, L1.EndPoint.y) >= Mathf.Min(L2.StartPoint.y, L2.EndPoint.y)) &&
               (Mathf.Max(L2.StartPoint.y, L2.EndPoint.y) >= Mathf.Min(L1.StartPoint.y, L1.EndPoint.y))
               );
    }

    void OnGUI()
    {
        if (NetDraw)
        {
            //DrawNet();
            if (pointsList.Count > 0)
            {
                Vector3 A = pointsList[0];
                Vector3 B = pointsList[pointsList.Count - 1];
                List<Vector2> list = GetDirectLine(A, B, 0.1f);
                if (list != null)
                {
                    GUI.Label(new Rect(20, 20, 200, 40), ("pointListCount=" + pointsList.Count + " listDirectCount=" + list.Count));
                }
            }
        }
    }
    
    private void DrawNet()
    {
        Vector3 topVec = new Vector3(0, float.MinValue);
        Vector3 botVec = new Vector3(0, float.MaxValue);

        for (int i = 0; i < pointsList.Count; i++)
        {
            if (pointsList[i].y > topVec.y)
            {
                topVec = pointsList[i];
            }
            if (pointsList[i].y < botVec.y)
            {
                botVec = pointsList[i];
            }
        }

        Vector3 leftVec = new Vector3(float.MaxValue,0);
        Vector3 rightVec = new Vector3(float.MinValue, 0);

        for (int i = 0; i < pointsList.Count; i++)
        {
            if (pointsList[i].x < leftVec.x)
            {
                leftVec = pointsList[i];
            }

            if (pointsList[i].x > rightVec.x)
            {
                rightVec = pointsList[i];
            }
        }
        List<Vector3> tempList = new List<Vector3>();

        //for (float i = leftVec.x; i < rightVec.x; i = i + NetStep)
        //{
        //    for (float a = botVec.y; a < topVec.y; a = a + NetStep)
        //    {
        //        tempList.Add(new Vector3(i, a));
        //    }
        //}

        for (float i = leftVec.x-1; i <= rightVec.x+1; i = i + NetStep)
        {
            tempList.Add(new Vector3(i, botVec.y));
            tempList.Add(new Vector3(i, topVec.y));
        }

        for (float i = botVec.y - 1; i <= topVec.y + 1; i = i + NetStep)
        {
            tempList.Add(new Vector3(leftVec.x, i));
            tempList.Add(new Vector3(rightVec.x, i));
        }

        if (tempList.Count > 1)
        {
            for (int i = 0; i < tempList.Count; i=i+2)
            {
                if (i + 1 < tempList.Count)
                {
                    Vector3 A = WorldToGuiPoint(tempList[i]);
                    Vector3 B = WorldToGuiPoint(tempList[i + 1]);
                    DrawL(A, B, NetLineSize);
                }
            }
        }
    }

    private void DrawL(Vector2 start, Vector2 end, int width)
    {
        Matrix4x4 matrix = GUI.matrix;
        Color savedColor = GUI.color;
        GUI.color = Color.red;

        if (!NetLineTexture) { NetLineTexture = new Texture2D(1, 1); }
        Vector2 d = end - start;
        float a = Mathf.Rad2Deg * Mathf.Atan(d.y / d.x);
        if (d.x < 0)
            a += 180;
        int width2 = (int)Mathf.Ceil(width / 2);
        GUIUtility.RotateAroundPivot(a, start);
        GUI.DrawTexture(new Rect(start.x, start.y - width2, d.magnitude, width), NetLineTexture);
        GUIUtility.RotateAroundPivot(-a, start);

        GUI.matrix = matrix;
        GUI.color = savedColor;
    }

    private Vector3 WorldToGuiPoint(Vector3 position)
    {
        var guiPosition = Camera.main.WorldToScreenPoint(position);
        guiPosition.y = Screen.height - guiPosition.y;
        return guiPosition;
    }

    //количество углов в масиве точек через заданный угол
    List<Vector3> listAddPref = new List<Vector3>();
    private int GetCountVectex(List<Vector3> list, float angle,float error)
    {
        int countCheckSteps = 3;

        List<Vector3> listTemps = new List<Vector3>();
        for (int i = 0; i < list.Count; i++)
        {
            if (i >= countCheckSteps)
            {
                float a = Vector3.Distance(list[i - countCheckSteps], list[i - 1]);
                float b = Vector3.Distance(list[i - 1], list[i]);
                float c = Vector3.Distance(list[i], list[i - countCheckSteps]);
                //углы
                float A = Mathf.Acos((b * b + c * c - a * a) / (2 * b * c)) * 180 / Mathf.PI;
                float B = Mathf.Acos((a * a + c * c - b * b) / (2 * a * c)) * 180 / Mathf.PI;
                float C = Mathf.Acos((a * a + b * b - c * c) / (2 * a * b)) * 180 / Mathf.PI;
                if ((C > angle - error) & ((C <angle + error)) && !listTemps.Contains(list[i]))
                {
                    Instantiate(Prefab, list[i], Quaternion.identity);
                    listTemps.Add(list[i]);
                    
                }
                Debug.Log("Count=" + listAddPref.Count + " a=" + A + "b=" + B + "c=" + C);
            }
        }
        return listTemps.Count;
    }

    //массив точек по прямой от точки к точке
    int countC = 1000;
    private List<Vector2> GetDirectLine(Vector2 start, Vector2 end, float step)
    {
        List<Vector2> list = new List<Vector2>();

        float dx = (end.x - start.x);
        float dy = (end.y - start.y);

        float line = Mathf.Sqrt(dx * dx + dy * dy);

        dx = step * dx / line;
        dy = step * dy / line;
        float dc = line;

        while (dc >= .1f && countC > 0)
        {
            start.x += dx;
            start.y += dy;
            dc -= Mathf.Sqrt(dx * dx + dy * dy);
            list.Add(new Vector2(start.x, start.y));
            //countC--;
        }

        if (list.Count > 1)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (i + 1 < list.Count)
                {
                    Vector3 A1 = WorldToGuiPoint(list[i]);
                    Vector3 B1 = WorldToGuiPoint(list[i + 1]);
                    DrawL(A1, B1, NetLineSize);
                    if (!listAddPref.Contains(list[i]))
                    {
                        GameObject go = (GameObject)Instantiate(Prefab, list[i], Quaternion.identity);
                        go.name = "point" + i;
                        listAddPref.Add(list[i]);
                    }
                }
            }
        }
        return list;
    }

    public float deviation = 30f;
    public int EveryPoint = 3;
    public float AngleError = 15f;
    private void checkShape(List<Vector3> list)
    {
        int CountVertex = GetCountVectex(list, 90, AngleError);

        if (CountVertex < 2)
        {
            bool isCircle = true;
            float massX = 0;
            float massY = 0;
            for (int i = 0; i < list.Count; i++)
            {
                massX += list[i].x;
                massY += list[i].x;
            }
            Vector2 CenterOfMass = new Vector2(massX / list.Count, massY / list.Count);

            float distFromCenterToPoint = GetVectorDistance(CenterOfMass, list[0]);
            float interval = (distFromCenterToPoint * deviation) / 100;
            float temoDist = 0;

            for (int i = 0; i < list.Count; i = i + EveryPoint)
            {
                temoDist = GetVectorDistance(CenterOfMass, list[i]);
                if (temoDist > distFromCenterToPoint + interval | temoDist < distFromCenterToPoint - interval)
                {
                    isCircle = false;
                    break;
                }
            }
            Debug.Log("isCircle=" + isCircle);
        }
        else
        {
            Debug.Log("Much angles to the circle");
        }
        
    }

    private float GetVectorDistance(Vector2 from, Vector2 to)
    {
        return Mathf.Sqrt(Mathf.Pow(to.x - from.x, 2) + Mathf.Pow(to.y - from.y, 2));
    }
}
