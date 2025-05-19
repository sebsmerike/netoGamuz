using UnityEngine;
using Mirror;

public class playerMove : NetworkBehaviour
{
    public float speed = 1.25f;

	// naming for easier debugging
	public override void OnStartClient()
	{
		name = $"Player[{netId}|{(isLocalPlayer ? "local" : "remote")}]";
		Debug.Log("OnStartClient: "+name);
	}

	public override void OnStartServer()
	{
		name = $"Player[{netId}|server]";
		Debug.Log("OnStartServer: "+name);
	}

	void Update()
	{
		//if (!isLocalPlayer) return;

		float h = Input.GetAxis("Horizontal");
		float v = Input.GetAxis("Vertical");

		Vector3 dir = new Vector3(h, 0, v);
		transform.position += dir.normalized * (Time.deltaTime * speed);
	}
}
