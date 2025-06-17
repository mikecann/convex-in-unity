using System;
using System.Collections.Generic;
using TMPro;
using uniffi.convexmobile;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;

public class SimpleConvexDemo : MonoBehaviour
{

    [SerializeField] private ConvexClient convex;

    [SerializeField] private TMP_InputField userNameInput;
    [SerializeField] private TMP_InputField messageInput;
    [SerializeField] private Button sendButton;
    [SerializeField] private TMP_Text messagesText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async void Start()
    {
        await convex.Subscribe("messages:list", new Dictionary<string, string>(),
            new ConvexQuerySubscriber(
                onUpdate: (data) =>
                {
                    Debug.Log($"Received data: {data}");
                    //const obj = JsonUtility.FromJson<Message[]>(data);
                    messagesText.text = data;
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
