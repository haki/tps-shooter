using UnityEngine;

public class PickUpController : MonoBehaviour
{
    public KeyCode pickUpKeyCode = KeyCode.E;
    public KeyCode dropKeyCode = KeyCode.Q;

    public GunSystem gun;
    public Rigidbody rb;
    public BoxCollider coll;

    public Transform player;
    public Transform gunContainer;

    public float pickUpRange;
    public float dropForwardForce;
    public float dropUpwardForce;

    public bool equipped;
    public static bool slotFull;

    public Vector3 defaultRotation;

    private Transform _camera;

    private void Start()
    {
        _camera = Camera.main!.transform;
        if (!equipped)
        {
            gun.enabled = false;
            rb.isKinematic = false;
            coll.isTrigger = false;
        }
        else
        {
            gun.enabled = true;
            rb.isKinematic = true;
            coll.isTrigger = true;
        }
    }

    private void Update()
    {
        var distanceToPlayer = player.position - transform.position;
        if (!equipped && distanceToPlayer.magnitude <= pickUpRange && Input.GetKeyDown(pickUpKeyCode))
        {
            if (!slotFull)
            {
                PickUp();
            }
            else
            {
                PutInInventory();
            }
        }

        if (equipped && Input.GetKeyDown(dropKeyCode)) Drop();
    }

    private void PutInInventory()
    {
        player.GetComponent<Inventory>().AddItem(gameObject);
        gameObject.transform.SetParent(gunContainer);
        gameObject.SetActive(false);
    }

    private void PickUp()
    {
        equipped = true;
        slotFull = true;

        var inventory = player.GetComponent<Inventory>();
        inventory.AddItem(gameObject);
        inventory.currentWeapon = gameObject;

        transform.SetParent(gunContainer);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(defaultRotation.x, defaultRotation.y, defaultRotation.z);

        rb.isKinematic = true;
        coll.isTrigger = true;

        gun.enabled = true;
    }

    private void Drop()
    {
        equipped = false;
        slotFull = false;

        var inventory = player.GetComponent<Inventory>();
        inventory.RemoveItem(gameObject);
        inventory.currentWeapon = null;

        transform.SetParent(null);

        rb.isKinematic = false;
        coll.isTrigger = false;

        rb.velocity = player.GetComponent<CharacterController>().velocity;

        rb.AddForce(_camera.forward * dropForwardForce, ForceMode.Impulse);
        rb.AddForce(_camera.up * dropForwardForce, ForceMode.Impulse);

        var random = Random.Range(-1f, 1f);
        rb.AddTorque(new Vector3(random, random, random) * 10);

        gun.enabled = false;
    }
}