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
    public void Action()
    {
        if(interactionType == InteractionType.PickUp)
        {
            playerControl.AddObjectToInventory(gameObject);
        }
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
}
