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
    [SerializeField] GameObject optionMenu;
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
            Vector2 mousePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas, Input.mousePosition, null, out mousePos);
            optionMenu.transform.localPosition = mousePos;
            Debug.Log(Display.RelativeMouseAt(UnityEngine.Input.mousePosition));
            Debug.Log(Input.mousePosition);
        }
    }

    public void Clear()
    {
        inventoryC = 0;
        inventoryR = 0;
        optionMenu.SetActive(false);
        isMoving = false;
    }
}
