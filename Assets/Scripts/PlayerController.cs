using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    // Keybindings
    [SerializeField] private KeyCode jumpKeyCode = KeyCode.Space;
    [SerializeField] private KeyCode sprintKeyCode = KeyCode.LeftShift;

    // Player values
    [SerializeField] private float gravityValue = -9.81f;
    [SerializeField] private float walkSpeed = 2.0f;
    [SerializeField] private float sprintSpeed = 4.0f;

    [SerializeField] private float jumpHeight = 1.0f;
    //[SerializeField] private float rotationSpeed = 10f;

    // References
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Text healthText;
    [SerializeField] private GameObject weaponHolder;
    private CharacterController _controller;

    // Variables
    [SerializeField] private float health = 100f;
    private float _remainingHealth;
    private Vector3 _playerVelocity;
    private bool _groundedPlayer;
    private float _inputVertical;
    private float _inputHorizontal;
    private float _playerSpeed;
    private Transform _cameraTransform;

    // Animation Variables
    private bool _canWalk;
    private bool _canRun;
    private static readonly int Walk = Animator.StringToHash("Walk");
    private static readonly int Run = Animator.StringToHash("Run");
    private static readonly int Jump = Animator.StringToHash("Jump");
    private static readonly int Gun = Animator.StringToHash("Gun");
    private static readonly int Fire = Animator.StringToHash("Fire");

    private void Start()
    {
        _remainingHealth = health;
        _controller = GetComponent<CharacterController>();
        _cameraTransform = Camera.main!.transform;
        healthText.text = _remainingHealth + " / " + health;
    }

    private void Update()
    {
        MyInput();
        Movement();
    }

    private void Movement()
    {
        if (_groundedPlayer && _playerVelocity.y < 0) _playerVelocity.y = gravityValue * 0.1f;

        var move = new Vector3(_inputHorizontal, 0, _inputVertical);
        move = move.x * _cameraTransform.right.normalized + move.z * _cameraTransform.forward.normalized;
        move.y = 0f;

        _controller.Move(move * (Time.deltaTime * _playerSpeed));

        if (move != Vector3.zero) gameObject.transform.forward = move;

        _playerVelocity.y += gravityValue * Time.deltaTime;
        _controller.Move(_playerVelocity * Time.deltaTime);
    }

    private void MyInput()
    {
        _inputHorizontal = Input.GetAxis("Horizontal");
        _inputVertical = Input.GetAxis("Vertical");
        _groundedPlayer = _controller.isGrounded;

        _playerSpeed = Input.GetKey(sprintKeyCode) ? sprintSpeed : walkSpeed;

        // Changes the height position of the player..
        if (Input.GetKeyDown(jumpKeyCode) && _groundedPlayer) StartCoroutine(JumpPlayer());

        // Animations
        if (_inputHorizontal != 0 || _inputVertical != 0)
        {
            _canWalk = Math.Abs(_playerSpeed - walkSpeed) < 0.001f;
            _canRun = Math.Abs(_playerSpeed - sprintSpeed) < 0.001f;
        }
        else
        {
            _canWalk = false;
            _canRun = false;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.Instance.PauseGame();
        }

        playerAnimator.SetBool(Walk, _canWalk);
        playerAnimator.SetBool(Run, _canRun);
        playerAnimator.SetBool(Gun, weaponHolder.transform.childCount > 0);
        playerAnimator.SetBool(Fire, Input.GetKey(KeyCode.Mouse0) && weaponHolder.transform.childCount > 0);
    }

    private IEnumerator JumpPlayer()
    {
        playerAnimator.SetTrigger(Jump);
        yield return new WaitForSeconds(0.3f);
        _playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
    }

    public void TakeDamage(float amount)
    {
        _remainingHealth -= amount;
        healthText.text = _remainingHealth + " / " + health;
        if (_remainingHealth <= 0f)
        {
            var loseCount = PlayerPrefs.GetInt("Lose");
            loseCount++;
            PlayerPrefs.SetInt("Lose", loseCount);

            GameManager.Instance.ShowFinalPanel(false);
        }
    }
}