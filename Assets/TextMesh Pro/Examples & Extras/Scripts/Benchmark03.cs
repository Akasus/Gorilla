using TMPro;
using UnityEngine;

namespace TextMesh_Pro.Scripts
{
    public class Benchmark03 : MonoBehaviour
    {
        public int numberOfNpc = 12;

        public int spawnType;

        public Font theFont;

        //private TextMeshProFloatingText floatingText_Script;

        private void Awake()
        {
        }


        private void Start()
        {
            for (var i = 0; i < numberOfNpc; i++)
                if (spawnType == 0)
                {
                    // TextMesh Pro Implementation
                    //go.transform.localScale = new Vector3(2, 2, 2);
                    var go = new GameObject(); //"NPC " + i);
                    //go.transform.position = new Vector3(Random.Range(-95f, 95f), 0.5f, Random.Range(-95f, 95f));

                    go.transform.position = new Vector3(0, 0, 0);
                    //go.renderer.castShadows = false;
                    //go.renderer.receiveShadows = false;
                    //go.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

                    var textMeshPro = go.AddComponent<TextMeshPro>();
                    //textMeshPro.FontAsset = Resources.Load("Fonts & Materials/LiberationSans SDF", typeof(TextMeshProFont)) as TextMeshProFont;
                    textMeshPro.alignment = TextAlignmentOptions.Center;
                    textMeshPro.fontSize = 96;

                    textMeshPro.text = "@";
                    textMeshPro.color = new Color32(255, 255, 0, 255);
                    //textMeshPro.Text = "!";


                    // Spawn Floating Text
                    //floatingText_Script = go.AddComponent<TextMeshProFloatingText>();
                    //floatingText_Script.SpawnType = 0;
                }
                else
                {
                    // TextMesh Implementation
                    var go = new GameObject(); //"NPC " + i);
                    //go.transform.position = new Vector3(Random.Range(-95f, 95f), 0.5f, Random.Range(-95f, 95f));

                    go.transform.position = new Vector3(0, 0, 0);

                    var textMesh = go.AddComponent<TextMesh>();
                    textMesh.GetComponent<Renderer>().sharedMaterial = theFont.material;
                    textMesh.font = theFont;
                    textMesh.anchor = TextAnchor.MiddleCenter;
                    textMesh.fontSize = 96;

                    textMesh.color = new Color32(255, 255, 0, 255);
                    textMesh.text = "@";

                    // Spawn Floating Text
                    //floatingText_Script = go.AddComponent<TextMeshProFloatingText>();
                    //floatingText_Script.SpawnType = 1;
                }
        }
    }
}