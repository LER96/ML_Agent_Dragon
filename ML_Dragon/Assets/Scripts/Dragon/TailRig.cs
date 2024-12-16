using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailRig : MonoBehaviour
{
    public HalfCirclePath path;

    public void UpdatePostion(float x){
        //x= Mathf.Clamp(x,-.7f,.7f);
        transform.position = path.GetPositionAlongPath(x);
    }
}
