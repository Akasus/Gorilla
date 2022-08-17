using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace TextMesh_Pro.Scripts
{
    public class TmpTextEventHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private Camera _mCamera;
        private Canvas _mCanvas;
        private int _mLastCharIndex = -1;
        private int _mLastLineIndex = -1;
        private int _mLastWordIndex = -1;

        [SerializeField] private CharacterSelectionEvent mOnCharacterSelection = new CharacterSelectionEvent();

        [SerializeField] private LineSelectionEvent mOnLineSelection = new LineSelectionEvent();

        [SerializeField] private LinkSelectionEvent mOnLinkSelection = new LinkSelectionEvent();

        [SerializeField] private SpriteSelectionEvent mOnSpriteSelection = new SpriteSelectionEvent();

        [SerializeField] private WordSelectionEvent mOnWordSelection = new WordSelectionEvent();

        private int _mSelectedLink = -1;


        private TMP_Text _mTextComponent;


        /// <summary>
        ///     Event delegate triggered when pointer is over a character.
        /// </summary>
        public CharacterSelectionEvent onCharacterSelection
        {
            get => mOnCharacterSelection;
            set => mOnCharacterSelection = value;
        }


        /// <summary>
        ///     Event delegate triggered when pointer is over a sprite.
        /// </summary>
        public SpriteSelectionEvent onSpriteSelection
        {
            get => mOnSpriteSelection;
            set => mOnSpriteSelection = value;
        }


        /// <summary>
        ///     Event delegate triggered when pointer is over a word.
        /// </summary>
        public WordSelectionEvent onWordSelection
        {
            get => mOnWordSelection;
            set => mOnWordSelection = value;
        }


        /// <summary>
        ///     Event delegate triggered when pointer is over a line.
        /// </summary>
        public LineSelectionEvent onLineSelection
        {
            get => mOnLineSelection;
            set => mOnLineSelection = value;
        }


        /// <summary>
        ///     Event delegate triggered when pointer is over a link.
        /// </summary>
        public LinkSelectionEvent onLinkSelection
        {
            get => mOnLinkSelection;
            set => mOnLinkSelection = value;
        }


        public void OnPointerEnter(PointerEventData eventData)
        {
            //Debug.Log("OnPointerEnter()");
        }


        public void OnPointerExit(PointerEventData eventData)
        {
            //Debug.Log("OnPointerExit()");
        }

        private void Awake()
        {
            // Get a reference to the text component.
            _mTextComponent = gameObject.GetComponent<TMP_Text>();

            // Get a reference to the camera rendering the text taking into consideration the text component type.
            if (_mTextComponent.GetType() == typeof(TextMeshProUGUI))
            {
                _mCanvas = gameObject.GetComponentInParent<Canvas>();
                if (_mCanvas != null)
                {
                    if (_mCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
                        _mCamera = null;
                    else
                        _mCamera = _mCanvas.worldCamera;
                }
            }
            else
            {
                _mCamera = Camera.main;
            }
        }


        private void LateUpdate()
        {
            if (TMP_TextUtilities.IsIntersectingRectTransform(_mTextComponent.rectTransform, Input.mousePosition,
                _mCamera))
            {
                #region Example of Character or Sprite Selection

                var charIndex =
                    TMP_TextUtilities.FindIntersectingCharacter(_mTextComponent, Input.mousePosition, _mCamera, true);
                if (charIndex != -1 && charIndex != _mLastCharIndex)
                {
                    _mLastCharIndex = charIndex;

                    var elementType = _mTextComponent.textInfo.characterInfo[charIndex].elementType;

                    // Send event to any event listeners depending on whether it is a character or sprite.
                    if (elementType == TMP_TextElementType.Character)
                        SendOnCharacterSelection(_mTextComponent.textInfo.characterInfo[charIndex].character,
                            charIndex);
                    else if (elementType == TMP_TextElementType.Sprite)
                        SendOnSpriteSelection(_mTextComponent.textInfo.characterInfo[charIndex].character, charIndex);
                }

                #endregion


                #region Example of Word Selection

                // Check if Mouse intersects any words and if so assign a random color to that word.
                var wordIndex = TMP_TextUtilities.FindIntersectingWord(_mTextComponent, Input.mousePosition, _mCamera);
                if (wordIndex != -1 && wordIndex != _mLastWordIndex)
                {
                    _mLastWordIndex = wordIndex;

                    // Get the information about the selected word.
                    var wInfo = _mTextComponent.textInfo.wordInfo[wordIndex];

                    // Send the event to any listeners.
                    SendOnWordSelection(wInfo.GetWord(), wInfo.firstCharacterIndex, wInfo.characterCount);
                }

                #endregion


                #region Example of Line Selection

                // Check if Mouse intersects any words and if so assign a random color to that word.
                var lineIndex = TMP_TextUtilities.FindIntersectingLine(_mTextComponent, Input.mousePosition, _mCamera);
                if (lineIndex != -1 && lineIndex != _mLastLineIndex)
                {
                    _mLastLineIndex = lineIndex;

                    // Get the information about the selected word.
                    var lineInfo = _mTextComponent.textInfo.lineInfo[lineIndex];

                    // Send the event to any listeners.
                    var buffer = new char[lineInfo.characterCount];
                    for (var i = 0;
                        i < lineInfo.characterCount && i < _mTextComponent.textInfo.characterInfo.Length;
                        i++)
                        buffer[i] = _mTextComponent.textInfo.characterInfo[i + lineInfo.firstCharacterIndex].character;

                    var lineText = new string(buffer);
                    SendOnLineSelection(lineText, lineInfo.firstCharacterIndex, lineInfo.characterCount);
                }

                #endregion


                #region Example of Link Handling

                // Check if mouse intersects with any links.
                var linkIndex = TMP_TextUtilities.FindIntersectingLink(_mTextComponent, Input.mousePosition, _mCamera);

                // Handle new Link selection.
                if (linkIndex != -1 && linkIndex != _mSelectedLink)
                {
                    _mSelectedLink = linkIndex;

                    // Get information about the link.
                    var linkInfo = _mTextComponent.textInfo.linkInfo[linkIndex];

                    // Send the event to any listeners. 
                    SendOnLinkSelection(linkInfo.GetLinkID(), linkInfo.GetLinkText(), linkIndex);
                }

                #endregion
            }
        }


        private void SendOnCharacterSelection(char character, int characterIndex)
        {
            if (onCharacterSelection != null)
                onCharacterSelection.Invoke(character, characterIndex);
        }

        private void SendOnSpriteSelection(char character, int characterIndex)
        {
            if (onSpriteSelection != null)
                onSpriteSelection.Invoke(character, characterIndex);
        }

        private void SendOnWordSelection(string word, int charIndex, int length)
        {
            if (onWordSelection != null)
                onWordSelection.Invoke(word, charIndex, length);
        }

        private void SendOnLineSelection(string line, int charIndex, int length)
        {
            if (onLineSelection != null)
                onLineSelection.Invoke(line, charIndex, length);
        }

        private void SendOnLinkSelection(string linkId, string linkText, int linkIndex)
        {
            if (onLinkSelection != null)
                onLinkSelection.Invoke(linkId, linkText, linkIndex);
        }

        [Serializable]
        public class CharacterSelectionEvent : UnityEvent<char, int>
        {
        }

        [Serializable]
        public class SpriteSelectionEvent : UnityEvent<char, int>
        {
        }

        [Serializable]
        public class WordSelectionEvent : UnityEvent<string, int, int>
        {
        }

        [Serializable]
        public class LineSelectionEvent : UnityEvent<string, int, int>
        {
        }

        [Serializable]
        public class LinkSelectionEvent : UnityEvent<string, string, int>
        {
        }
    }
}