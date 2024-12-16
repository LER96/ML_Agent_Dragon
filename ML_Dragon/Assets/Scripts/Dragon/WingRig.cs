using UnityEngine;
using DG.Tweening; // Make sure you have DOTween installed and using this namespace
using System.Threading.Tasks;
public class WingRig : MonoBehaviour
{
    public Transform target;

    public Transform flapUp;
    public Transform flapDown;

    public float flapDuration = 0.5f; // Time it takes to go from Up to Down and back to Up

    public bool isReady = true;
    public async void Flap()
    {
        isReady = false;
        // Ensure target starts at the flapUp position and rotation
        target.position = flapUp.position;
        target.rotation = flapUp.rotation;

        // Sequence to control the animation flow
        Sequence flapSequence = DOTween.Sequence();

        // First, move and rotate from flapUp to flapDown
        flapSequence.Append(target.DOMove(flapDown.position, flapDuration / 2).SetEase(Ease.InOutSine));
        flapSequence.Join(target.DORotateQuaternion(flapDown.rotation, flapDuration / 2).SetEase(Ease.InOutSine));

        // Then, move and rotate back from flapDown to flapUp
        flapSequence.Append(target.DOMove(flapUp.position, flapDuration / 2).SetEase(Ease.InOutSine));
        flapSequence.Join(target.DORotateQuaternion(flapUp.rotation, flapDuration / 2).SetEase(Ease.InOutSine));

        await flapSequence.AsyncWaitForCompletion();
        isReady = true;
        // Optionally, you can loop the flap motion if desired
        // flapSequence.SetLoops(-1, LoopType.Yoyo);
    }
}
