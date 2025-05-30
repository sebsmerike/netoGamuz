using UnityEngine;
using Mirror;
using UnityEngine.UIElements;

public class playerMove : NetworkBehaviour
{
    float speed = 1.25f;
    // public Material mat;

    public GameObject floating;
    public TextMesh txtName;

    private Material myMaterial;

    [SyncVar(hook = nameof(OnNameChange))]
    public string playerName;

    void OnNameChange(string _old, string _new)
    {
        txtName.text = playerName;
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

        Camera.main.transform.SetParent(transform);
        Camera.main.transform.localPosition = new Vector3(0, 0, 0);

        string _name = "player_" + Random.Range(100, 999);
        Color _color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));

        CmdSetupPlayer(_name, _color);
    }

    [Command]
    public void CmdSetupPlayer (string newName, Color newColor)
    {
        playerName = newName;
        playerColor = newColor;
    }

    void Update()
    {
        if (!isLocalPlayer)
        {
            floating.transform.LookAt(Camera.main.transform);
            return;
        }

        float moveX = Input.GetAxis("Horizontal") * Time.deltaTime * 110.0f;
        float moveZ = Input.GetAxis("Vertical") * Time.deltaTime * 4.0f;

        transform.Rotate(0, moveX, 0);
        transform.Translate(0, 0, moveZ);

    }
}
