/*Credit to the following tutorial for help with this script:
https://www.youtube.com/watch?v=_QajrabyTJc
*/


using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
public class CharacterMoveBehaviour : MonoBehaviour

{
    [SerializeField] private Animator animator;
    private LookChar playerInput;
    private CharacterController controller;
    private Transform camMain;
    [SerializeField] private float rotationSpeed = 4f;
    [SerializeField] private Button attackButton;
    [SerializeField] private Button rollButton;
    public UnityEvent onAttack, offAttack, onCharDeath;
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;
    [SerializeField] private Image[] hearts;
    [SerializeField] private int heartNum;
    [SerializeField] private int health;
    private bool canLose = true;
    [SerializeField] private Material orgMat;
    [SerializeField] private Material flashMat;
    [SerializeField] private Renderer rendBody;
    [SerializeField] private Renderer rendSword;
    [SerializeField] private Renderer rendHead;
    [SerializeField] private Renderer rendFeet;
    


    [SerializeField] private FloatData speed;

    public bool canMove = true;

    private void Awake()
    {
        playerInput = new LookChar();
        controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        camMain = Camera.main.transform;
    }

    private void OnEnable()
    {
        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }
    void Update()
    {
        if (canMove)
        {


            Vector2 movementInput = playerInput.Main.Move.ReadValue<Vector2>();
            Vector3 move = (camMain.forward * movementInput.y + camMain.right * movementInput.x);
            move.y = 0f;
            controller.Move(move * (Time.deltaTime * speed.Value));
            if (movementInput != Vector2.zero)
            {

                float targAngle = Mathf.Atan2(movementInput.x, movementInput.y) * Mathf.Rad2Deg + camMain.eulerAngles.y;
                Quaternion rotation = Quaternion.Euler(0f, targAngle, 0f);
                transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
                animator.SetBool("IsHurt", false);
                animator.SetBool("IsWalking", true);
            }
            else
            {
                animator.SetBool("IsWalking", false);
                if (health < 2)
                {
                    animator.SetBool("IsHurt", true);
                }
            }
        }
        
        
    
    if (health > heartNum)
        {
            health = heartNum;
        }

        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < health)
            {
                hearts[i].sprite = fullHeart;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }

            if (i < heartNum)
            {
                hearts[i].enabled = true;
            }

            else
            {
                hearts[i].enabled = false;
            }
        }

        if (health <= 0)
        {
            canMove = false;
            animator.SetBool("IsHurt", false);
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsDead", true);
            
            onCharDeath.Invoke();
            
        }
    }

    public void isRoll()
    {
        
        canMove = false;
        animator.SetBool("IsWalking", false);
        animator.SetBool("IsRoll", true);
        rollButton.onClick.AddListener(() =>
        {
            rollButton.interactable = false;
        });

        StartCoroutine(WaitForReactiveRoll());
        StartCoroutine(WaitForAnim("IsRoll"));
    }

    public void loseHealth()
    {
        if (canLose)
        {
            health -= 1;
        }
    }
    
    public void BeginFlash()
    {
        canLose = false;
        StartCoroutine(OnFlash());
    }

    private IEnumerator OnFlash()
    {
        
        rendBody.sharedMaterial = flashMat;
        rendFeet.sharedMaterial = flashMat;
        rendHead.sharedMaterial = flashMat;
        rendSword.sharedMaterial = flashMat;
        yield return new WaitForSeconds(0.5f);
        rendBody.sharedMaterial = orgMat;
        rendFeet.sharedMaterial = orgMat;
        rendSword.sharedMaterial = orgMat;
        rendHead.sharedMaterial = orgMat;
        canLose = true;

    }
    public void isAttack()
    {
        canMove = false;
        
        onAttack.Invoke();
        
        animator.SetBool("IsWalking", false);
        animator.SetBool("IsAttack", true);
        
        
        
        attackButton.onClick.AddListener(() =>
        {
            attackButton.interactable = false;
        });
    
        StartCoroutine(WaitForReactiveAttack());
        StartCoroutine(WaitForAnim("IsAttack"));

    }

    private IEnumerator WaitForAnim(string animName)
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);
        
        animator.SetBool(animName, false);
        offAttack.Invoke();
        canMove = true;
        
        
    }

    private IEnumerator WaitForReactiveAttack()
    {
        yield return new WaitForSeconds(4);
        attackButton.interactable = true;
        
        
    }
    
    private IEnumerator WaitForReactiveRoll()
    {
        yield return new WaitForSeconds(4);
        rollButton.interactable = true;
        
    }
    
}
