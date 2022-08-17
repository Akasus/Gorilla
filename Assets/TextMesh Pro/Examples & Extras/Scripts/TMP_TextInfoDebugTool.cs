using TMPro;
using UnityEditor;
using UnityEngine;

namespace TextMesh_Pro.Scripts
{
    public class TmpTextInfoDebugTool : MonoBehaviour
    {
        // Since this script is used for debugging, we exclude it from builds.
        // TODO: Rework this script to make it into an editor utility.
#if UNITY_EDITOR
        public bool showCharacters;
        public bool showWords;
        public bool showLinks;
        public bool showLines;
        public bool showMeshBounds;
        public bool showTextBounds;

        [Space(10)] [TextArea(2, 2)] public string objectStats;

        [SerializeField] private TMP_Text mTextComponent;

        [SerializeField] private Transform mTransform;


        private void OnDrawGizmos()
        {
            if (mTextComponent == null)
            {
                mTextComponent = gameObject.GetComponent<TMP_Text>();

                if (mTextComponent == null)
                    return;

                if (mTransform == null)
                    mTransform = gameObject.GetComponent<Transform>();
            }

            // Update Text Statistics
            var textInfo = mTextComponent.textInfo;

            objectStats = "Characters: " + textInfo.characterCount + "   Words: " + textInfo.wordCount + "   Spaces: " +
                          textInfo.spaceCount + "   Sprites: " + textInfo.spriteCount + "   Links: " +
                          textInfo.linkCount
                          + "\nLines: " + textInfo.lineCount + "   Pages: " + textInfo.pageCount;


            // Draw Quads around each of the Characters

            #region Draw Characters

            if (showCharacters)
                DrawCharactersBounds();

            #endregion


            // Draw Quads around each of the words

            #region Draw Words

            if (showWords)
                DrawWordBounds();

            #endregion


            // Draw Quads around each of the words

            #region Draw Links

            if (showLinks)
                DrawLinkBounds();

            #endregion


            // Draw Quads around each line

            #region Draw Lines

            if (showLines)
                DrawLineBounds();

            #endregion


            // Draw Quad around the bounds of the text

            #region Draw Bounds

            if (showMeshBounds)
                DrawBounds();

            #endregion

            // Draw Quad around the rendered region of the text.

            #region Draw Text Bounds

            if (showTextBounds)
                DrawTextBounds();

            #endregion
        }


