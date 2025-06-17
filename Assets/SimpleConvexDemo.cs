using System;
using System.Collections.Generic;
using TMPro;
using uniffi.convexmobile;
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
                    messagesText.text = data;
                },
                onError: (message, value) => Debug.LogError($"Custom error: {message}, value: {value}")
            ));
    }


}

