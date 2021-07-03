using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tool_Polygone : ScriptableObject
{
    public List<Transform> ListePos;
    public static Tool_Polygone instance;
    CanvasManagement canvas;
    private bool firstPick = true;
    public GameObject actualPolygon;
    
    private void Awake()
    {
        ListePos = new List<Transform>();
        canvas = GameObject.FindGameObjectWithTag("EditorOnly").GetComponent<CanvasManagement>();
    }
    public void Pressed(Vector3 pos, Transform point)  
    {
        if (firstPick)
        {
            
            actualPolygon = new GameObject();
            actualPolygon.name = "Polygon";
            Transform pt = Instantiate(point, pos, Quaternion.identity);
            pt.parent = actualPolygon.transform;
            ListePos.Add(pt);
            firstPick = false;
        }
        else
        {
            Transform pt = Instantiate(point, pos, Quaternion.identity);
            pt.parent = actualPolygon.transform;
            LineRenderer line = canvas.DrawALine(ListePos[ListePos.Count - 1].position, pt.position, canvas.actualColor);
            line.transform.parent = actualPolygon.transform;
            ListePos.Add(pt);
        }
        
    }

    public static Tool_Polygone getInstance()
    {
        if (instance == null)
        {
            instance = (Tool_Polygone)ScriptableObject.CreateInstance("Tool_Polygone");
        }
        return instance;
    }

    public void End_Polygon()
    {

        LineRenderer line = canvas.DrawALine(ListePos[ListePos.Count - 1].position, ListePos[0].position, Color.blue);
        line.transform.parent = actualPolygon.transform;
        firstPick = true;
        canvas.MultiSommets.Add(ListePos);
        ListePos.Clear();
    }
}
