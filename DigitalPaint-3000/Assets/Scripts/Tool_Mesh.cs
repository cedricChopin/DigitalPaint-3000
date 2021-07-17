using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using SplineMesh;
using System.Linq;

public class Tool_Mesh : MonoBehaviour
{
    private List<Vertex> shapeVertices = new List<Vertex>();
    public readonly Vector2 scale = new Vector2(1,1);
    private MeshFilter mf;
    private Mesh result;
    private Quaternion rotation = Quaternion.identity;
    private float roll = 1;
    private Vector3 up;
    private Vector3 tangent;
    public readonly Vector3 location = Vector3.one;
    private void Start()
    {
        shapeVertices.Clear();
        shapeVertices.Add(new Vertex(new Vector2(0, 0.5f), new Vector2(0, 1), 0));
        shapeVertices.Add(new Vertex(new Vector2(1, -0.5f), new Vector2(1, -1), 0.33f));
        shapeVertices.Add(new Vertex(new Vector2(-1, -0.5f), new Vector2(-1, -1), 0.66f));
    }

    private void OnEnable()
    {
        mf = GetComponent<MeshFilter>();
        if (mf.sharedMesh == null)
        {
            mf.sharedMesh = new Mesh();
        }
    }
    private void LateUpdate()
    {
        if (Tool_Bezier.getInstance().ListeSommets.Count > 3)
        {
            int vertsInShape = shapeVertices.Count;

            var triangleIndices = new List<int>(vertsInShape * 2 * 3);
            var bentVertices = new List<MeshVertex>(vertsInShape * 2 * 3);
            foreach (var Point in Tool_Bezier.getInstance().ListeSommetsCurve)
            {
                foreach (Vertex v in shapeVertices)
                {
                    bentVertices.Add(GetBent(new MeshVertex(
                        new Vector3(0, v.point.y, -v.point.x),
                        new Vector3(0, v.normal.y, -v.normal.x),
                        new Vector2(v.uCoord, -v.uCoord))));
                }
            }
            var index = 0;
            for (int i = 0; i < Tool_Bezier.getInstance().ListeSommets.Count; i++)
            {
                for (int j = 0; j < shapeVertices.Count; j++)
                {
                    int offset = j == shapeVertices.Count - 1 ? -(shapeVertices.Count - 1) : 1;
                    int a = index + shapeVertices.Count;
                    int b = index;
                    int c = index + offset;
                    int d = index + offset + shapeVertices.Count;
                    triangleIndices.Add(c);
                    triangleIndices.Add(b);
                    triangleIndices.Add(a);
                    triangleIndices.Add(a);
                    triangleIndices.Add(d);
                    triangleIndices.Add(c);
                    index++;
                }
            }

            MeshUtility.Update(mf.sharedMesh,
                mf.sharedMesh,
                triangleIndices,
                bentVertices.Select(b => b.position),
                bentVertices.Select(b => b.normal),
                bentVertices.Select(b => b.uv));
        }
    }



    public MeshVertex GetBent(MeshVertex vert)
    {
        var res = new MeshVertex(vert.position, vert.normal, vert.uv);

        // application of scale
        res.position = Vector3.Scale(res.position, new Vector3(0, scale.y, scale.x));

        // application of roll
        res.position = Quaternion.AngleAxis(roll, Vector3.right) * res.position;
        res.normal = Quaternion.AngleAxis(roll, Vector3.right) * res.normal;

        // reset X value
        res.position.x = 0;

        // application of the rotation + location
        Quaternion q = Rotation * Quaternion.Euler(0, -90, 0);
        res.position = q * res.position + location;
        res.normal = q * res.normal;
        return res;
    }

    public Quaternion Rotation
    {
        get
        {
            if (rotation == Quaternion.identity)
            {
                var upVector = Vector3.Cross(tangent, Vector3.Cross(Quaternion.AngleAxis(roll, Vector3.forward) * up, tangent).normalized);
                rotation = Quaternion.LookRotation(tangent, upVector);
            }
            return rotation;
        }
    }
}



[Serializable]
public class Vertex
{
    public Vector2 point;
    public Vector2 normal;
    public float uCoord;

    public Vertex(Vector2 point, Vector2 normal, float uCoord)
    {
        this.point = point;
        this.normal = normal;
        this.uCoord = uCoord;
    }
    public Vertex(Vertex other)
    {
        this.point = other.point;
        this.normal = other.normal;
        this.uCoord = other.uCoord;
    }
}


[Serializable]
public class MeshVertex
{
    public Vector3 position;
    public Vector3 normal;
    public Vector2 uv;

    public MeshVertex(Vector3 position, Vector3 normal, Vector2 uv)
    {
        this.position = position;
        this.normal = normal;
        this.uv = uv;
    }

    public MeshVertex(Vector3 position, Vector3 normal)
        : this(position, normal, Vector2.zero)
    {
    }
}