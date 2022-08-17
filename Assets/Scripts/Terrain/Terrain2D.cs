using System.Collections.Generic;
using UnityEngine;

namespace Terrain

{
public class Terrain2D : MonoBehaviour
{
    public Transform backCollider;
    public BoxCollider bc;
    public BoxCollider2D bc2d;
    private int _called;
    public float depth = 0.01f;
    public Color32[] mColors;
    private int _mHeight;
    private readonly List<Vector3> _mNormals = new List<Vector3>();

    private readonly List<Vector3> _mVertices = new List<Vector3>();
    private int _mWidth;
    public MeshCollider mc;
    public SpriteRenderer sprite;
    public Terrain2DMain terrain;
    public Texture2D texture;
    public int x;
    public int y;


    private bool HasPixel(int aX, int aY)
    {
        return mColors[aX + aY * _mWidth].a > 50;
    }

    private void Start()
    {
        mColors = texture.GetPixels32();
        GenCollider();
    }


    public void GenCollider()
    {
        _called++;
        GenerateMesh();
    }

    private void Update()
    {
    }

    public void Dig(Vector3 p)
    {
        Vector2 pos = p;

        pos = transform.InverseTransformPoint(pos);
        pos.x += 0.5f;
        pos.y += 0.5f;

        pos.x *= texture.width;
        pos.y *= texture.height;

        float i = (int) pos.x + (int) pos.y;

        if (i >= 0 && i <= texture.width * texture.height)
        {
            mColors[(int) i] = Color.clear;


            texture.SetPixel((int) pos.x, (int) pos.y, Color.clear);
            texture.Apply();
            GenCollider();
        }
    }

    private void AddQuad(Vector3 aFirstEdgeP1, Vector3 aFirstEdgeP2, Vector3 aSecondRelative, Vector3 aNormal,
        Vector2 aUv1, Vector2 aUv2, bool aFlipUVs)
    {
        _mVertices.Add(aFirstEdgeP1);
        _mVertices.Add(aFirstEdgeP2);
        _mVertices.Add(aFirstEdgeP2 + aSecondRelative);
        _mVertices.Add(aFirstEdgeP1 + aSecondRelative);
        _mNormals.Add(aNormal);
        _mNormals.Add(aNormal);
        _mNormals.Add(aNormal);
        _mNormals.Add(aNormal);
    }

    private void AddEdge(int aX, int aY, Edge aEdge)
    {
        var size = new Vector2(1.0f / _mWidth, 1.0f / _mHeight);
        Vector2 uv = new Vector3(aX * size.x, aY * size.y);
        var p = uv - Vector2.one * 0.5f;
        uv += size * 0.5f;
        var p2 = p;
        Vector3 normal;
        if (aEdge == Edge.Top)
        {
            p += size;
            p2.y += size.y;
            normal = Vector3.up;
        }
        else if (aEdge == Edge.Left)
        {
            p.y += size.y;
            normal = Vector3.left;
        }
        else if (aEdge == Edge.Bottom)
        {
            p2.x += size.x;
            normal = Vector3.down;
        }
        else
        {
            p2 += size;
            p.x += size.x;
            normal = Vector3.right;
        }

        AddQuad(p, p2, Vector3.forward * depth, normal, uv, uv, false);
    }

    private void GenerateMesh()
    {
        while (_called > 0)
        {
            _mVertices.Clear();
            _mNormals.Clear();

            _mWidth = texture.width;
            _mHeight = texture.height;

            for (var y = 0; y < _mHeight; y++) // bottom to top
            for (var x = 0; x < _mWidth; x++) // left to right
                if (HasPixel(x, y))
                {
                    if (x == 0 || !HasPixel(x - 1, y))
                        AddEdge(x, y, Edge.Left);

                    if (x == _mWidth - 1 || !HasPixel(x + 1, y))
                        AddEdge(x, y, Edge.Right);

                    if (y == 0 || !HasPixel(x, y - 1))
                        AddEdge(x, y, Edge.Bottom);

                    if (y == _mHeight - 1 || !HasPixel(x, y + 1))
                        AddEdge(x, y, Edge.Top);
                }

            var mesh = new Mesh();

            if (_mVertices.Count < 65000)
            {
                mesh.vertices = _mVertices.ToArray();
                mesh.normals = _mNormals.ToArray();

                var quads = new int[_mVertices.Count];
                for (var i = 0; i < quads.Length; i++)
                    quads[i] = i;
                mesh.SetIndices(quads, MeshTopology.Quads, 0);
                mesh.SetTriangles(mesh.GetTriangles(0), 0);

                mc.sharedMesh = mesh;
                mc.tag = "Terrain";
            }
            else
            {
                print("TOO MANY VERTS");
            }

            if (_mVertices.Count < 3)
                sprite.enabled = false;
            else
                sprite.enabled = true;
            _called--;
        }
    }

    private enum Edge
    {
        Top,
        Left,
        Bottom,
        Right
    }
}
}