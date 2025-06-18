using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FruitConvexDemo : MonoBehaviour
{

    [SerializeField] private ConvexClient convex;
    [SerializeField] private TMP_Text fruitPoppedText;

    [SerializeField] private Fruit bananaPrefab;
    [SerializeField] private Fruit watermelonPrefab;
    [SerializeField] private Fruit strawberryPrefab;
    [SerializeField] private Fruit orangePrefab;
    [SerializeField] private Fruit applePrefab;

    private Dictionary<string, Fruit> activeFruits = new Dictionary<string, Fruit>();

    async void Start()
    {
        Debug.Log($"Ensuring game exists..");
        await convex.Mutation("fruit:ensureGameIsCreated", new Dictionary<string, string>());

        Debug.Log($"Subscribing to fruit list..");
        await convex.Subscribe("fruit:list", new Dictionary<string, string>(),
            new ConvexQuerySubscriber<ConvexFruit[]>(
                                onUpdate: (fruits) =>
                {
                    Debug.Log($"Received {fruits.Length} fruits");

                    // Get current fruit IDs from the server
                    var currentFruitIds = new HashSet<string>();
                    foreach (var fruit in fruits)
                        currentFruitIds.Add(fruit._id);

                    // Remove fruit that no longer exist on the server
                    var fruitsToRemove = new List<string>();
                    foreach (var kvp in activeFruits)
                    {
                        if (currentFruitIds.Contains(kvp.Key))
                            continue;

                        fruitsToRemove.Add(kvp.Key);

                        if (kvp.Value == null)
                            continue;

                        Debug.Log($"Removing fruit {kvp.Key} - no longer exists on server");
                        Destroy(kvp.Value.gameObject);
                    }

                    foreach (var fruitId in fruitsToRemove)
                        activeFruits.Remove(fruitId);

                    // Add new fruit that don't exist locally
                    foreach (var fruit in fruits)
                    {
                        if (activeFruits.ContainsKey(fruit._id)) continue;

                        Debug.Log($"Creating new fruit: {fruit.kind} (ID: {fruit._id}) at position ({fruit.initialPosition.x}, {fruit.initialPosition.y}, {fruit.initialPosition.z})");

                        var prefab = GetPrefab(fruit.kind);
                        if (!prefab) continue;

                        var fruitInstance = Instantiate(prefab, transform);
                        fruitInstance.convex = convex;
                        fruitInstance.fruitId = fruit._id;
                        fruitInstance.gameObject.SetActive(true);

                        // Use the initial position and velocity from Convex
                        fruitInstance.transform.position = fruit.initialPosition.ToUnityVector3();
                        fruitInstance.rigidbody.AddForce(fruit.initialVelocity.ToUnityVector3(), ForceMode.Impulse);

                        // Track this fruit instance
                        activeFruits[fruit._id] = fruitInstance;
                    }
                },
                onError: (message, value) => Debug.LogError($"Custom error: {message}, value: {value}")
            ));

        Debug.Log($"Subscribing to fruit game..");
        await convex.Subscribe("fruit:getGame", new Dictionary<string, string>(),
            new ConvexQuerySubscriber<ConvexFruitGame>(
                onUpdate: (game) =>
                {
                    Debug.Log($"Received game: {game.numFruitPopped}");
                    fruitPoppedText.text = $"Fruit Popped: {game.numFruitPopped}";
                }
            ));

    }

    private Fruit GetPrefab(string kind)
    {
        switch (kind)
        {
            case "banana": return bananaPrefab;
            case "watermelon": return watermelonPrefab;
            case "strawberry": return strawberryPrefab;
            case "orange": return orangePrefab;
            case "apple": return applePrefab;
            default: return null;
        }
    }

    public void OnFruitDestroyed(string fruitId)
    {
        if (!activeFruits.ContainsKey(fruitId)) return;
        activeFruits.Remove(fruitId);
    }
}

[Serializable]
public class ConvexFruitGame : ConvexObject
{
    public double numFruitPopped;
}

[Serializable]
public class ConvexFruit : ConvexObject
{
    public string kind;
    public ConvexVector3 initialPosition;
    public ConvexVector3 initialVelocity;
}

[Serializable]
public class ConvexVector3
{
    public float x;
    public float y;
    public float z;

    public Vector3 ToUnityVector3() => new Vector3(x, y, z);
}