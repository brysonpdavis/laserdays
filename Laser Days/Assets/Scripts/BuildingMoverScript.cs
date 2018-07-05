using UnityEngine;
using System.Collections;

public class BuildingMoverScript : MonoBehaviour 
{
    public Vector3 move = new Vector3(9f, 4f, 5.196f);

    public void MoveForward()
    {
        Vector3 p = this.transform.position;
        p.z += move.z;
        this.transform.position = p;

    }
    public void MoveBackward()
    {
        Vector3 p = this.transform.position;
        p.z -= move.z;
        this.transform.position = p;

    }
   
    public void MoveLeft()
    {
        Vector3 p = this.transform.position;
        p.x -= move.x;
        this.transform.position = p;

    }

    public void MoveRight()
    {
        Vector3 p = this.transform.position;
        p.x += move.x;
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
        p.y += move.y;
        this.transform.position = p;

    }

    public void MoveDown()
    {
        Vector3 p = this.transform.position;
        p.y -= move.y;
        this.transform.position = p;

    }

}