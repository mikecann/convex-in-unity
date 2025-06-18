using System;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : MonoBehaviour
{

    public ConvexClient convex;
    public string fruitId;
    public Rigidbody rigidbody;


    async void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    async void Start()
    {
        if (!convex) return;
    }





}
