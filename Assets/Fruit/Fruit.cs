using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Fruit : MonoBehaviour
{
    public ConvexClient convex;
    public string fruitId;
    public Rigidbody rigidbody;

    [SerializeField] private AudioClip popSound;
    [SerializeField] private ParticleSystem popParticles;
    private AudioSource audioSource;
    private bool isPopped = false;

    async void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();

        // Get or add AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (!audioSource)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    async void Start()
    {
        if (!convex) return;
    }

    void Update()
    {
        // Alternative click detection using raycasting with new Input System
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            CheckForMouseClick();
        }
    }

    void CheckForMouseClick()
    {
        Camera cam = Camera.main;
        if (!cam) return;

        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = cam.ScreenPointToRay(mousePosition);

        int layerMask = ~(1 << 8); // Exclude layer 8 (Walls) - adjust layer number as needed
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            if (hit.collider.gameObject == gameObject)
            {
                Debug.Log($"Raycast hit fruit: {fruitId}");
                if (!isPopped)
                {
                    PopFruit();
                }
            }
        }


    }


    private async void PopFruit()
    {
        if (isPopped) return;
        isPopped = true;

        Debug.Log($"Popping fruit {fruitId}");

        // Play pop sound effect
        if (popSound && audioSource)
            audioSource.PlayOneShot(popSound);

        try
        {
            // Call the pop mutation on the server
            await convex.Mutation("fruit:pop", new Dictionary<string, string>
            {
                { "fruitId", fruitId }
            });

            Debug.Log($"Successfully popped fruit {fruitId} on server");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error popping fruit {fruitId}: {e.Message}");
        }

        // The fruit will be destroyed automatically when the subscription updates
        // and removes it from the server list
    }
}
