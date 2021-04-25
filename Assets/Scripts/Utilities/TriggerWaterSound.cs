using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TriggerWaterSound : MonoBehaviour
{
    [SerializeField]
    private string searchTag = "Player";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(searchTag))
            return;
        
        AudioController.PlaySound(AudioController.SFX.WATER);
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(searchTag))
            return;
        
        AudioController.PlaySound(AudioController.SFX.WATER);
    }
}
