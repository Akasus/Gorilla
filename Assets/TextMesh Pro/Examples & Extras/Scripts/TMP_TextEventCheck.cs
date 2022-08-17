using UnityEngine;

namespace TextMesh_Pro.Scripts
{
    public class TmpTextEventCheck : MonoBehaviour
    {
        public TmpTextEventHandler textEventHandler;

        private void OnEnable()
        {
            if (textEventHandler != null)
            {
                textEventHandler.onCharacterSelection.AddListener(OnCharacterSelection);
                textEventHandler.onSpriteSelection.AddListener(OnSpriteSelection);
                textEventHandler.onWordSelection.AddListener(OnWordSelection);
                textEventHandler.onLineSelection.AddListener(OnLineSelection);
                textEventHandler.onLinkSelection.AddListener(OnLinkSelection);
            }
        }


        private void OnDisable()
        {
            if (textEventHandler != null)
            {
                textEventHandler.onCharacterSelection.RemoveListener(OnCharacterSelection);
                textEventHandler.onSpriteSelection.RemoveListener(OnSpriteSelection);
                textEventHandler.onWordSelection.RemoveListener(OnWordSelection);
                textEventHandler.onLineSelection.RemoveListener(OnLineSelection);
                textEventHandler.onLinkSelection.RemoveListener(OnLinkSelection);
            }
        }


        private void OnCharacterSelection(char c, int index)
        {
            Debug.Log("Character [" + c + "] at Index: " + index + " has been selected.");
        }

        private void OnSpriteSelection(char c, int index)
        {
            Debug.Log("Sprite [" + c + "] at Index: " + index + " has been selected.");
        }

        private void OnWordSelection(string word, int firstCharacterIndex, int length)
        {
            Debug.Log("Word [" + word + "] with first character index of " + firstCharacterIndex + " and length of " +
                      length + " has been selected.");
        }

        private void OnLineSelection(string lineText, int firstCharacterIndex, int length)
        {
            Debug.Log("Line [" + lineText + "] with first character index of " + firstCharacterIndex +
                      " and length of " + length + " has been selected.");
        }

        private void OnLinkSelection(string linkId, string linkText, int linkIndex)
        {
            Debug.Log("Link Index: " + linkIndex + " with ID [" + linkId + "] and Text \"" + linkText +
                      "\" has been selected.");
        }
    }
}