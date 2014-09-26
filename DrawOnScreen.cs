using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DrawOnScreen : MonoBehaviour {

    public float DrawLineSize = 0.1f;
    public Color DrawColor = Color.green;

    private LineRenderer line;
    private bool isMousePressed;
    private List<Vector3> pointsList;
    private Vector3 mousePos;

    void Awake()
    {
        line = gameObject.AddComponent<LineRenderer>();
        line.material = new Material(Shader.Find("Particles/Additive"));
        line.SetVertexCount(0);
        line.SetWidth(DrawLineSize, DrawLineSize);
        line.SetColors(DrawColor, DrawColor);
        line.useWorldSpace = true;
        isMousePressed = false;
        pointsList = new List<Vector3>();
    }
   
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isMousePressed = true;
            line.SetVertexCount(0);
            pointsList.RemoveRange(0, pointsList.Count);
            line.SetColors(DrawColor, DrawColor);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isMousePressed = false;
            //GetCountVectex2(pointsList, 10, 1);
            CheckLine(pointsList);
        }

        if (isMousePressed)
        {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            if (!pointsList.Contains(mousePos))
            {
                pointsList.Add(mousePos);
                line.SetVertexCount(pointsList.Count);
                line.SetPosition(pointsList.Count - 1, (Vector3)pointsList[pointsList.Count - 1]);
            }
        }
    }

    //количество углов в масиве точек
    private int GetCountVectex(List<Vector3> list, float angle, float error)
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
                if ((C > angle - error) & (C < angle + error) && !listTemps.Contains(list[i]))
                {
                    listTemps.Add(list[i]);
                }
            }
        }
        return listTemps.Count;
    }

    public int stepCount = 1;
    private int GetCountVectex2(List<Vector3> list, float angle, float error)
    {
        List<Vector3> listTemps = new List<Vector3>();
        for (int i = 0; i < list.Count; i++)
        {
            if ((i + stepCount * 2 )< list.Count)
            {
                //получаем стороны треугольника
                float a = GetVectorDistance(list[i], list[i + stepCount]);
                float b = GetVectorDistance(list[i + stepCount], list[i + stepCount * 2]);
                float c = GetVectorDistance(list[i + stepCount * 2], list[i]);
                //находим углы треугольника
                float A = Mathf.Acos((b * b + c * c - a * a) / (2 * b * c)) * 180 / Mathf.PI;
                float B = Mathf.Acos((a * a + c * c - b * b) / (2 * a * c)) * 180 / Mathf.PI;
                float C = Mathf.Acos((a * a + b * b - c * c) / (2 * a * b)) * 180 / Mathf.PI;
                Debug.Log(" <A=" + A + " <B=" + b + " <C=" + c);
            }
        }
        return listTemps.Count;
    }

    private float GetVectorDistance(Vector2 from, Vector2 to)
    {
        return Mathf.Sqrt(Mathf.Pow(to.x - from.x, 2) + Mathf.Pow(to.y - from.y, 2));
    }

    public float CheckPointOnLineError = 0.2f;
    private void CheckLine(List<Vector3> list)
    {
        if (list.Count > 0)
        {
            Vector2 a = list[0];
            Vector2 b = list[list.Count - 1];
            for (int i = 1; i < list.Count-1; i++)
            {
                float poinCheck=GetCheckPoint(a,b,list[i]);
                if (poinCheck > -CheckPointOnLineError & poinCheck <CheckPointOnLineError)
                {
                    Debug.Log("Point on the line " + poinCheck);
                }
                else if (poinCheck > CheckPointOnLineError)
                {
                    Debug.Log("Point left of the line " + poinCheck);
                }
                else if (poinCheck < -CheckPointOnLineError)
                {
                    Debug.Log("Point right of the line " + poinCheck);
                }
            }
        }
    }
    
    /// <summary>
    /// проверка где находится точка относительно прямой
    /// </summary>
    /// <param name="a">начало прямой</param>
    /// <param name="b">конец прямой</param>
    /// <param name="c">точка</param>
    /// <returns></returns>
    private float GetCheckPoint(Vector2 a, Vector2 b, Vector2 c)
    {
        //return (b.x - a.x) * (b.y - c.y) - (b.y - a.y) * (b.x - c.x);
        return (c.x - a.x) * (c.y - b.y) - (c.y - a.y) * (c.x - b.x);
    } 
}
