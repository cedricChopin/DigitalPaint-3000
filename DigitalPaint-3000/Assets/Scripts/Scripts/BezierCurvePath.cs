/*
Unity Procedural Mesh Bezier Curve Tutorial
Created by BrainFooLong @ July 2016
Youtube Series: https://www.youtube.com/playlist?list=PL8XNf8lax18m-RGjWivOpAqaFBqHDQ-X_
Github Repository: https://github.com/brainfoolong/unity-procedural-mesh-bezier-curve
License: MIT
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BezierCurvePath : MonoBehaviour
{
    /// <summary>
    /// The curves
    /// </summary>
    public List<BezierCurveData> curveDatas = new List<BezierCurveData>();

    /// <summary>
    /// The curve meshes
    /// </summary>
    public List<BezierCurveMesh> curveMeshes = new List<BezierCurveMesh>();

    /// <summary>
    /// The curve granularity, higher is more exact curve lengths
    /// </summary>
    public float curveGranularity = 90f;

    /// <summary>
    /// Expected average curve segment length
    /// </summary>
    public float curveSegmentLength = 5f;

    /// <summary>
    /// The cube thickness
    /// </summary>
    public float cubeThickness = 0.7f;

    /// <summary>
    /// How many quads per row
    /// </summary>
    public int quadsPerRow = 3;

    /// <summary>
    /// The material to assign to planes
    /// </summary>
    public Material material;

    /// <summary>
    /// How many textures per curve (y coordinate), x is always the whole length
    /// </summary>
    public float uvTilesPerCurve = 2f;

    /// <summary>
    /// Generate some curve demo data
    /// </summary>
    void DemoData1()
    {
        var curve = new BezierCurveData();
        curve.points = new Vector3[4]
        {
            new Vector3(0,0,0),
            new Vector3(10,0,0),
            new Vector3(20,5f,10),
            new Vector3(30,10,30)
        };
        curveDatas.Add(curve);

        curve = new BezierCurveData();
        curve.points = new Vector3[4]
        {
            new Vector3(30,10,30),
            new Vector3(40,20,40),
            new Vector3(40,20,40),
            new Vector3(60,-10,100)
        };
        curveDatas.Add(curve);
        
    }

    /// <summary>
    /// Initialize stuff
    /// </summary>
    void Start()
    {
        CreateMesh();
        CreateData();
    }

    /// <summary>
    /// Update positions of the curveData by their anchor game object counterparts
    /// </summary>
    void UpdateCurvePositionsByAnchors()
    {
        foreach(BezierCurveMesh curveMesh in curveMeshes)
        {
            var curveData = curveDatas[curveMesh.curveIndex];
            for (int i = 0; i < curveMesh.anchors.Length; i++)
            {
                var anchor = curveMesh.anchors[i];
                curveData.points[i] = anchor.transform.position - curveMesh.transform.position;
            }
        }
    }

    /// <summary>
    /// Update mesh every frame
    /// </summary>
    void Update()
    {
        if (Tool_Bezier.getInstance().ListeSommets.Count > 2)
        {
            //PointMesh();
            UpdateCurvePositionsByAnchors();
            CreateMesh();
        }
    }

    /// <summary>
    /// Create the mesh
    /// </summary>
    void CreateMesh()
    {
        for (int i = 0; i < curveDatas.Count; i++)
        {
            if(curveMeshes.Count > i)
            {
                curveMeshes[i].CreateMesh();
            }
            else
            {
                var curveMesh = BezierCurveMesh.Instantiate(i, this);
                curveMeshes.Insert(i, curveMesh);
                curveMesh.CreateMesh();
            }
        }
    }

    public void PointMesh()
    {
        int count = Tool_Bezier.getInstance().ListeSommetsCurve.Count;
        int countdivided = count / 4;
        if (Tool_Bezier.getInstance().ListeSommets.Count > 2)
        {


            Transform[] objects = GetComponentsInChildren<Transform>();
            foreach (Transform child in objects)
            {
                if (child != this.transform)
                    GameObject.Destroy(child.gameObject);
            }
            curveMeshes.Clear();
            int j = 0;
            for (int i = 0; i < countdivided; i++)
            {
                
                curveDatas[i].points[0] = Tool_Bezier.getInstance().ListeSommetsCurve[j];
                curveDatas[i].points[1] = Tool_Bezier.getInstance().ListeSommetsCurve[j + 1];
                curveDatas[i].points[2] = Tool_Bezier.getInstance().ListeSommetsCurve[j + 2];
                curveDatas[i].points[3] = Tool_Bezier.getInstance().ListeSommetsCurve[j + 3];
                curveDatas[i].points[0].z += 0.35f;
                curveDatas[i].points[1].z += 0.35f;
                curveDatas[i].points[2].z += 0.35f;
                curveDatas[i].points[3].z += 0.35f;
                j += 4;
            }
        }
    }
    public void CreateData()
    {
        for(int i = 0; i < 25; i++)
        {
            var curve = new BezierCurveData();
            curve.points = new Vector3[4];
            curveDatas.Add(curve);

        }

    }
}
