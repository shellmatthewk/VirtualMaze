using UnityEngine;

public class WrongRewardAreaError : MonoBehaviour
{
    //public CheckPoster checkPoster;
    public AudioClip errorClip;
    public RewardArea rewardArea;
    private static CueController cueController;
    private static ExperimentController experimentController;
    private static NonTargetRaycast nonTargetRaycast;
    private float timer = 1000f;
    private bool isSoundTriggered = false;

    // Number and duration of blinks
    int numBlinks = 2;
    private float overallBlinkDuration = 0.5f;

    void Start()
    {
        cueController = GameObject.FindObjectOfType(typeof(CueController)) as CueController;
        experimentController = GameObject.FindObjectOfType(typeof(ExperimentController)) as ExperimentController;
        nonTargetRaycast = GameObject.FindObjectOfType(typeof(NonTargetRaycast)) as NonTargetRaycast;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        HintBlink();

        if (!LevelController.sessionStarted)
        {
            Reset();
        }
    }

    
    private void OnTriggerEnter(Collider other)
    {
        if (LevelController.sessionStarted && !isSoundTriggered)
        {
            string areaPosterImage = rewardArea.cueImage.name;
            //Debug.Log(areaPosterImage);
            string cueImage = CueImage.cueImage;
            //Debug.Log(cueImage);
            if (areaPosterImage != cueImage && experimentController.enableRewardAreaError && nonTargetRaycast.errorFlag.ToString() == "True")
            // global variable nonTargetRaycast.errorFlag does not directly interact with scripts unless used as string
            {
                Vector3 direction = rewardArea.transform.position - other.transform.position;
                direction.y = 0;
                float angle = Vector3.Angle(direction, other.transform.forward);
                float distance = Vector3.Magnitude(direction);
                if (angle < RewardArea.RequiredViewAngle * 0.5f)
                {
                    if (distance <= RewardArea.ProximityDistance)
                    {
                        Debug.Log("TriggerEnter Ping");
                        PlayerAudio.instance.PlayErrorClip();
                        timer = 0f;
                        isSoundTriggered = true;
                    }
                    
                }
            }
        }
    }
    /*
    private void OnTriggerStay(Collider other)
    {
        // Debug.Log("Disable Hint: " + experimentController.disableHint);
        if (LevelController.sessionStarted && experimentController.disableHint
        && !isSoundTriggered && !experimentController.enableRewardAreaError)
        {
            string areaPosterImage = rewardArea.cueImage.name;
            string cueImage = CueImage.cueImage;
            if (areaPosterImage != cueImage)
            {
                Vector3 direction = rewardArea.transform.position - other.transform.position;
                direction.y = 0;
                float angle = Vector3.Angle(direction, other.transform.forward);
                float distance = Vector3.Magnitude(direction);

                if (angle < RewardArea.RequiredViewAngle * 0.5f)
                {
                    if (distance <= RewardArea.ProximityDistance)
                    {
                        Debug.Log("TriggerStay Ping");
                        PlayerAudio.instance.PlayErrorClip();
                        timer = 0f;
                        isSoundTriggered = true;
                    }
                }
            }
        }
    }
    */

    private void OnTriggerExit(Collider other)
    {
        isSoundTriggered = false;
    }

    private void HintBlink()
    {
        for (int i = 0; i < numBlinks; i++)
        {
            if (timer >= (i * overallBlinkDuration) && timer < (((2 * i) + 1) * overallBlinkDuration / 2))
            {
                if (experimentController.disableHint) { cueController.ShowHint(); }
                else if (!experimentController.disableHint) { cueController.HideHint(); }
            }
            if (timer >= (((2 * i) + 1) * overallBlinkDuration / 2) && timer < ((i + 1) * overallBlinkDuration))
            {
                if (experimentController.disableHint) { cueController.HideHint(); } // if disableHint is true, end with hide hint
                else if (!experimentController.disableHint) { cueController.ShowHint(); } // if disableHint is false, end with show hint
            }
        }
    }

    private void Reset()
    {
        timer = 1000f;
    }
}