        /// <summary>
        ///     Method to draw a rectangle around each character.
        /// </summary>
        /// <param name="text"></param>
        private void DrawCharactersBounds()
        {
            var textInfo = mTextComponent.textInfo;

            for (var i = 0; i < textInfo.characterCount; i++)
            {
                // Draw visible as well as invisible characters
                var cInfo = textInfo.characterInfo[i];

                var isCharacterVisible = i >= mTextComponent.maxVisibleCharacters ||
                                         cInfo.lineNumber >= mTextComponent.maxVisibleLines ||
                                         mTextComponent.overflowMode == TextOverflowModes.Page &&
                                         cInfo.pageNumber + 1 != mTextComponent.pageToDisplay
                    ? false
                    : true;

                if (!isCharacterVisible) continue;

                // Get Bottom Left and Top Right position of the current character
                var bottomLeft = mTransform.TransformPoint(cInfo.bottomLeft);
                var topLeft = mTransform.TransformPoint(new Vector3(cInfo.topLeft.x, cInfo.topLeft.y, 0));
                var topRight = mTransform.TransformPoint(cInfo.topRight);
                var bottomRight = mTransform.TransformPoint(new Vector3(cInfo.bottomRight.x, cInfo.bottomRight.y, 0));

                var color = cInfo.isVisible ? Color.yellow : Color.grey;
                DrawRectangle(bottomLeft, topLeft, topRight, bottomRight, color);

                // Baseline
                var baselineStart = new Vector3(topLeft.x,
                    mTransform.TransformPoint(new Vector3(0, cInfo.baseLine, 0)).y, 0);
                var baselineEnd = new Vector3(topRight.x,
                    mTransform.TransformPoint(new Vector3(0, cInfo.baseLine, 0)).y, 0);

                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(baselineStart, baselineEnd);


                // Draw Ascender & Descender for each character.
                var ascenderStart = new Vector3(topLeft.x,
                    mTransform.TransformPoint(new Vector3(0, cInfo.ascender, 0)).y, 0);
                var ascenderEnd = new Vector3(topRight.x,
                    mTransform.TransformPoint(new Vector3(0, cInfo.ascender, 0)).y, 0);
                var descenderStart = new Vector3(bottomLeft.x,
                    mTransform.TransformPoint(new Vector3(0, cInfo.descender, 0)).y, 0);
                var descenderEnd = new Vector3(bottomRight.x,
                    mTransform.TransformPoint(new Vector3(0, cInfo.descender, 0)).y, 0);

                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(ascenderStart, ascenderEnd);
                Gizmos.DrawLine(descenderStart, descenderEnd);

                // Draw Cap Height
                var capHeight = cInfo.baseLine + cInfo.fontAsset.faceInfo.capLine * cInfo.scale;
                var capHeightStart =
                    new Vector3(topLeft.x, mTransform.TransformPoint(new Vector3(0, capHeight, 0)).y, 0);
                var capHeightEnd = new Vector3(topRight.x, mTransform.TransformPoint(new Vector3(0, capHeight, 0)).y,
                    0);

                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(capHeightStart, capHeightEnd);

                // Draw Centerline
                var meanline = cInfo.baseLine + cInfo.fontAsset.faceInfo.meanLine * cInfo.scale;
                var centerlineStart =
                    new Vector3(topLeft.x, mTransform.TransformPoint(new Vector3(0, meanline, 0)).y, 0);
                var centerlineEnd =
                    new Vector3(topRight.x, mTransform.TransformPoint(new Vector3(0, meanline, 0)).y, 0);

                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(centerlineStart, centerlineEnd);

                // Draw Origin for each character.
                var gizmoSize = (ascenderEnd.y - descenderEnd.y) * 0.02f;
                var origin = mTransform.TransformPoint(cInfo.origin, cInfo.baseLine, 0);
                var originBl = new Vector3(origin.x - gizmoSize, origin.y - gizmoSize, 0);
                var originTl = new Vector3(originBl.x, origin.y + gizmoSize, 0);
                var originTr = new Vector3(origin.x + gizmoSize, originTl.y, 0);
                var originBr = new Vector3(originTr.x, originBl.y, 0);

                Gizmos.color = new Color(1, 0.5f, 0);
                Gizmos.DrawLine(originBl, originTl);
                Gizmos.DrawLine(originTl, originTr);
                Gizmos.DrawLine(originTr, originBr);
                Gizmos.DrawLine(originBr, originBl);

                // Draw xAdvance for each character.
                gizmoSize = (ascenderEnd.y - descenderEnd.y) * 0.04f;
                var xAdvance = mTransform.TransformPoint(cInfo.xAdvance, 0, 0).x;
                var topAdvance = new Vector3(xAdvance, baselineStart.y + gizmoSize, 0);
                var bottomAdvance = new Vector3(xAdvance, baselineStart.y - gizmoSize, 0);
                var leftAdvance = new Vector3(xAdvance - gizmoSize, baselineStart.y, 0);
                var rightAdvance = new Vector3(xAdvance + gizmoSize, baselineStart.y, 0);

                Gizmos.color = Color.green;
                Gizmos.DrawLine(topAdvance, bottomAdvance);
                Gizmos.DrawLine(leftAdvance, rightAdvance);
            }
        }


