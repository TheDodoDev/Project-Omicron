using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.SceneManagement;

public class InteractableBehavior : MonoBehaviour
{
    [SerializeField]enum InteractionType{
        PickUp,
        Activate,
        Open
    }

    public enum ItemType
    {
        None,
        Boots,
        Weapon,
        Healing,
        Replenishing,
        Chestplate
    }
    [SerializeField] InteractionType interactionType;

    [SerializeField] ItemType itemType;

    [SerializeField] PlayerControl playerControl;

    [SerializeField] Sprite icon;

    [SerializeField] string name;

    [SerializeField] int id;

    [SerializeField] int num;

    [SerializeField] GameObject equipped;

    [SerializeField] Material material;

    [SerializeField] GameObject portal1, portal2;

    //STATS
    [Header("STATS")]
    [SerializeField] int atk, def, agi, healing;
    public void Start()
    {
        playerControl = GameObject.Find("Player").GetComponent<PlayerControl>();
    }
    public bool Action()
    {
        AudioSource audiosrc = GetComponent<AudioSource>();
        if(interactionType == InteractionType.PickUp)
        {
            return playerControl.AddObjectToInventory(gameObject);
        }
        if (interactionType == InteractionType.Open)
        {
            gameObject.GetComponent<Animator>().SetTrigger("open");
            playerControl.AddCoins(Random.Range(3, 9));
            audiosrc.PlayOneShot(audiosrc.clip);
            gameObject.layer = LayerMask.NameToLayer("Default");
            return false;
        }
        if (interactionType == InteractionType.Activate)
        {
            gameObject.transform.Find("Icosphere").GetComponent<Renderer>().material = material;
            gameObject.layer = 0;
            if(gameObject.name.Equals("Green Monument"))
            {
                portal1.SetActive(true);
                portal2.SetActive(true);
            }
            audiosrc.PlayOneShot(audiosrc.clip);
        }
        return false;
    }

    public void Use()
    {
        if (itemType == ItemType.Chestplate)
        {
            playerControl.AddToArmor(gameObject, 1);
        }
        if (itemType == ItemType.Boots)
        {
            playerControl.AddToArmor(gameObject, 3);
        }
        if(itemType == ItemType.Healing)
        {
            playerControl.Heal(healing);
        }
        if (itemType == ItemType.Replenishing)
        {
            playerControl.GainMana(healing);
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

    public GameObject GetEquipped()
    {
        return equipped;
    }

    public int GetAtk()
    {
        return atk;
    }

    public int GetDef()
    {
        return def;
    }

    public int GetAgi()
    {
        return agi;
    }

    public ItemType GetItemType()
    { return itemType; }


}
