using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class OrderMenu : MonoBehaviour
{
    private InventoryManager inventoryManager;
    private GameManager gameManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
