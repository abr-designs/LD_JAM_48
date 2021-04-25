using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TriggerWaterSound : MonoBehaviour
{
    [SerializeField]
    private string searchTag = "Player";

    [SerializeField]
    private GameObject waterEffectPrefab;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(searchTag))
            return;
        
        AudioController.PlaySound(AudioController.SFX.WATER);
        CreateEffects(waterEffectPrefab, other.transform.position);
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(searchTag))
            return;
        
        AudioController.PlaySound(AudioController.SFX.WATER);
        CreateEffects(waterEffectPrefab, other.transform.position);
    }

    //====================================================================================================================//
    private static void CreateEffects(in GameObject prefab, in Vector2 position, in float killTime = 5f)
    {
        var effect = Instantiate(prefab, position, Quaternion.identity);
        Destroy(effect, killTime);
    }
}
