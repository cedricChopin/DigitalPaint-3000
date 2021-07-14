using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Tool_Bezier : ScriptableObject
{

public List<Vector3> ListeSommetsCurve;
    public List<Transform> ListeSommets;
    public static Tool_Bezier instance;
    CanvasManagement canvas;
    private bool firstPick = true;
    public int step;
    private bool firstBezier = true;
    public GameObject actualPolygon;
    public LineRenderer curve;
    public GameObject Points;

    private void Awake()
    {
        ListeSommetsCurve = new List<Vector3>();
        ListeSommets = new List<Transform>();
        canvas = GameObject.FindGameObjectWithTag("EditorOnly").GetComponent<CanvasManagement>();
        step = 100;
        
        
    }
    public void Pressed(Vector3 pos, Transform point)  
    {
        if (firstPick)
        {
            Points = new GameObject();
            curve = canvas.DrawALine(Vector3.zero, Vector3.zero, Color.black);
            Points.name = "Points";
            actualPolygon = new GameObject();
            actualPolygon.name = "Bezier";
            Points.transform.parent = actualPolygon.transform;
            curve.transform.parent = actualPolygon.transform;
            Transform pt = Instantiate(point, pos, Quaternion.identity);
            pt.parent = Points.transform;
            ListeSommets.Add(pt);
            firstPick = false;
        }
        else
        {
            Transform pt = Instantiate(point, pos, Quaternion.identity);
            pt.parent = Points.transform;
            ListeSommets.Add(pt);
            if(ListeSommets.Count == 3)
            {
                Bezier();
                firstBezier = false;
            }
            if (!firstBezier)
            {
                Bezier(); 
            }
        }
        
    }

    public static Tool_Bezier getInstance()
    {
        if (instance == null)
        {
            instance = (Tool_Bezier)ScriptableObject.CreateInstance("Tool_Bezier");
        }
        return instance;
    }

    public void Bezier()
    {
        ListeSommetsCurve.Clear();
        
        List<Vector3> SommetsBezier = new List<Vector3>();
        List<Vector3> sommetsTemp;

        foreach (var it in ListeSommets)
        {
            SommetsBezier.Add(it.position);

        }

        Vector3 temp;

        ListeSommetsCurve.Add(SommetsBezier[0]);
        for(int i = 1; i < step; i++)
        {
            sommetsTemp = SommetsBezier;
            for(int j = 0; j < SommetsBezier.Count; j++)
            {
                temp = sommetsTemp[0];
                for(int k = 1; k < sommetsTemp.Count - j; k++)
                {

                    sommetsTemp[k - 1] = Bary(i, step, temp, sommetsTemp[k]);
                    temp = sommetsTemp[k];
                }

            }
            ListeSommetsCurve.Add(sommetsTemp[0]);



        }
        ListeSommetsCurve.Add(SommetsBezier[SommetsBezier.Count - 1]);


        drawCurve();
    }



    Vector3 Bary(int iStep, int step, Vector3 a, Vector3 b)
    {

        float iStepf = (float)iStep;
        float stepf = (float)step;

        return new Vector3(a.x + (iStepf / stepf) * (b.x - a.x), a.y + (iStepf / stepf) * (b.y - a.y), a.z + (iStepf / stepf) * (b.z - a.z));
    }


    public void drawCurve()
    {
        int count = ListeSommetsCurve.Count;
        curve.positionCount = count;
        for(int i = 0; i < count; ++i)
        {
            curve.SetPosition(i, ListeSommetsCurve[i]);

        }
        

    }

}
