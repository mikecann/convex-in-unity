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
    [SerializeField] private ParticleSystem popParticlesPrefab;
    private AudioSource audioSource;
    private bool isPopped = false;

    async void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();

        // Get or add AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource != null) return;

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        // Particle system will be instantiated from prefab when needed
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

        if (!Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) return;
        if (hit.collider.gameObject != gameObject) return;
        if (isPopped) return;

        PopFruit();
    }


    private async void PopFruit()
    {
        if (isPopped) return;
        isPopped = true;

        // Immediate visual feedback - hide the fruit and disable clicking
        var meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer) meshRenderer.enabled = false;

        var collider = GetComponent<Collider>();
        if (collider) collider.enabled = false;

        // Play pop sound effect
        if (popSound && audioSource)
            audioSource.PlayOneShot(popSound);

        // Play particle effect by instantiating prefab
        PlayParticleEffect();

        try
        {
            // Call the pop mutation on the server
            await convex.Mutation("fruit:pop", new Dictionary<string, string>
            {
                { "fruitId", fruitId }
            });

        }
        catch (Exception e)
        {
            Debug.LogError($"Error popping fruit {fruitId}: {e.Message}");
        }

        // The fruit will be destroyed automatically when the subscription updates
        // and removes it from the server list
    }

    private void PlayParticleEffect()
    {
        if (!popParticlesPrefab)
        {
            Debug.LogWarning($"No particle system prefab assigned to fruit {fruitId}!");
            return;
        }

        var particleInstance = Instantiate(popParticlesPrefab, transform.position, Quaternion.identity);

        // Make sure the particle system is set up correctly
        var main = particleInstance.main;
        main.playOnAwake = false; // Ensure it doesn't auto-play

        // Force play the particle system
        particleInstance.Stop(); // Stop any existing playback
        particleInstance.Play();


        // Destroy the particle system after it finishes (use a longer time to be safe)
        float destroyTime = Mathf.Max(3f, main.startLifetime.constantMax + 2f);
        Destroy(particleInstance.gameObject, destroyTime);
    }

    // ConfigureDefaultParticleSystem method removed - using prefab instead
}
