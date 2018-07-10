using UnityEngine;
using System.Collections;

public class BuildingMoverScript : MonoBehaviour 
{

    public Vector3[] move = new[] { new Vector3(7.5f, 4.4f, 4.33f), new Vector3(15f, 4.4f, 8.66f) };
    public int option;

    public void MoveForward()
    {
        Vector3 p = this.transform.position;
        p.z += move[option].z;
        this.transform.position = p;

    }
    public void MoveBackward()
    {
        Vector3 p = this.transform.position;
        p.z -= move[option].z;
        this.transform.position = p;

    }
   
    public void MoveLeft()
    {
        Vector3 p = this.transform.position;
        p.x -= move[option].x;
        this.transform.position = p;

    }

    public void MoveRight()
    {
        Vector3 p = this.transform.position;
        p.x += move[option].x;
        this.transform.position = p;
    }


    public void RotateClockwise()
    {
        Vector3 p = this.transform.rotation.eulerAngles;
        p.y += 60;
        this.transform.rotation = Quaternion.Euler(p);
    }

    public void RotateCounter()
    {
        Vector3 p = this.transform.rotation.eulerAngles;
        p.y -= 60;

        this.transform.rotation = Quaternion.Euler(p);
    }

    public void MoveUp()
    {
        Vector3 p = this.transform.position;
        p.y += move[option].y;
        this.transform.position = p;

    }

    public void MoveDown()
    {
        Vector3 p = this.transform.position;
        p.y -= move[option].y;
        this.transform.position = p;

    }

}