        /// <summary>
        ///     Method to draw rectangles around each word of the text.
        /// </summary>
        /// <param name="text"></param>
        private void DrawWordBounds()
        {
            var textInfo = mTextComponent.textInfo;

            for (var i = 0; i < textInfo.wordCount; i++)
            {
                var wInfo = textInfo.wordInfo[i];

                var isBeginRegion = false;

                var bottomLeft = Vector3.zero;
                var topLeft = Vector3.zero;
                var bottomRight = Vector3.zero;
                var topRight = Vector3.zero;

                var maxAscender = -Mathf.Infinity;
                var minDescender = Mathf.Infinity;

                var wordColor = Color.green;

                // Iterate through each character of the word
                for (var j = 0; j < wInfo.characterCount; j++)
                {
                    var characterIndex = wInfo.firstCharacterIndex + j;
                    var currentCharInfo = textInfo.characterInfo[characterIndex];
                    var currentLine = currentCharInfo.lineNumber;

                    var isCharacterVisible = characterIndex > mTextComponent.maxVisibleCharacters ||
                                             currentCharInfo.lineNumber > mTextComponent.maxVisibleLines ||
                                             mTextComponent.overflowMode == TextOverflowModes.Page &&
                                             currentCharInfo.pageNumber + 1 != mTextComponent.pageToDisplay
                        ? false
                        : true;

                    // Track Max Ascender and Min Descender
                    maxAscender = Mathf.Max(maxAscender, currentCharInfo.ascender);
                    minDescender = Mathf.Min(minDescender, currentCharInfo.descender);

                    if (isBeginRegion == false && isCharacterVisible)
                    {
                        isBeginRegion = true;

                        bottomLeft = new Vector3(currentCharInfo.bottomLeft.x, currentCharInfo.descender, 0);
                        topLeft = new Vector3(currentCharInfo.bottomLeft.x, currentCharInfo.ascender, 0);

                        //Debug.Log("Start Word Region at [" + currentCharInfo.character + "]");

                        // If Word is one character
                        if (wInfo.characterCount == 1)
                        {
                            isBeginRegion = false;

                            topLeft = mTransform.TransformPoint(new Vector3(topLeft.x, maxAscender, 0));
                            bottomLeft = mTransform.TransformPoint(new Vector3(bottomLeft.x, minDescender, 0));
                            bottomRight =
                                mTransform.TransformPoint(new Vector3(currentCharInfo.topRight.x, minDescender, 0));
                            topRight = mTransform.TransformPoint(new Vector3(currentCharInfo.topRight.x, maxAscender,
                                0));

                            // Draw Region
                            DrawRectangle(bottomLeft, topLeft, topRight, bottomRight, wordColor);

                            //Debug.Log("End Word Region at [" + currentCharInfo.character + "]");
                        }
                    }

                    // Last Character of Word
                    if (isBeginRegion && j == wInfo.characterCount - 1)
                    {
                        isBeginRegion = false;

                        topLeft = mTransform.TransformPoint(new Vector3(topLeft.x, maxAscender, 0));
                        bottomLeft = mTransform.TransformPoint(new Vector3(bottomLeft.x, minDescender, 0));
                        bottomRight =
                            mTransform.TransformPoint(new Vector3(currentCharInfo.topRight.x, minDescender, 0));
                        topRight = mTransform.TransformPoint(new Vector3(currentCharInfo.topRight.x, maxAscender, 0));

                        // Draw Region
                        DrawRectangle(bottomLeft, topLeft, topRight, bottomRight, wordColor);

                        //Debug.Log("End Word Region at [" + currentCharInfo.character + "]");
                    }
                    // If Word is split on more than one line.
                    else if (isBeginRegion && currentLine != textInfo.characterInfo[characterIndex + 1].lineNumber)
                    {
                        isBeginRegion = false;

                        topLeft = mTransform.TransformPoint(new Vector3(topLeft.x, maxAscender, 0));
                        bottomLeft = mTransform.TransformPoint(new Vector3(bottomLeft.x, minDescender, 0));
                        bottomRight =
                            mTransform.TransformPoint(new Vector3(currentCharInfo.topRight.x, minDescender, 0));
                        topRight = mTransform.TransformPoint(new Vector3(currentCharInfo.topRight.x, maxAscender, 0));

                        // Draw Region
                        DrawRectangle(bottomLeft, topLeft, topRight, bottomRight, wordColor);
                        //Debug.Log("End Word Region at [" + currentCharInfo.character + "]");
                        maxAscender = -Mathf.Infinity;
                        minDescender = Mathf.Infinity;
                    }
                }

                //Debug.Log(wInfo.GetWord(m_TextMeshPro.textInfo.characterInfo));
            }
        }


