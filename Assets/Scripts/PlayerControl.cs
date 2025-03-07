using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Movement")]
    
    [SerializeField] float walkSpeed;
    [SerializeField] float sprintSpeed;
    [SerializeField] Transform orientation;
    [SerializeField] float jumpForce;
    private float horizontalInput, verticalInput;
    private float movementSpeed;
    private Vector3 moveDirection = Vector3.forward;
    private bool isGrounded;
    private bool toggleSprint;
    private bool isJumping;

    //References
    private Rigidbody rb;
    private Animator animator;
    [SerializeField] GameObject healthBar, lostHealthBar, staminaBar, manaBar, lostManaBar;

    //Stats
    [SerializeField] float maxHP, currHP, maxStam, curStam, maxMana, currMana;
    private bool canReduceHealthBar, changingStam, canReduceManaBar, canIncreaseStaminaBar;

    //Inventory
    [SerializeField] GameObject hotBarMenu, selectionIndicator, inventoryMenu;
    private int currentSelected;
    GameObject[,] inventory = new GameObject[4, 4];
    GameObject[] hotBar = new GameObject[4];
    GameObject equipped;

    //Spells
    [SerializeField] GameObject electricBall;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        movementSpeed = walkSpeed;
        toggleSprint = true;
        isGrounded = true;
        animator = GetComponent<Animator>();
        maxHP = 100;
        currHP = maxHP;
        maxStam = 100;
        curStam = 100;
        currMana = 100;
        maxMana = 100;
        currentSelected = 1;
    }

    // Update is called once per frame
    void Update()
    {

        //Movement
        transform.rotation = orientation.rotation;
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        if (toggleSprint)
        {
            if (Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.LeftShift))
            {
                if (movementSpeed == walkSpeed)
                {
                    movementSpeed = sprintSpeed;
                }
                else
                {
                    movementSpeed = walkSpeed;
                }
            }
        }

        if (isGrounded && Input.GetAxisRaw("Jump") != 0)
        {
            isJumping = true;
        }

        //Inventory
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
        }
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchSelectedSlot(1);
            currentSelected = 1;
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchSelectedSlot(2);
            currentSelected = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchSelectedSlot(3);
            currentSelected = 3;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SwitchSelectedSlot(4);
            currentSelected = 4;
        }
        equipped = hotBar[currentSelected - 1];
        if (equipped != null)
        {
            Debug.Log("Equipped " + equipped.name);
            Debug.Log(equipped.GetComponent<InteractableBehavior>().GetID());

        }
        else
        {
            Debug.Log("Equipped Nothing");
        }
        if (equipped != null && equipped.GetComponent<InteractableBehavior>().GetID() == 1)
        {
            Debug.Log("Firing");
            if (Input.GetMouseButtonDown(0) && currMana >= 10)
            {
                GameObject o = Instantiate(electricBall, transform.position + transform.forward * 0.5f + transform.up * 2f, Quaternion.identity);
                Destroy(o, 3f);
                LoseMana(1);
            }
        }

        //Stats
        if (canReduceHealthBar)
        {
            canReduceHealthBar = false;
            StartCoroutine(EmptyHealthBar());
        }

        if(canReduceManaBar)
        {
            canReduceManaBar = false;
            StartCoroutine(EmptyManaBar());
        }

        

    }
    private void FixedUpdate()
    {
        if (curStam <= 0)
        {
            movementSpeed = walkSpeed;
        }

        rb.velocity = moveDirection.normalized * movementSpeed + new Vector3(0, rb.velocity.y, 0);

        if (movementSpeed == sprintSpeed && moveDirection.magnitude > 0 && !changingStam)
        {
            changingStam = true;
            StartCoroutine(EmptyStaminaBar());
            canIncreaseStaminaBar = false;
        }

        if ((movementSpeed == walkSpeed || moveDirection.magnitude == 0) && curStam < maxStam)
        {
            StartCoroutine(CheckIfNoStaminaLoss());
        }

        if (canIncreaseStaminaBar && !changingStam)
        {
            changingStam = true;
            StartCoroutine(FillStaminaBar());

        }
        if (curStam == maxStam)
        {
            canIncreaseStaminaBar = false;
        }
        if (isJumping)
        {
            Jump();
            isJumping = false;
        }

        ControlAnimation();
        curStam = Mathf.Min(maxStam, curStam);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Projectile"))
        {
            TakeDamage(other.GetComponent<VFXBehavior>().GetDamage());
            Destroy(other.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag("Ground"))
        {
            animator.SetBool("onGround", true);
            isGrounded = true;
            Debug.Log("Entered Ground");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            animator.SetBool("onGround", false);
            isGrounded = false;
            Debug.Log("Exited Ground");
        }
    }

    private void Jump()
    {
        if (curStam >= 10)
        {
            animator.SetTrigger("isJumping");
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
            curStam -= 10;
            staminaBar.GetComponent<Image>().rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 500 * curStam / maxStam);
        }
    }

    private void ControlAnimation()
    {
        if (moveDirection.magnitude > 0 && verticalInput != 0 && movementSpeed == walkSpeed)
        {
            animator.SetBool("isWalking", true);
            animator.SetBool("isRunning", false);
        }
        if (moveDirection.magnitude > 0 && verticalInput != 0 && movementSpeed == sprintSpeed)
        {
            animator.SetBool("isRunning", true);
            animator.SetBool("isWalking", false);
        }
        if (verticalInput == 0)
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
        }
        if (horizontalInput > 0 && movementSpeed == walkSpeed)
        {
            animator.SetBool("isWalkingRight", true);
            animator.SetBool("isWalkingLeft", false);
            animator.SetBool("isRunningRight", false);
            animator.SetBool("isRunningLeft", false);

        }
        else if (horizontalInput < 0 && movementSpeed == walkSpeed)
        {
            animator.SetBool("isWalkingRight", false);
            animator.SetBool("isWalkingLeft", true);
            animator.SetBool("isRunningRight", false);
            animator.SetBool("isRunningLeft", false);
        }
        else if (horizontalInput > 0 && movementSpeed == sprintSpeed)
        {
            animator.SetBool("isWalkingRight", false);
            animator.SetBool("isWalkingLeft", false);
            animator.SetBool("isRunningRight", true);
            animator.SetBool("isRunningLeft", false);
        }
        else if (horizontalInput < 0 && movementSpeed == sprintSpeed)
        {
            animator.SetBool("isWalkingRight", false);
            animator.SetBool("isWalkingLeft", false);
            animator.SetBool("isRunningRight", false);
            animator.SetBool("isRunningLeft", true);
        }
        else
        {
            animator.SetBool("isWalkingRight", false);
            animator.SetBool("isWalkingLeft", false);
            animator.SetBool("isRunningRight", false);
            animator.SetBool("isRunningLeft", false);
        }

        if (rb.velocity.y < -1.0f)
        {
            animator.SetBool("isFalling", true);
        }
        else
        {
            animator.SetBool("isFalling", false);
        }
    }

    public void SetSprintToggle(bool toggle)
    {
        this.toggleSprint = toggle;
    }

    public void TakeDamage(int damage)
    {
        currHP -= damage;
        healthBar.GetComponent<Image>().GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 500 * currHP/maxHP);
        StartCoroutine(CheckIfNotDamaged());
    }

    public void LoseMana(int manaLoss)
    {
        currMana -= manaLoss;
        manaBar.GetComponent<Image>().GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 500 * currMana / maxMana);
        StartCoroutine(CheckIfNoManaLoss());
    }

    public void InflictStatusEffect()
    {

    }

    public void SwitchSelectedSlot(int newSlot)
    {
        selectionIndicator.transform.localPosition = new Vector3(-200 + 100 * (newSlot - 1), 0, 0);
    }

    public void ToggleInventory()
    {
        inventoryMenu.SetActive(!inventoryMenu.activeSelf);
    }

    public bool IsInventoryOpen()
    {
        return inventoryMenu.activeSelf;
    }

    public bool AddObjectToInventory(GameObject gameObject)
    {
        bool foundLastSlot = false;
        for (int i = 0; i < hotBar.Length; i++)
        {
            if (hotBar[i] != null)
            {
                if (gameObject.GetComponent<InteractableBehavior>().GetID() == hotBar[i].GetComponent<InteractableBehavior>().GetID())
                {
                    hotBar[i].GetComponent<InteractableBehavior>().SetNum(hotBar[i].GetComponent<InteractableBehavior>().GetNum() + 1);
                    foundLastSlot = true;
                    RenderInventory();
                    return foundLastSlot;
                }
            }
        }
        for (int r = 0; r < inventory.GetLength(0) && !foundLastSlot; r++)
        {
            for (int c = 0; c < inventory.GetLength(1); c++)
            {
                if (inventory[r, c] != null && gameObject.GetComponent<InteractableBehavior>().GetID() == inventory[r, c].GetComponent<InteractableBehavior>().GetID())
                {
                    inventory[r, c].GetComponent<InteractableBehavior>().SetNum(inventory[r, c].GetComponent<InteractableBehavior>().GetNum() + 1);
                    RenderInventory();
                    foundLastSlot = true;
                    break;
                }
            }
        }
        for (int i = 0; i < hotBar.Length; i++)
        {
            if (hotBar[i] == null)
            {
                hotBar[i] = gameObject;
                hotBar[i].GetComponent<InteractableBehavior>().SetNum(1);
                foundLastSlot = true;
                RenderInventory();
                return foundLastSlot;
            }
        }
        for (int r = 0; r < inventory.GetLength(0) && !foundLastSlot; r++)
        {
            for (int c = 0; c < inventory.GetLength(1); c++)
            {
                if (inventory[r, c] == null)
                {
                    inventory[r, c] = gameObject;
                    inventory[r, c].GetComponent<InteractableBehavior>().SetNum(1);
                    foundLastSlot = true;
                    break;
                }
            }
        }
        RenderInventory();
        return foundLastSlot;
    }

    public void RenderInventory()
    {
        for (int i = 0; i < hotBar.Length; i++)
        {
            if (hotBar[i] != null)
            {
                hotBarMenu.transform.GetChild(i + 1).Find("Image").GetComponent<Image>().sprite = hotBar[i].GetComponent<InteractableBehavior>().GetIcon();
                hotBarMenu.transform.GetChild(i + 1).Find("Image").GetComponent<Image>().color = new Color(255, 255, 255, 0.6f);
                hotBarMenu.transform.GetChild(i + 1).Find("QuantityText").gameObject.SetActive(true);
                hotBarMenu.transform.GetChild(i + 1).Find("QuantityText").GetComponent<TextMeshProUGUI>().text = "" + hotBar[i].GetComponent<InteractableBehavior>().GetNum();
            }
            else
            {
                hotBarMenu.transform.GetChild(i + 1).Find("QuantityText").gameObject.SetActive(false);
                hotBarMenu.transform.GetChild(i + 1).Find("Image").GetComponent<Image>().color = new Color(255, 255, 255, 0.0f);
            }
        }
        for (int r = 0; r < inventory.GetLength(0); r++)
        {
            for (int c = 0; c < inventory.GetLength(1); c++)
            {
                if(inventory[r, c] != null)
                {
                    inventoryMenu.transform.Find("InvSlot" + r + "_" + c).Find("Image").GetComponent<Image>().sprite = inventory[r,c].GetComponent<InteractableBehavior>().GetIcon();
                    inventoryMenu.transform.Find("InvSlot" + r + "_" + c).Find("Image").GetComponent<Image>().color = new Color(255, 255, 255, 0.6f);
                    inventoryMenu.transform.Find("InvSlot" + r + "_" + c).Find("QuantityText").gameObject.SetActive(true);
                    inventoryMenu.transform.Find("InvSlot" + r + "_" + c).Find("QuantityText").GetComponent<TextMeshProUGUI>().text = "" + inventory[r, c].GetComponent<InteractableBehavior>().GetNum();
                }
                else
                {
                    inventoryMenu.transform.Find("InvSlot" + r + "_" + c).Find("QuantityText").gameObject.SetActive(false);
                    inventoryMenu.transform.Find("InvSlot" + r + "_" + c).Find("Image").GetComponent<Image>().color = new Color(255, 255, 255, 0.0f);
                }
            }
        }
    }

    public void MoveItem(int srcR, int srcC, int tarR, int tarC)
    {
        Debug.Log("Moving from (" + srcR + ", " + srcC + ") to (" + tarR + ", " + tarC + ")" );
        if(srcR == -1 && tarR == -1)
        {
            GameObject temp = hotBar[srcC];
            hotBar[srcC] = hotBar[tarC];
            hotBar[tarC] = temp;
        }
        if(srcR == -1 && tarR > -1)
        {
            GameObject temp = hotBar[srcC];
            hotBar[srcC] = inventory[tarR, tarC];
            inventory[tarR, tarC] = temp;

        }
        if (srcR > -1 && tarR == -1)
        {
            GameObject temp = inventory[srcR, srcC];
            inventory[srcR, srcC] = hotBar[tarC];
            hotBar[tarC] = temp;
        }
        if(srcR > -1 && tarR > -1)
        {
            GameObject temp = inventory[srcR, srcC];
            inventory[srcR, srcC] = inventory[tarR, tarC];
            inventory[tarR, tarC] = temp;
        }
        RenderInventory();
    }
    IEnumerator CheckIfNotDamaged()
    {
        float tempHP = currHP;
        yield return new WaitForSeconds(1.5f);
        if (tempHP <= currHP)
        {
            canReduceHealthBar = true;
        }
    }

    IEnumerator EmptyHealthBar()
    {
        float amountToReduce = lostHealthBar.GetComponent<Image>().rectTransform.rect.width - healthBar.GetComponent<Image>().rectTransform.rect.width;
        amountToReduce /= 10;
        Debug.Log(amountToReduce);
        for(int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(0.05f);
            lostHealthBar.GetComponent<Image>().rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, lostHealthBar.GetComponent<Image>().rectTransform.rect.width  - amountToReduce);
        }
    }

    IEnumerator CheckIfNoManaLoss()
    {
        float tempHP = currMana;
        yield return new WaitForSeconds(1f);
        if (tempHP <= currMana)
        {
            canReduceManaBar = true;
        }
    }

    IEnumerator EmptyManaBar()
    {
        float amountToReduce = lostManaBar.GetComponent<Image>().rectTransform.rect.width - manaBar.GetComponent<Image>().rectTransform.rect.width;
        amountToReduce /= 10;
        Debug.Log(amountToReduce);
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(0.05f);
            lostManaBar.GetComponent<Image>().rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, lostManaBar.GetComponent<Image>().rectTransform.rect.width - amountToReduce);
        }
    }

    IEnumerator EmptyStaminaBar()
    {
        yield return new WaitForSeconds(0.1f);
        curStam -= 2;
        staminaBar.GetComponent<Image>().rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 500 * curStam/maxStam);
        changingStam = false;
        canIncreaseStaminaBar = false;
    }

    IEnumerator FillStaminaBar()
    {
        yield return new WaitForSeconds(0.1f);
        if (curStam < maxStam) curStam += 1;
        staminaBar.GetComponent<Image>().rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 500 * curStam / maxStam);
        changingStam = false;
    }
    
    IEnumerator CheckIfNoStaminaLoss()
    {
        float tempStam = curStam;
        yield return new WaitForSeconds(1f);
        if (tempStam <= curStam)
        {
            canIncreaseStaminaBar = true;
        }
    }


}
