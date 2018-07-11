using UnityEngine;
using System.Collections;

public class PlayerBuilderScript : MonoBehaviour 
{
    public Transform Player;
    public Transform[] spawnPoint;
    public int spawnSelect;


    public void BuildPlayer(int chooser)
    {

        Player.position = spawnPoint[chooser].position;


    }
    public void ResetCharge()
    {

        if (Player.tag == "Player")
        {
            int resetCharge = Player.GetComponentInParent<PlayerCharge>().maxCharge;
            Player.GetComponent<PlayerCharge>().currentCharge = resetCharge;
            Player.GetComponent<PlayerCharge>().chargeValue.text = resetCharge.ToString();
            Player.GetComponent<PlayerCharge>().potentialCharge = resetCharge;
        }
    }
}