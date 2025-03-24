using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableBehavior : MonoBehaviour
{
    [SerializeField]enum InteractionType{
        PickUp,
        Activate
    }
    [SerializeField] InteractionType interactionType;

    [SerializeField] PlayerControl playerControl;

    [SerializeField] Sprite icon;

    [SerializeField] string name;

    [SerializeField] int id;

    [SerializeField] int num;

    [SerializeField] GameObject equipped;
    public bool Action()
    {
        if(interactionType == InteractionType.PickUp)
        {
            return playerControl.AddObjectToInventory(gameObject);
        }
        return false;
    }

    public Sprite GetIcon()
    {
        return icon;
    }

    public string GetName()
    {
        return name;
    }

    public int GetID()
    {
        return id;
    }

    public int GetNum()
    {
        return num;
    }

    public void SetNum(int num)
    {
        this.num = num;
    }

    public GameObject GetEquipped()
    {
        return equipped;
    }
}
