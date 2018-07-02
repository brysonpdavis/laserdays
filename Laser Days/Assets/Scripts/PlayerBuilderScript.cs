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
}