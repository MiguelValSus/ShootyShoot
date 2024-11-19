using UnityEngine;

public class AnimationEventsReceiver : MonoBehaviour
{
    [Header("Receiver")]
    public GameObject Receiver;
    [Header("Debug")]
    public bool ForceDestroy;

    #region Events
    public void OnAnimationEvent(AnimationEvent animationEvent)
    {
        // Do something
    }

    public void DisableReceiver(AnimationEvent animationEvent)
    {
        Receiver.SetActive(false);
        gameObject.SetActive(false);

        if (ForceDestroy) DestroyReceiver(animationEvent);
    }

    public void DestroyReceiver(AnimationEvent animationEvent)
    {
        Destroy(Receiver);
    }
    #endregion
}
