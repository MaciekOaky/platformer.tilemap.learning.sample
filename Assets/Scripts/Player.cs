using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Events
    public delegate void UpdateResourceHandler(int amount, Collectable.CollectableType type);
    public static UpdateResourceHandler ResourcesUpdated;

    // Changable variables from the Inspector - will stay constant on Gameplay
    [SerializeField] private float _speed;
    [SerializeField] private float _jumpSpeed;
    [SerializeField] [Range(.1f, .9f)] private float _waterSpeedDump;
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] [Range(0f, 1f)] private float _groundCheckRadius;
    [SerializeField] private float _oxygenLossInterval;

    // Input varaiables
    private float _xInput, _yInput;
    private bool _jumpInput;

    // States
    private bool _grounded;
    private bool _inWater;
    private bool _died;
    private Collider2D _bridgeCollider;
    private Vector2 _initialPosition;
    private int _oxygenAmount;
    private int _fuelAmount;

    // Components
    private Rigidbody2D _rb;
    private Collider2D _collider;
    private SpriteRenderer _sr;
    private Animator _animator;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _sr = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();

        _initialPosition = transform.position;
    }


    private void Start()
    {
        StartCoroutine(ControlOxygen());
    }


    // Updates every frame
    private void Update()
    {
        UpdateInput();
        UpdateLookDirection();
    }


    // All physics based operations (once in a few Updates)
    private void FixedUpdate()
    {
        CheckGround();
        
        if(!_died) // Can't move when dead
            Move();
    }


    private void UpdateInput()
    {
        _xInput = Input.GetAxis("Horizontal");
        _yInput = Input.GetAxis("Vertical");

        if(!_jumpInput && _grounded && !_inWater) // We ommit check if we already clicked jump button but jump has not been performed in FixedUpdate, yet
            _jumpInput = Input.GetButtonDown("Jump");
    }


    private void Move()
    {
        Vector2 velocity = _rb.velocity;
        
        if(_bridgeCollider != null && _yInput < -.5f && _jumpInput)
        {
            StartCoroutine(JumpOff());
            _jumpInput = false;
            _grounded = false;
            return;
        }

        // Apply horizontal movement as a RigidBody velocity.x
        velocity.x = _xInput * _speed;

        // Jumping
        if (_jumpInput)
        {
            _jumpInput = false; // Reset jump input so we can click it again
            velocity.y = _jumpSpeed;
        }

        // Apply dump when in water and allow vertical movement
        if (_inWater)
        {
            if(Mathf.Abs(_yInput) > Mathf.Epsilon) // Apply _yInput only when we move vertically, otherwise, we let physics work
                velocity.y = _yInput * _speed;

            velocity *= _waterSpeedDump;
        }

        // Apply changes to the Rigidbody's velocity
        _rb.velocity = velocity;

        _animator.SetFloat("HorizontalVelocity", Mathf.Abs(velocity.x)); // Don't care about direction
        _animator.SetFloat("VerticalVelocity", velocity.y);
    }


    private void CheckGround()
    {
        var hit = Physics2D.OverlapCircle(GetGroundCheckOrigin(), _groundCheckRadius, _groundMask);
        _grounded = hit != null; // If there's any collision with Circle generated above, then we are grounded

        _animator.SetBool("Grounded", _grounded);
    }


    private Vector2 GetGroundCheckOrigin()
    {
        if (_collider == null)
            _collider = GetComponent<Collider2D>();

        return new Vector2(_collider.bounds.center.x, _collider.bounds.min.y);
    }


    private void UpdateLookDirection()
    {
        if(Mathf.Abs(_xInput) > Mathf.Epsilon) // If we're really moving...
            _sr.flipX = _xInput < 0f;
    }


    public void SetInWaterState(bool state)
    {
        _inWater = state;
        _animator.SetBool("InWater", state);
        if (!state && !_died) _jumpInput = true; // Let's give a kick out of the water
    }


    public void UpdateResource(int amount, Collectable.CollectableType resourceType)
    {
        switch(resourceType)
        {
            case Collectable.CollectableType.OXYGEN:
                _oxygenAmount += amount;

                if (_oxygenAmount < 0)
                    _oxygenAmount = 0;

                ResourcesUpdated?.Invoke(_oxygenAmount, Collectable.CollectableType.OXYGEN);

                break;

            case Collectable.CollectableType.FUEL:
                _fuelAmount += amount;
                ResourcesUpdated?.Invoke(_fuelAmount, Collectable.CollectableType.FUEL);
                break;
        }
    }


    public void SetOnBridgeState(Collider2D c) => _bridgeCollider = c;


    private IEnumerator JumpOff()
    {
        Collider2D bridgeCollider = _bridgeCollider;
        Physics2D.IgnoreCollision(_collider, bridgeCollider, true);
        yield return new WaitForSeconds(.5f);
        Physics2D.IgnoreCollision(_collider, bridgeCollider, false);
    }


    private IEnumerator ControlOxygen()
    {
        while(true)
        {
            yield return new WaitUntil(() => _inWater && !_died);
            
            yield return new WaitForSeconds(_oxygenLossInterval);
            
            if (_inWater) // Reduce oxygen
            {
                UpdateResource(-1, Collectable.CollectableType.OXYGEN);
                
                // Out of oxygen? Die!
                if (_oxygenAmount == 0) Die();
            }
            
            yield return new WaitForEndOfFrame();
        }
    }


    public void Die()
    {
        _animator.SetTrigger("Die");
        _died = true;
        Time.timeScale = 0f; // Pasue the game
    }


    private void Respawn()
    {
        transform.position = _initialPosition;
        Time.timeScale = 1f; // Resume the game
        StartCoroutine(DieCountdown());
    }


    private IEnumerator DieCountdown()
    {
        yield return new WaitForSeconds(.5f);
        _died = false;
    }


    private void OnDrawGizmosSelected()
    {
        // Draw a helping sphere to see how big is collision checker
        Gizmos.DrawSphere(GetGroundCheckOrigin(), _groundCheckRadius);
    }
}
