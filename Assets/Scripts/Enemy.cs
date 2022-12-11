using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    // Variables
    [SerializeField] private float maxHealth = 50f;
    [SerializeField] private float health = 50f;
    [SerializeField] private float damage = 5f;
    [SerializeField] private float attackDuration = 1f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float speed = 2f;
    
    // References
    [SerializeField] private Transform player;
    [SerializeField] private GameObject hpBar;
    [SerializeField] private GameObject hpIndicator;

    // Private variables
    private NavMeshAgent _enemy;
    private Animator _animator;
    private BoxCollider _collider;
    private Vector3 _startPosition;
    private Vector3 _defaultScale;
    private Vector2 _hpBarSize;
    private bool _followPlayer = false;
    private float _lastDamage;
    private bool _attacking = false;
    private bool _isDied = false;
    
    // Animation variables
    private static readonly int AttackAnim = Animator.StringToHash("Attack");
    private static readonly int Die1 = Animator.StringToHash("Die");

    private void Start()
    {
        _enemy = GetComponent<NavMeshAgent>();
        _enemy.speed = speed;

        _startPosition = transform.position;
        _collider = GetComponent<BoxCollider>();
        _defaultScale = _collider.size;
        _hpBarSize = hpIndicator.GetComponent<RectTransform>().sizeDelta;

        _animator = GetComponent<Animator>();
    }

    private void LateUpdate()
    {
        if (_followPlayer)
        {
            _enemy.SetDestination(player.position);

            _animator.SetBool("Walk", true);

            if (_enemy.remainingDistance <= attackRange && !_attacking)
            {
                _animator.SetBool("Walk", false);
                _attacking = true;
                StartCoroutine(Attack());
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _followPlayer = true;
            StartCoroutine(ResetEnemy());
            _collider.size = new Vector3(1f, 5f, 1f);
        }
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        var rectTransform = hpIndicator.GetComponent<RectTransform>();

        var damage = (amount / maxHealth) * 100;
        var hpDown = (_hpBarSize.x / 100) * damage;

        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x - hpDown, rectTransform.sizeDelta.y);
        _followPlayer = true;

        _lastDamage = Time.time;

        StartCoroutine(ResetEnemy());

        if (health <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        hpBar.SetActive(false);
        hpIndicator.GetComponent<RectTransform>().sizeDelta = _hpBarSize;

        if (!_isDied)
        {
            ZombieCount.Instance.CountDown();
            _isDied = true;
        }

        _animator.SetTrigger(Die1);

        _followPlayer = false;

        StartCoroutine(DestroyEnemy());
        _enemy.isStopped = true;

        _animator.StopPlayback();
    }

    private IEnumerator DestroyEnemy()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }

    private IEnumerator ResetEnemy()
    {
        yield return new WaitForSeconds(20f);

        if (Time.time - _lastDamage <= 20f) yield break;

        _followPlayer = false;
        health = maxHealth;
        _followPlayer = false;
        hpBar.SetActive(false);
        _enemy.SetDestination(_startPosition);
        _collider.size = new Vector3(_defaultScale.x, _collider.size.y, _defaultScale.z);
    }

    private IEnumerator Attack()
    {
        yield return new WaitForSeconds(attackDuration);

        _animator.SetTrigger(AttackAnim);

        if (_enemy.remainingDistance <= attackRange) player.GetComponent<PlayerController>().TakeDamage(damage);
        _attacking = false;
    }
}