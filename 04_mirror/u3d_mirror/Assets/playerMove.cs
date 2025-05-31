using UnityEngine;
using Mirror;
using UnityEngine.UIElements;

public class playerMove : NetworkBehaviour
{
    sceneScript scScript;

    public GameObject floating;
    public TextMesh txtName;
    public TextMesh txtHealth;

    private Material myMaterial;

    private Weapon activeWeapon;
    private float weaponCooldownTime;

    private int selectedWeaponLocal = 0;
    public GameObject[] weapons;

    [SyncVar(hook = nameof(OnWeaponChanged))]
    public int activeWeaponSync = 0;
    void OnWeaponChanged(int _old, int _new)
    {
        weapons[_old].SetActive(false);
        weapons[_new].SetActive(true);

        // V2
        activeWeapon = weapons[activeWeaponSync].GetComponent<Weapon>();
        if (isLocalPlayer)
        {
            scScript.ammoUI(activeWeapon.ammo);
        }
    }

    [Command]
    public void CmdChangeActiveWeapon(int newIndex)
    {
        activeWeaponSync = newIndex;
    }


    [SyncVar(hook = nameof(OnNameChange))]
    public string playerName;

    void OnNameChange(string _old, string _new)
    {
        txtName.text = playerName;
    }


    [SyncVar(hook = nameof(OnHealthChanged))]
    public int healthSync;
    void OnHealthChanged(int _old, int _new)
    {
        txtHealth.text = healthSync.ToString();

        if (healthSync > 70)
        {
            txtHealth.color = Color.green;
        }else if (healthSync > 70)
        {
            txtHealth.color = Color.yellow;
        }
        else if (healthSync > 30)
        {
            txtHealth.color = Color.yellow;
        }
        else if (healthSync <= 0)
        {
            Debug.LogError("You're dead...");
        }
        
    }

    [SyncVar(hook = nameof(OnColorChange))]
    public Color playerColor;

    void OnColorChange(Color _old, Color _new)
    {
        myMaterial = new Material(GetComponent<Renderer>().material);
        myMaterial.color = _new;
        GetComponent<Renderer>().material = myMaterial;

        txtName.color = _new;
    }

    public void Awake()
    {
        scScript = GameObject.FindFirstObjectByType<sceneScript>();

        foreach (var item in weapons)
        {
            if (item != null) item.SetActive(false);
        }

        activeWeapon = weapons[selectedWeaponLocal].GetComponent<Weapon>();
        activeWeapon.gameObject.SetActive(true);

        if (scScript)
            scScript.ammoUI(activeWeapon.ammo);
    }

    public void Start()
    {
        //mat.color = Color.gray;
    }

    // naming for easier debugging
    public override void OnStartClient()
    {
        name = $"Player[{netId}|{(isLocalPlayer ? "local" : "remote")}]";
        Debug.Log("OnStartClient: " + name);
    }

    public override void OnStartServer()
    {
        name = $"Player[{netId}|server]";
        Debug.Log("OnStartServer: " + name);
    }

    public override void OnStartLocalPlayer()
    {
        //base.OnStartLocalPlayer();

        scScript.plMove = this;

        Camera.main.transform.SetParent(transform);
        Camera.main.transform.localPosition = new Vector3(0, 0, 0);

        string _name = "player_" + Random.Range(100, 999);
        Color _color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));

        CmdSetupPlayer(_name, _color, 100);
    }

    [Command]
    public void CmdSetupPlayer(string newName, Color newColor, int newHealth)
    {
        playerName = newName;
        playerColor = newColor;
        healthSync = newHealth;

        scScript.statusText = $"{playerName} joined.";
    }

    [Command]
    public void CmdUpdateHealth (int _health)
    {
        healthSync = _health;
    }

    [Command]
    public void CmdSendPlayerMessage()
    {
        if (scScript)
            scScript.statusText = $"{playerName} clicked button {Random.Range(10, 99)}";
    }

    [Command]
    void CmdShootRay()
    {
        RcpFireWeapon();
    }

    [ClientRpc]
    void RcpFireWeapon ()
    {
        GameObject bullet = Instantiate(activeWeapon.bullet, activeWeapon.firePos.position, activeWeapon.firePos.rotation);
        bullet.GetComponent<Rigidbody>().linearVelocity = bullet.transform.forward * activeWeapon.speed;
        Destroy(bullet, activeWeapon.life);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isLocalPlayer)
        {
            Debug.Log("OnTriggerEnter");

            if (other.gameObject.tag == "bullet")
            {
                Debug.Log("get trigger damage");
                
                CmdUpdateHealth(healthSync - 40);
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (isLocalPlayer)
        {
            Debug.Log("OnCollisionEnter");

            if (collision.gameObject.tag == "bullet")
            {
                Debug.Log("get collision damage");
            }
        }
    }

    void Update()
    {
        if (!isLocalPlayer)
        {
            floating.transform.LookAt(Camera.main.transform);
            return;
        }
        // move
        float moveX = Input.GetAxis("Horizontal") * Time.deltaTime * 110.0f;
        float moveZ = Input.GetAxis("Vertical") * Time.deltaTime * 4.0f;

        transform.Rotate(0, moveX, 0);
        transform.Translate(0, 0, moveZ);
        
        if(Input.GetButtonDown("Fire2"))
        {
            selectedWeaponLocal += 1;
            if(selectedWeaponLocal >= weapons.Length)
            {
                selectedWeaponLocal = 0;
            }

            CmdChangeActiveWeapon(selectedWeaponLocal);
        }

        if(Input.GetButtonDown("Fire1"))
        {
            if( activeWeapon && Time.time > weaponCooldownTime &&activeWeapon.ammo > 0)
            {
                weaponCooldownTime = Time.time + activeWeapon.cooldown;
                activeWeapon.ammo -= 1;
                scScript.ammoUI(activeWeapon.ammo);
                CmdShootRay();
            }
        }

    }
}
