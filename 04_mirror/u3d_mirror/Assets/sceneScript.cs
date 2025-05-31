using Mirror;
using UnityEngine;

public class sceneScript : NetworkBehaviour
{
    public TMPro.TextMeshProUGUI canvasStatusText;
    public TMPro.TextMeshProUGUI canvasBulletsText;
    public playerMove plMove;

    [SyncVar(hook = nameof(OnStatusTextChanged))]
    public string statusText;

    void OnStatusTextChanged(string _Old, string _New)
    {
        //called from sync var hook, to update info on screen for all players
        canvasStatusText.text = statusText;
    }

    public void ButtonSendMessage()
    {
        if (plMove != null)
            plMove.CmdSendPlayerMessage();
    }

    public void ammoUI (float _ammo)
    {
        canvasBulletsText.text = "Bullets left " + _ammo.ToString();
    }
}