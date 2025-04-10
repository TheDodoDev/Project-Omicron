using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerCam : MonoBehaviour
{
    // Start is called before the first frame update
    [Range(0.1f, 10.0f)] [SerializeField] float sensX, sensY;

    [SerializeField] Transform orientation;

    private GameObject player;
    private float xRotation, yRotation;
    private int score;
    private float shotsFired, shotsHit;

    //Object Interaction
    [SerializeField] LayerMask whatIsInteractable;
    [SerializeField] GameObject actionNotification;
    void Start()
    {
        player = GameObject.Find("Player");
        score = 0;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX * 100;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY * 100;

        yRotation += mouseX;
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);

        if(player.GetComponent<PlayerControl>().IsInventoryOpen())
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        Ray shot = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool rayCast = Physics.Raycast(shot, out hit, 5, whatIsInteractable);
        if (rayCast)
        {
            actionNotification.SetActive(true);
            if(Input.GetKeyDown(KeyCode.F))
            {
                InteractableBehavior interactable = hit.transform.gameObject.GetComponent<InteractableBehavior>();
                if (interactable.Action())
                {
                    hit.transform.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            actionNotification.SetActive(false);
        }

    }
    public void SetSens(float sens)
    {
        sensX = sens;
        sensY = sens;
    }
}