        /// <summary>
        ///     Draw rectangle around each of the links contained in the text.
        /// </summary>
        /// <param name="text"></param>
        private void DrawLinkBounds()
        {
            var textInfo = mTextComponent.textInfo;

            for (var i = 0; i < textInfo.linkCount; i++)
            {
                var linkInfo = textInfo.linkInfo[i];

                var isBeginRegion = false;

                var bottomLeft = Vector3.zero;
                var topLeft = Vector3.zero;
                var bottomRight = Vector3.zero;
                var topRight = Vector3.zero;

                var maxAscender = -Mathf.Infinity;
                var minDescender = Mathf.Infinity;

                Color32 linkColor = Color.cyan;

                // Iterate through each character of the link text
                for (var j = 0; j < linkInfo.linkTextLength; j++)
                {
                    var characterIndex = linkInfo.linkTextfirstCharacterIndex + j;
                    var currentCharInfo = textInfo.characterInfo[characterIndex];
                    var currentLine = currentCharInfo.lineNumber;

                    var isCharacterVisible = characterIndex > mTextComponent.maxVisibleCharacters ||
                                             currentCharInfo.lineNumber > mTextComponent.maxVisibleLines ||
                                             mTextComponent.overflowMode == TextOverflowModes.Page &&
                                             currentCharInfo.pageNumber + 1 != mTextComponent.pageToDisplay
                        ? false
                        : true;

                    // Track Max Ascender and Min Descender
                    maxAscender = Mathf.Max(maxAscender, currentCharInfo.ascender);
                    minDescender = Mathf.Min(minDescender, currentCharInfo.descender);

                    if (isBeginRegion == false && isCharacterVisible)
                    {
                        isBeginRegion = true;

                        bottomLeft = new Vector3(currentCharInfo.bottomLeft.x, currentCharInfo.descender, 0);
                        topLeft = new Vector3(currentCharInfo.bottomLeft.x, currentCharInfo.ascender, 0);

                        //Debug.Log("Start Word Region at [" + currentCharInfo.character + "]");

                        // If Link is one character
                        if (linkInfo.linkTextLength == 1)
                        {
                            isBeginRegion = false;

                            topLeft = mTransform.TransformPoint(new Vector3(topLeft.x, maxAscender, 0));
                            bottomLeft = mTransform.TransformPoint(new Vector3(bottomLeft.x, minDescender, 0));
                            bottomRight =
                                mTransform.TransformPoint(new Vector3(currentCharInfo.topRight.x, minDescender, 0));
                            topRight = mTransform.TransformPoint(new Vector3(currentCharInfo.topRight.x, maxAscender,
                                0));

                            // Draw Region
                            DrawRectangle(bottomLeft, topLeft, topRight, bottomRight, linkColor);

                            //Debug.Log("End Word Region at [" + currentCharInfo.character + "]");
                        }
                    }

                    // Last Character of Link
                    if (isBeginRegion && j == linkInfo.linkTextLength - 1)
                    {
                        isBeginRegion = false;

                        topLeft = mTransform.TransformPoint(new Vector3(topLeft.x, maxAscender, 0));
                        bottomLeft = mTransform.TransformPoint(new Vector3(bottomLeft.x, minDescender, 0));
                        bottomRight =
                            mTransform.TransformPoint(new Vector3(currentCharInfo.topRight.x, minDescender, 0));
                        topRight = mTransform.TransformPoint(new Vector3(currentCharInfo.topRight.x, maxAscender, 0));

                        // Draw Region
                        DrawRectangle(bottomLeft, topLeft, topRight, bottomRight, linkColor);

                        //Debug.Log("End Word Region at [" + currentCharInfo.character + "]");
                    }
                    // If Link is split on more than one line.
                    else if (isBeginRegion && currentLine != textInfo.characterInfo[characterIndex + 1].lineNumber)
                    {
                        isBeginRegion = false;

                        topLeft = mTransform.TransformPoint(new Vector3(topLeft.x, maxAscender, 0));
                        bottomLeft = mTransform.TransformPoint(new Vector3(bottomLeft.x, minDescender, 0));
                        bottomRight =
                            mTransform.TransformPoint(new Vector3(currentCharInfo.topRight.x, minDescender, 0));
                        topRight = mTransform.TransformPoint(new Vector3(currentCharInfo.topRight.x, maxAscender, 0));

                        // Draw Region
                        DrawRectangle(bottomLeft, topLeft, topRight, bottomRight, linkColor);

                        maxAscender = -Mathf.Infinity;
                        minDescender = Mathf.Infinity;
                        //Debug.Log("End Word Region at [" + currentCharInfo.character + "]");
                    }
                }

                //Debug.Log(wInfo.GetWord(m_TextMeshPro.textInfo.characterInfo));
            }
        }


