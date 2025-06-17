using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SimpleConvexDemo : MonoBehaviour
{

    [SerializeField] private ConvexClient convex;
    [SerializeField] private TMP_InputField userNameInput;
    [SerializeField] private TMP_InputField messageInput;
    [SerializeField] private Button sendButton;
    [SerializeField] private TMP_Text messagesText;

    async void Start()
    {
        await convex.Subscribe("messages:list", new Dictionary<string, string>(),
            new ConvexQuerySubscriber<ConvexMessage[]>(
                onUpdate: (messages) =>
                {
                    Debug.Log($"Received {messages.Length} messages");

                    // Now you have strongly typed data!
                    string displayText = "";
                    foreach (var msg in messages)
                    {
                        displayText += $"[{msg.CreationDateTime:HH:mm:ss}] {msg.user}: {msg.message}\n";
                    }
                    messagesText.text = displayText;
                },
                onError: (message, value) => Debug.LogError($"Custom error: {message}, value: {value}")
            ));

        sendButton.onClick.AddListener(HandleSend);
        messageInput.onSubmit.AddListener(_ => HandleSend());
    }

    private async void HandleSend()
    {
        try
        {
            await convex.Mutation("messages:addMessage", new Dictionary<string, string>
            {
                { "message", messageInput.text },
                { "user", userNameInput.text }
            });
            messageInput.text = string.Empty;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error sending message: {e.Message}");
        }
    }


}
