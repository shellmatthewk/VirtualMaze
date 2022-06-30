using UnityEngine;
using System.Collections;

public class NonTargetRaycast : MonoBehaviour
{
    public Camera cam;
    //public CheckPoster checkPoster;
    public AudioClip errorClip;
    public CueController cueController;
    private static ExperimentController experimentController;
    private static RewardArea rewardArea;
    private bool isSoundTriggered = false;
    private bool isPosterInView = false;
    private bool isCorrectPoster = false;
    //private float sweepValue = 0f;
    public bool errorFlag = false;
    private bool flag;
    private bool FlagLeft;
    private bool FlagRight;
    private bool FlagStraight;
    private float timer = 1000f;

    //public static string cueImage { get; private set; }

    [SerializeField]
    private float maxDist = 1;

    void Start()
    {
        experimentController = GameObject.FindObjectOfType(typeof(ExperimentController)) as ExperimentController;
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
        if (LevelController.sessionStarted)
        {
            Shoot();
            HintBlink();
        }
        else
        {
            Reset();
        }

        if (timer >= experimentController.rewardAreaErrorTime)
        {
            errorFlag = true; // duration between error sounds long enough
        }
        else { errorFlag = false; } // duration between error sounds not long enough
        Debug.Log(errorFlag);
        // Debug.Log(timer);

        if (FlagLeft || FlagRight || FlagStraight || flag)
        {
            isPosterInView = true; // At least one of the rays is hitting a poster
        }
        else if (!FlagLeft && !FlagRight && !FlagStraight && !flag)
        {
            isPosterInView = false;
            isCorrectPoster = false;
        }

        if (isPosterInView == false)
        {
            isSoundTriggered = false; // Resets error sound flag so that next correct poster detected will trigger error sound
        }
    }

    // Raycasts to check whether rays are colliding with poster
    void Shoot()
    {
        timer += Time.deltaTime;
        //cueImage = checkPoster.GetCueImageName();
        string cueImage = CueImage.cueImage; // Retrieves name of cue image from CueImage

        //Vector3 straightline = cam.transform.forward;
        Vector3 straightline = Quaternion.AngleAxis(0f / 2f, Vector3.up) * cam.transform.forward;
        Vector3 leftline = Quaternion.AngleAxis(-RewardArea.RequiredViewAngle / 2f, Vector3.up) * cam.transform.forward;
        Vector3 rightline = Quaternion.AngleAxis(RewardArea.RequiredViewAngle / 2f, Vector3.up) * cam.transform.forward;
        Vector3 checkleftline = Quaternion.AngleAxis(-(RewardArea.RequiredViewAngle + 5) / 2f, Vector3.up) * cam.transform.forward;
        Vector3 checkrightline = Quaternion.AngleAxis((RewardArea.RequiredViewAngle + 5) / 2f, Vector3.up) * cam.transform.forward;
        straightline.y = 0;
        leftline.y = 0;
        rightline.y = 0;
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

        if (Physics.Raycast(cam.transform.position, straightline, out RaycastHit hitstraight, 500))
        {
            Debug.DrawLine(cam.transform.position, hitstraight.point);
            //Debug.Log(hitstraight.transform.name);
            if (hitstraight.transform.name == "Poster" && hitstraight.distance < maxDist)
            {
                FlagStraight = true;
                string posterImage = hitstraight.transform.GetComponent<Renderer>().material.name;
                //Debug.Log(posterImage);
                string strcheck = cueImage + " (Instance)";
                //Debug.Log(strcheck);
                //Debug.Log(hitstraight.distance);
                if (posterImage == strcheck)
                {
                    //Debug.Log("Correct Poster");
                    isCorrectPoster = true;
                }
                else if (posterImage != strcheck && !isSoundTriggered && !isCorrectPoster)
                {
                    //Debug.Log("Wrong Poster");
                    WrongPoster();
                    isSoundTriggered = true;
                }
            }
            else FlagStraight = false;
        }

        if (Physics.Raycast(cam.transform.position, leftline, out RaycastHit hitleft, 500))
        {
            Debug.DrawLine(cam.transform.position, hitleft.point);
            //Debug.Log(hitleft.transform.name);
            if (hitleft.transform.name == "Poster" && hitleft.distance < maxDist)
            {
                FlagLeft = true;
                string posterImage = hitleft.transform.GetComponent<Renderer>().material.name;
                //Debug.Log(posterImage);
                string strcheck = cueImage + " (Instance)";
                //Debug.Log(strcheck);
                if (posterImage == strcheck)
                {
                    //Debug.Log("Correct Poster");
                    isCorrectPoster = true;
                }
                else if (posterImage != strcheck && !isSoundTriggered && !isCorrectPoster)
                {
                    //Debug.Log("Wrong Poster");
                    WrongPoster();
                    isSoundTriggered = true;
                }
            }
            else FlagLeft = false;
        }

        if (Physics.Raycast(cam.transform.position, rightline, out RaycastHit hitright, 500))
        {
            Debug.DrawLine(cam.transform.position, hitright.point);
            //Debug.Log(hitleft.transform.name);
            if (hitright.transform.name == "Poster" && hitright.distance < maxDist)
            {
                FlagRight = true;
                string posterImage = hitright.transform.GetComponent<Renderer>().material.name;
                //Debug.Log(posterImage);
                string strcheck = cueImage + " (Instance)";
                //Debug.Log(strcheck);
                if (posterImage == strcheck)
                {
                    //Debug.Log("Correct Poster");
                    isCorrectPoster = true;
                }
                else if (posterImage != strcheck && !isSoundTriggered && !isCorrectPoster)
                {
                    //Debug.Log("Wrong Poster");
                    WrongPoster();
                    isSoundTriggered = true;
                }
            }
            else FlagRight = false;
        }


        for (float rayAngleDeviation = 0f; rayAngleDeviation < 100f; rayAngleDeviation += 10f)
        {
            ShootRay(rayAngleDeviation, RewardArea.RequiredViewAngle);
            if (flag) break;
            ShootRay(-rayAngleDeviation, -RewardArea.RequiredViewAngle);
            if (flag) break;
        }

        /************************************************************************/
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
                    else if (posterImage != strcheck && !isSoundTriggered && !isCorrectPoster)
                    {
                        //Debug.Log("Wrong Poster");
                        WrongPoster();
                        isSoundTriggered = true;
                    }
                }
                else flag = false;
            }
        }
        /*************************************************************************/
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
        FlagLeft = false;
        FlagRight = false;
        FlagStraight = false;
        isPosterInView = false;
        isCorrectPoster = false;
    }
}