        /// <summary>
        ///     Draw Rectangles around each lines of the text.
        /// </summary>
        /// <param name="text"></param>
        private void DrawLineBounds()
        {
            var textInfo = mTextComponent.textInfo;

            for (var i = 0; i < textInfo.lineCount; i++)
            {
                var lineInfo = textInfo.lineInfo[i];

                var isLineVisible = lineInfo.characterCount == 1 &&
                                    textInfo.characterInfo[lineInfo.firstCharacterIndex].character == 10 ||
                                    i > mTextComponent.maxVisibleLines ||
                                    mTextComponent.overflowMode == TextOverflowModes.Page &&
                                    textInfo.characterInfo[lineInfo.firstCharacterIndex].pageNumber + 1 !=
                                    mTextComponent.pageToDisplay
                    ? false
                    : true;

                if (!isLineVisible) continue;

                //if (!ShowLinesOnlyVisibleCharacters)
                //{
                // Get Bottom Left and Top Right position of each line
                var ascender = lineInfo.ascender;
                var descender = lineInfo.descender;
                var baseline = lineInfo.baseline;
                var maxAdvance = lineInfo.maxAdvance;
                var bottomLeft = mTransform.TransformPoint(
                    new Vector3(textInfo.characterInfo[lineInfo.firstCharacterIndex].bottomLeft.x, descender, 0));
                var topLeft = mTransform.TransformPoint(
                    new Vector3(textInfo.characterInfo[lineInfo.firstCharacterIndex].bottomLeft.x, ascender, 0));
                var topRight =
                    mTransform.TransformPoint(
                        new Vector3(textInfo.characterInfo[lineInfo.lastCharacterIndex].topRight.x, ascender, 0));
                var bottomRight = mTransform.TransformPoint(
                    new Vector3(textInfo.characterInfo[lineInfo.lastCharacterIndex].topRight.x, descender, 0));

                DrawRectangle(bottomLeft, topLeft, topRight, bottomRight, Color.green);

                var bottomOrigin =
                    mTransform.TransformPoint(new Vector3(textInfo.characterInfo[lineInfo.firstCharacterIndex].origin,
                        descender, 0));
                var topOrigin =
                    mTransform.TransformPoint(new Vector3(textInfo.characterInfo[lineInfo.firstCharacterIndex].origin,
                        ascender, 0));
                var bottomAdvance = mTransform.TransformPoint(new Vector3(
                    textInfo.characterInfo[lineInfo.firstCharacterIndex].origin + maxAdvance, descender, 0));
                var topAdvance = mTransform.TransformPoint(
                    new Vector3(textInfo.characterInfo[lineInfo.firstCharacterIndex].origin + maxAdvance, ascender, 0));

                DrawDottedRectangle(bottomOrigin, topOrigin, topAdvance, bottomAdvance, Color.green);

                var baselineStart = mTransform.TransformPoint(
                    new Vector3(textInfo.characterInfo[lineInfo.firstCharacterIndex].bottomLeft.x, baseline, 0));
                var baselineEnd =
                    mTransform.TransformPoint(
                        new Vector3(textInfo.characterInfo[lineInfo.lastCharacterIndex].topRight.x, baseline, 0));

                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(baselineStart, baselineEnd);

                // Draw LineExtents
                Gizmos.color = Color.grey;
                Gizmos.DrawLine(mTransform.TransformPoint(lineInfo.lineExtents.min),
                    mTransform.TransformPoint(lineInfo.lineExtents.max));

                //}
                //else
                //{
                //// Get Bottom Left and Top Right position of each line
                //float ascender = lineInfo.ascender;
                //float descender = lineInfo.descender;
                //Vector3 bottomLeft = m_Transform.TransformPoint(new Vector3(textInfo.characterInfo[lineInfo.firstVisibleCharacterIndex].bottomLeft.x, descender, 0));
                //Vector3 topLeft = m_Transform.TransformPoint(new Vector3(textInfo.characterInfo[lineInfo.firstVisibleCharacterIndex].bottomLeft.x, ascender, 0));
                //Vector3 topRight = m_Transform.TransformPoint(new Vector3(textInfo.characterInfo[lineInfo.lastVisibleCharacterIndex].topRight.x, ascender, 0));
                //Vector3 bottomRight = m_Transform.TransformPoint(new Vector3(textInfo.characterInfo[lineInfo.lastVisibleCharacterIndex].topRight.x, descender, 0));

                //DrawRectangle(bottomLeft, topLeft, topRight, bottomRight, Color.green);

                //Vector3 baselineStart = m_Transform.TransformPoint(new Vector3(textInfo.characterInfo[lineInfo.firstVisibleCharacterIndex].bottomLeft.x, textInfo.characterInfo[lineInfo.firstVisibleCharacterIndex].baseLine, 0));
                //Vector3 baselineEnd = m_Transform.TransformPoint(new Vector3(textInfo.characterInfo[lineInfo.lastVisibleCharacterIndex].topRight.x, textInfo.characterInfo[lineInfo.lastVisibleCharacterIndex].baseLine, 0));

                //Gizmos.color = Color.cyan;
                //Gizmos.DrawLine(baselineStart, baselineEnd);
                //}
            }
        }

