using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeckRig : MonoBehaviour
{
    public HalfCirclePath path;

    public void UpdatePostion(float x){
        x= Mathf.Clamp(x,-.75f,.75f);
        transform.position = path.GetPositionAlongPath(x);
    }
}
