using UnityEngine;
using System.Collections;

public class NonTargetRaycast : MonoBehaviour
{
    public Camera cam;
    //public CheckPoster checkPoster;
    public AudioClip errorClip;
    public CueController cueController;
    private static ExperimentController experimentController;
    private static RewardsController rewardsController;
    private static WrongRewardAreaError wrongRewardAreaError;
    private bool isSoundTriggered = false;
    private bool isPosterInView = false;
    private bool isCorrectPoster = false;
    private bool tempWrongRewardAreaFlag = false;
    //private float sweepValue = 0f;
    public bool errorFlag = true;
    private bool flag;
    private float timer = 1000f;

    //public static string cueImage { get; private set; }

    [SerializeField]
    public float maxDist = 1f;
    public float maxAngle = 97f;

    void Start()
    {
        experimentController = GameObject.FindObjectOfType(typeof(ExperimentController)) as ExperimentController;
        rewardsController = GameObject.FindObjectOfType(typeof(RewardsController)) as RewardsController;
        wrongRewardAreaError = GameObject.FindObjectOfType(typeof(WrongRewardAreaError)) as WrongRewardAreaError;
    }

    void Update()
    {
        //For Testing:
        /*if (Input.GetKeyDown("space"))
        {
            Shoot();
        }*/

        /*for (sweepValue = -90; sweepValue <= 90; sweepValue += 10)
        {
            Shoot();
        }*/

        // Checks if a session is currently running
        if (LevelController.sessionStarted && rewardsController.enableNonTargetRaycast)
        {
            isErrorTriggered();
            Shoot();
            HintBlink();
        }
        else
        {
            Reset();
        }

        if (timer >= rewardsController.rewardAreaErrorTime)
        {
            errorFlag = true; // duration between error sounds long enough
        }
        else { errorFlag = false; } // duration between error sounds not long enough
        // Debug.Log(errorFlag);
        // Debug.Log(timer);

        if (flag) //(FlagLeft || FlagRight || FlagStraight || flag)
        {
            isPosterInView = true; // At least one of the rays is hitting a poster
        }
        else if (!flag) //(!FlagLeft && !FlagRight && !FlagStraight && !flag)
        {
            isPosterInView = false;
            isCorrectPoster = false;
        }

        if (isPosterInView == false)
        {
            isSoundTriggered = false; // Resets error sound flag so that next correct poster detected will trigger error sound
        }
    }

    /// <summary>
	/// if function resets the timer when any of the reward zone error sound is triggered. This method is used to bypass the need
	/// to keep track of every reward zone.
	/// This function only resets timer when the isSoundTriggered is changed in WrongRewardArea script from false to true, and not true to false or remains true
	/// </summary>
    private void isErrorTriggered()
    {
        if (wrongRewardAreaError.isSoundTriggered == true && tempWrongRewardAreaFlag == false)
        {
            timer = 0f;
        }
        tempWrongRewardAreaFlag = wrongRewardAreaError.isSoundTriggered; // stores isSoundTrigger value to carry to the next iteration
    }

    // Raycasts to check whether rays are colliding with poster
    void Shoot()
    {
        timer += Time.deltaTime;
        //cueImage = checkPoster.GetCueImageName();
        string cueImage = CueImage.cueImage; // Retrieves name of cue image from CueImage

        //Vector3 straightline = cam.transform.forward;
        Vector3 checkleftline = Quaternion.AngleAxis(-(maxAngle + 5) / 2f, Vector3.up) * cam.transform.forward;
        Vector3 checkrightline = Quaternion.AngleAxis((maxAngle + 5) / 2f, Vector3.up) * cam.transform.forward;
        checkleftline.y = 0;
        checkrightline.y = 0;
        //Debug.Log(straightline);
        //Debug.Log(isPosterInView);

        if (Physics.Raycast(cam.transform.position, checkleftline, out RaycastHit checkleft, 500))
        {
            Debug.DrawLine(cam.transform.position, checkleft.point, Color.green);
            if (checkleft.transform.name == "Poster")
            {
                isPosterInView = false;
            }
        }

        if (Physics.Raycast(cam.transform.position, checkrightline, out RaycastHit checkright, 500))
        {
            Debug.DrawLine(cam.transform.position, checkright.point, Color.green);
            if (checkright.transform.name == "Poster")
            {
                isPosterInView = false;
            }
        }

        /// <summary>
        /// this for loop shoots all the rays from 0 degrees from midline to 100 degrees away from midline
        /// in both directions at 10 degrees interval each.
        /// </summary>
        /// <param name="other"></param>
        for (float rayAngleDeviation = 0f; rayAngleDeviation < 100f; rayAngleDeviation += 10f)
        {
            ShootRay(rayAngleDeviation, maxAngle);
            if (flag) break;
            // the flag is triggered when a poster is in view. prevents extra rays from being created unnecessarily
            ShootRay(-rayAngleDeviation, -maxAngle);
            if (flag) break;
        }

        void ShootRay(float rayAngleDeviation, float maxAngle)
        {
            Vector3 line = Quaternion.AngleAxis((maxAngle - rayAngleDeviation) / 2f, Vector3.up) * cam.transform.forward;
            line.y = 0;
            if (Physics.Raycast(cam.transform.position, line, out RaycastHit hit, 500))
            {
                Debug.DrawLine(cam.transform.position, hit.point);
                if (hit.transform.name == "Poster" && hit.distance < maxDist)
                {
                    flag = true;
                    string posterImage = hit.transform.GetComponent<Renderer>().material.name;
                    string strcheck = cueImage + " (Instance)";
                    if (posterImage == strcheck)
                    {
                        //Debug.Log("Correct Poster");
                        isCorrectPoster = true;
                    }
                    else if (posterImage != strcheck && !isSoundTriggered && !isCorrectPoster && errorFlag)
                    {
                        //Debug.Log("Wrong Poster");
                        WrongPoster();
                        isSoundTriggered = true;
                    }
                }
                else flag = false;
            }
        }
    }


    private void WrongPoster()
    {
        Debug.Log("Raycast Ping");
        PlayerAudio.instance.PlayErrorClip();
        timer = 0f; // For resetting the blinking timer
    }

    // Number and duration of blinks
    int numBlinks = 2;
    float overallBlinkDuration = 0.5f;
    
    private void HintBlink()
    {
        for (int i = 0; i < numBlinks; i++)
        {
            if (timer >= (i * overallBlinkDuration)  && timer < (((2 * i) + 1) * overallBlinkDuration / 2))
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
        // if (experimentController.disableHint) { cueController.HideHint(); }
    }

    private void Reset()
    {
        timer = 1000f;
        isSoundTriggered = false;
        flag = false;
        isPosterInView = false;
        isCorrectPoster = false;
        tempWrongRewardAreaFlag = false;
    }
}