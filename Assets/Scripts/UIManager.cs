using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] bool isMoving;
    [SerializeField] int inventoryR, inventoryC;
    [SerializeField] PlayerControl playerControl;
    [SerializeField] GameObject optionMenu, armorMenu;
    [SerializeField] RectTransform canvas;
    [SerializeField] Camera mainCamera;
    [SerializeField] GameObject inventory;

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            Clear();
        }
    }
    public void SetModeToMove()
    {
        isMoving = true;
        optionMenu.SetActive(false);
    }

    public void MoveItem(string rc)
    {
        if(isMoving && inventory.activeSelf)
        {
            playerControl.MoveItem(inventoryR , inventoryC, rc[0] - '0', rc[2] - '0');
            Clear();
        }
    }
    public void SetSelected(string rc)
    {
        if (!isMoving && inventory.activeSelf)
        {
            this.inventoryR = rc[0] - '0';
            this.inventoryC = rc[2] - '0';
            optionMenu.SetActive(true);
            armorMenu.SetActive(false);
            Vector2 mousePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas, Input.mousePosition, null, out mousePos);
            optionMenu.transform.localPosition = mousePos;
        }
    }

    public void SetSelectedArmor(int slot)
    {
        if (inventory.activeSelf)
        {
            this.inventoryR = slot; 
            armorMenu.SetActive(true);
            optionMenu.SetActive(false);
            Vector2 mousePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas, Input.mousePosition, null, out mousePos);
            armorMenu.transform.localPosition = mousePos;
        }
    }

    public void Unequip()
    {
        playerControl.UnequipArmor(inventoryR);
        Clear();
    }

    public void Use()
    {
        if (inventory.activeSelf)
        {
            playerControl.UseItem(inventoryR, inventoryC);
            Clear();
        }
    }

    public void Clear()
    {
        inventoryC = 0;
        inventoryR = 0;
        optionMenu.SetActive(false);
        armorMenu.SetActive(false);
        isMoving = false;
    }
}