        /// <summary>
        ///     Draw Rectangle around the bounds of the text object.
        /// </summary>
        private void DrawBounds()
        {
            var meshBounds = mTextComponent.bounds;

            // Get Bottom Left and Top Right position of each word
            var bottomLeft = mTextComponent.transform.position + (meshBounds.center - meshBounds.extents);
            var topRight = mTextComponent.transform.position + (meshBounds.center + meshBounds.extents);

            DrawRectangle(bottomLeft, topRight, new Color(1, 0.5f, 0));
        }


        private void DrawTextBounds()
        {
            var textBounds = mTextComponent.textBounds;

            var bottomLeft = mTextComponent.transform.position + (textBounds.center - textBounds.extents);
            var topRight = mTextComponent.transform.position + (textBounds.center + textBounds.extents);

            DrawRectangle(bottomLeft, topRight, new Color(0f, 0.5f, 0.5f));
        }


        // Draw Rectangles
        private void DrawRectangle(Vector3 bl, Vector3 tr, Color color)
        {
            Gizmos.color = color;

            Gizmos.DrawLine(new Vector3(bl.x, bl.y, 0), new Vector3(bl.x, tr.y, 0));
            Gizmos.DrawLine(new Vector3(bl.x, tr.y, 0), new Vector3(tr.x, tr.y, 0));
            Gizmos.DrawLine(new Vector3(tr.x, tr.y, 0), new Vector3(tr.x, bl.y, 0));
            Gizmos.DrawLine(new Vector3(tr.x, bl.y, 0), new Vector3(bl.x, bl.y, 0));
        }


        // Draw Rectangles
        private void DrawRectangle(Vector3 bl, Vector3 tl, Vector3 tr, Vector3 br, Color color)
        {
            Gizmos.color = color;

            Gizmos.DrawLine(bl, tl);
            Gizmos.DrawLine(tl, tr);
            Gizmos.DrawLine(tr, br);
            Gizmos.DrawLine(br, bl);
        }


        // Draw Rectangles
        private void DrawDottedRectangle(Vector3 bl, Vector3 tl, Vector3 tr, Vector3 br, Color color)
        {
            var cam = Camera.current;
            var dotSpacing = (cam.WorldToScreenPoint(br).x - cam.WorldToScreenPoint(bl).x) / 75f;
            Handles.color = color;

            Handles.DrawDottedLine(bl, tl, dotSpacing);
            Handles.DrawDottedLine(tl, tr, dotSpacing);
            Handles.DrawDottedLine(tr, br, dotSpacing);
            Handles.DrawDottedLine(br, bl, dotSpacing);
        }

#endif
    }
}