using UnityEngine;

namespace Terrain
{
    public class Terrain2DMain : MonoBehaviour
    {
        public static Terrain2DMain Main;

        [SerializeField] private LayerMask ly;
        [SerializeField] private float pixelsPerUnit = 10;
        [SerializeField] private Vector3 scale;
        [SerializeField] private Vector2 terrainParts;
        [SerializeField] private GameObject terrainPrefab;
        [SerializeField] private Terrain2D[,] terrains;
        [SerializeField] private Texture2D texture;

        private void Start()
        {
            Main = this;
            SplitTerrain();
        }


        public void Dig(Vector3 pos, int radius)
        {
            if (radius == 1)
            {
                pos.z = -10;
                RaycastHit hit;
                Physics.Raycast(pos, transform.forward, out hit, 1000, ly);
                if (hit.transform == null)
                    return;

                var pixelUv = hit.textureCoord;

                var t = hit.transform.parent.parent.gameObject.GetComponent<Terrain2D>();
                if (t != null)
                {
                    var size = new Vector2(t.texture.width, t.texture.height);
                    t.texture.SetPixel((int) (pixelUv.x * size.x), (int) (pixelUv.y * size.y), Color.clear);
                    float i = (int) (pixelUv.y * size.y) * t.texture.width + (int) (pixelUv.x * size.x);
                    t.mColors[(int) i] = Color.clear;
                    t.texture.Apply();
                    t.GenCollider();
                }
            }
            else
            {
                pos.z = -10;
                RaycastHit hit;
                Physics.Raycast(pos, transform.forward, out hit, 1000, ly);
                if (hit.transform == null)
                    return;

                if (radius % 2 > 0)
                {
                    Debug.Log("Radius not dividable");
                    return;
                }

                var pixelUv = hit.textureCoord;
                var t = hit.transform.parent.parent.gameObject.GetComponent<Terrain2D>();

                if (t != null)
                {
                    var size = new Vector2(t.texture.width, t.texture.height);

                    float w = t.texture.width;
                    float h = t.texture.height;
                    var l = t.texture.width * t.texture.height;

                    var r = radius / 2;
                    Vector2 center = pos;
                    for (var y = -r; y < r; y++)
                    for (var x = -r; x < r; x++)
                    {
                        var yC = (int) (pixelUv.y * size.y) + y;
                        var xC = (int) (pixelUv.x * size.x) + x;

                        var p = new Vector2(x, y);

                        if (Vector2.Distance(p, center) >= -r && Vector2.Distance(p, center) <= r)
                            if (yC >= 0 && xC >= 0 && xC <= w && yC <= h && yC * w + xC < l)
                            {
                                t.texture.SetPixel(xC, yC, Color.clear);
                                var i = yC * w + xC;
                                t.mColors[(int) i] = Color.clear;
                            }
                    }

                    t.texture.Apply();
                    t.GenCollider();
                }
            }
        }

        private void SplitTerrain()
        {
            if (terrainParts.magnitude == 0)
            {
                var dividers = "";
                for (var i = 1; i < texture.height; i++)
                    if (texture.height % i == 0)
                    {
                        dividers += i.ToString();
                        dividers += ",";
                    }

                print("Dividers " + dividers);
            }

            if (texture.width % terrainParts.x + texture.height % terrainParts.y > 0)
            {
                Debug.LogError("Resolution not dividable by " + terrainParts);
                Debug.Break();
                return;
            }

            terrains = new Terrain2D[(int) terrainParts.x, (int) terrainParts.y];
            for (var x = 0; x < terrainParts.x; x++)
            for (var y = 0; y < terrainParts.y; y++)
            {
                var go = Instantiate(terrainPrefab, Vector3.zero, Quaternion.identity);
                go.name = x + " , " + y;
                go.transform.parent = transform;
                go.transform.localPosition = new Vector3(x * texture.width / terrainParts.x / pixelsPerUnit,
                    y * texture.height / terrainParts.y / pixelsPerUnit);
                go.transform.localScale = scale;
                var t2d = go.GetComponent<Terrain2D>();
                t2d.terrain = this;
                var tex = new Texture2D((int) (texture.width / terrainParts.x),
                    (int) (texture.height / terrainParts.y));
                tex.filterMode = FilterMode.Point;
                var c = texture.GetPixels((int) (x * texture.width / terrainParts.x),
                    (int) (y * texture.height / terrainParts.y), (int) (texture.width / terrainParts.x),
                    (int) (texture.height / terrainParts.y));
                tex.SetPixels(c);
                tex.wrapMode = TextureWrapMode.Clamp;
                tex.Apply();
                var s = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f),
                    pixelsPerUnit);

                terrains[x, y] = t2d;

                t2d.sprite.sprite = s;
                t2d.texture = tex;

                t2d.bc2d = go.AddComponent<BoxCollider2D>();
                t2d.backCollider.localScale = new Vector3(t2d.bc2d.size.x, t2d.bc2d.size.y, 1);
                Destroy(t2d.bc2d);
            }
        }
    }
}