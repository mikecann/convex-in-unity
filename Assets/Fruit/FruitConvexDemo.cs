using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FruitConvexDemo : MonoBehaviour
{

    [SerializeField] private ConvexClient convex;
    [SerializeField] private TMP_Text fruitPoppedText;

    [SerializeField] private Fruit bananaPrefab;
    [SerializeField] private Fruit watermelonPrefab;
    [SerializeField] private Fruit strawberryPrefab;
    [SerializeField] private Fruit orangePrefab;
    [SerializeField] private Fruit applePrefab;

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

                    foreach (var fruit in fruits)
                    {
                        Debug.Log($"Fruit: {fruit.kind}");
                        var prefab = GetPrefab(fruit.kind);
                        if (prefab)
                        {
                            var fruitInstance = Instantiate(prefab, transform);
                            fruitInstance.convex = convex;
                            fruitInstance.fruitId = fruit._id;
                            fruitInstance.gameObject.SetActive(true);
                            fruitInstance.transform.position = new Vector3(UnityEngine.Random.Range(-10, 10), 0, UnityEngine.Random.Range(-10, 10));
                            fruitInstance.rigidbody.AddForce(new Vector3(UnityEngine.Random.Range(-10, 10), 0, UnityEngine.Random.Range(-10, 10)), ForceMode.Impulse);
                        }
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
}