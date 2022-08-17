﻿using TMPro;
using UnityEngine;

namespace TextMesh_Pro.Scripts
{
    public class TmpExampleScript01 : MonoBehaviour
    {
        public enum ObjectType
        {
            TextMeshPro = 0,
            TextMeshProUgui = 1
        }

        //private TMP_InputField m_inputfield;


        private const string Label = "The count is <#0080ff>{0}</color>";
        private int _count;
        public bool isStatic;

        private TMP_Text _mText;

        public ObjectType objectType;

        private void Awake()
        {
            // Get a reference to the TMP text component if one already exists otherwise add one.
            // This example show the convenience of having both TMP components derive from TMP_Text. 
            if (objectType == 0)
                _mText = GetComponent<TextMeshPro>() ?? gameObject.AddComponent<TextMeshPro>();
            else
                _mText = GetComponent<TextMeshProUGUI>() ?? gameObject.AddComponent<TextMeshProUGUI>();

            // Load a new font asset and assign it to the text object.
            _mText.font = Resources.Load<TMP_FontAsset>("Fonts & Materials/Anton SDF");

            // Load a new material preset which was created with the context menu duplicate.
            _mText.fontSharedMaterial = Resources.Load<Material>("Fonts & Materials/Anton SDF - Drop Shadow");

            // Set the size of the font.
            _mText.fontSize = 120;

            // Set the text
            _mText.text = "A <#0080ff>simple</color> line of text.";

            // Get the preferred width and height based on the supplied width and height as opposed to the actual size of the current text container.
            var size = _mText.GetPreferredValues(Mathf.Infinity, Mathf.Infinity);

            // Set the size of the RectTransform based on the new calculated values.
            _mText.rectTransform.sizeDelta = new Vector2(size.x, size.y);
        }


        private void Update()
        {
            if (!isStatic)
            {
                _mText.SetText(Label, _count % 1000);
                _count += 1;
            }
        }
    }
}