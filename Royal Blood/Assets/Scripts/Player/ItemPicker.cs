﻿using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class ItemPicker : MonoBehaviour
{
    public int totalDiamonds = 0;


    private void Awake()
    {
        // Đảm bảo collider này luôn là trigger
        GetComponent<CircleCollider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Pickable"))
        {
            BaseItem item = other.GetComponent<BaseItem>();
            if (item != null)
            {
                item.StartPull(transform);
                return;
            }
        }
    }

    public int GetTotalDiamonds() => totalDiamonds;
    public void LootDiamonds() => totalDiamonds++;
}