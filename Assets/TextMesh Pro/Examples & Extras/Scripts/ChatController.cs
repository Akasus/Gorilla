using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TextMesh_Pro.Scripts
{
    public class ChatController : MonoBehaviour
    {
        public Scrollbar chatScrollbar;


        public TMP_InputField tmpChatInput;

        public TMP_Text tmpChatOutput;

        private void OnEnable()
        {
            tmpChatInput.onSubmit.AddListener(AddToChatOutput);
        }

        private void OnDisable()
        {
            tmpChatInput.onSubmit.RemoveListener(AddToChatOutput);
        }


        private void AddToChatOutput(string newText)
        {
            // Clear Input Field
            tmpChatInput.text = string.Empty;

            var timeNow = DateTime.Now;

            tmpChatOutput.text += "[<#FFFF80>" + timeNow.Hour.ToString("d2") + ":" + timeNow.Minute.ToString("d2") + ":" +
                                  timeNow.Second.ToString("d2") + "</color>] " + newText + "\n";

            tmpChatInput.ActivateInputField();

            // Set the scrollbar to the bottom when next text is submitted.
            chatScrollbar.value = 0;
        }
    }
}