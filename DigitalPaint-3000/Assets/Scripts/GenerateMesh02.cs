using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class GenerateMesh02 : MonoBehaviour
{
    /* scratchpad */
    private MeshFilter mf;


    void Start()
    {
        mf = GetComponent<MeshFilter>();
    }
    private void Update()
    {
        if(Tool_Bezier.getInstance().ListeSommetsCurve.Count > 3)
            GenerateMesh();
    }

    private void GenerateMesh()
    {
        var mesh = GetMesh();
        var shape = GetExtrudeShape();
        var path = GetPath();

        Extrude(mesh, shape, path);
    }


    private ExtrudeShape GetExtrudeShape()
    {
        var vert2Ds = new Vertex[] {
            new Vertex(
                new Vector3(0, 0, 0),
                new Vector3(0, 1, 0),
                0),
            new Vertex(
                new Vector3(2, 0, 0),
                new Vector3(0, 1, 0),
                0),
            new Vertex(
                new Vector3(2, 0, 0),
                new Vector3(0, 1, 0),
                0),
            new Vertex(
                new Vector3(4, 0, 0),
                new Vector3(0, 1, 0),
                0)
        };

        var lines = new int[] {
            0, 1,
            1, 2,
            2, 3
        };

        return new ExtrudeShape(vert2Ds, lines);
    }
    private OrientedPoint[] GetPath()
    {
        var p = new Vector3[] {
            new Vector3(0, 0, 0),
            new Vector3(0, 0, 10),
            new Vector3(10, 0, 10),
            new Vector3(10, 0, 0)
        };
        var p2 = Tool_Bezier.getInstance().ListeSommetsCurve;
        var path = new List<OrientedPoint>();

        for (float t = 0; t <= 1; t += 0.1f)
        {
            var point = GetPoint(p2, t);
            var rotation = GetOrientation3D(p2, t, Vector3.up);
            path.Add(new OrientedPoint(point, rotation));
        }

        return path.ToArray();
    }


    private Mesh GetMesh()
    {
        if (mf.sharedMesh == null)
        {
            mf.sharedMesh = new Mesh();
        }
        return mf.sharedMesh;
    }


    private Vector3 GetPoint(List<Vector3> p, float t)
    {
        float omt = 1f - t;
        float omt2 = omt * omt;
        float t2 = t * t;
        return
            p[0] * (omt2 * omt) +
            p[1] * (3f * omt2 * t) +
            p[2] * (3f * omt * t2) +
            p[3] * (t2 * t);
    }


    private Vector3 GetTangent(List<Vector3> p, float t)
    {
        float omt = 1f - t;
        float omt2 = omt * omt;
        float t2 = t * t;
        Vector3 tangent =
            p[0] * (-omt2) +
            p[1] * (3 * omt2 - 2 * omt) +
            p[2] * (-3 * t2 + 2 * t) +
            p[3] * (t2);
        return tangent.normalized;
    }


    private Vector3 GetNormal3D(List<Vector3> p, float t, Vector3 up)
    {
        var tng = GetTangent(p, t);
        var binormal = Vector3.Cross(up, tng).normalized;
        return Vector3.Cross(tng, binormal);
    }


    private Quaternion GetOrientation3D(List<Vector3> p, float t, Vector3 up)
    {
        var tng = GetTangent(p, t);
        var nrm = GetNormal3D(p, t, up);
        return Quaternion.LookRotation(tng, nrm);
    }


    private void Extrude(Mesh mesh, ExtrudeShape shape, OrientedPoint[] path)
    {
        int vertsInShape = shape.vert2Ds.Length;
        int segments = path.Length - 1;
        int edgeLoops = path.Length;
        int vertCount = vertsInShape * edgeLoops;
        int triCount = shape.lines.Length * segments;
        int triIndexCount = triCount * 3;

        var triangleIndices = new int[triIndexCount];
        var vertices = new Vector3[vertCount];
        var normals = new Vector3[vertCount];
        var uvs = new Vector2[vertCount];

        float totalLength = 0;
        float distanceCovered = 0;
        for (int i = 0; i < path.Length - 1; i++)
        {
            var d = Vector3.Distance(path[i].position, path[i + 1].position);
            totalLength += d;
        }

        for (int i = 0; i < path.Length; i++)
        {
            int offset = i * vertsInShape;
            if (i > 0)
            {
                var d = Vector3.Distance(path[i].position, path[i - 1].position);
                distanceCovered += d;
            }
            float v = distanceCovered / totalLength;

            for (int j = 0; j < vertsInShape; j++)
            {
                int id = offset + j;
                vertices[id] = path[i].LocalToWorld(shape.vert2Ds[j].point);
                normals[id] = path[i].LocalToWorldDirection(shape.vert2Ds[j].normal);
                uvs[id] = new Vector2(shape.vert2Ds[j].uCoord, v);
            }
        }
        int ti = 0;
        for (int i = 0; i < segments; i++)
        {
            int offset = i * vertsInShape;
            for (int l = 0; l < shape.lines.Length; l += 2)
            {
                int a = offset + shape.lines[l] + vertsInShape;
                int b = offset + shape.lines[l];
                int c = offset + shape.lines[l + 1];
                int d = offset + shape.lines[l + 1] + vertsInShape;
                triangleIndices[ti] = c; ti++;
                triangleIndices[ti] = b; ti++;
                triangleIndices[ti] = a; ti++;
                triangleIndices[ti] = a; ti++;
                triangleIndices[ti] = d; ti++;
                triangleIndices[ti] = c; ti++;
            }
        }


        mesh.Clear();
        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.uv = uvs;
        mesh.triangles = triangleIndices;
    }


    public struct ExtrudeShape
    {
        public Vertex[] vert2Ds;
        public int[] lines;

        public ExtrudeShape(Vertex[] vert2Ds, int[] lines)
        {
            this.vert2Ds = vert2Ds;
            this.lines = lines;
        }
    }


    public struct Vertex
    {
        public Vector3 point;
        public Vector3 normal;
        public float uCoord;


        public Vertex(Vector3 point, Vector3 normal, float uCoord)
        {
            this.point = point;
            this.normal = normal;
            this.uCoord = uCoord;
        }
    }


    public struct OrientedPoint
    {
        public Vector3 position;
        public Quaternion rotation;


        public OrientedPoint(Vector3 position, Quaternion rotation)
        {
            this.position = position;
            this.rotation = rotation;
        }


        public Vector3 LocalToWorld(Vector3 point)
        {
            return position + rotation * point;
        }


        public Vector3 WorldToLocal(Vector3 point)
        {
            return Quaternion.Inverse(rotation) * (point - position);
        }


        public Vector3 LocalToWorldDirection(Vector3 dir)
        {
            return rotation * dir;
        }
    }
}