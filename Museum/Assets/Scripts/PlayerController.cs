using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public LayerMask m_DragLayers;

	[Range (0.0f, 100.0f)]
	public float m_Damping = 1.0f;

	[Range (0.0f, 100.0f)]
	public float m_Frequency = 5.0f;

	public bool m_DrawDragLine = true;
	public Color m_Color = Color.cyan;

	private TargetJoint2D m_TargetJoint;
    public GameObject confettiParticle;
    public Rigidbody2D leftBicep;
    public Rigidbody2D leftForearm;
    public Rigidbody2D rightBicep;
    public Rigidbody2D rightForearm;
    public Rigidbody2D SkeletonLeftBicep;
    public Rigidbody2D SkeletonLeftForearm;
    public Rigidbody2D SkeletonRightBicep;
    public Rigidbody2D SkeletonRightForearm;
    public TMP_Text timerTxt;
    public TMP_Text timerUI;
    public GameObject winText;
    public GameObject loseText;
    public AudioSource audioSource;
    public AudioSource musicSource;
    public float timeLeft;
    public bool timerOn;
    public GameObject introGraphic;
    public GameObject introText;
    public GameObject introText2;
    public GameObject peopleOverlay;
    public GameObject fadeOverlay;
    private bool Playing;
    private Rigidbody2D body;
    private bool fadeIn;
    public float fadeSpeed;
    //Degree Threshold
    public float DT = .2f;
    public AudioClip Music;
    public AudioClip WinSFX;
    public AudioClip LoseSFX;
    public AudioClip IntroSFX;
    public AudioClip ClockTick;

    Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        introText.SetActive(true);
        introText2.SetActive(true);
        introGraphic.SetActive(true);
        fadeOverlay.SetActive(true);
        fadeIn = false;
        timerTxt.gameObject.SetActive(false);
        timerUI.gameObject.SetActive(false);
        winText.SetActive(false);
        loseText.SetActive(false);
        audioSource.PlayOneShot(IntroSFX);
        
    }
    void updateTimer(float currentTime)
    {
        currentTime += 1;
        int seconds = Mathf.FloorToInt(currentTime);
        
        timerTxt.text = seconds.ToString() + " SECONDS";
    }
    //Randomized the rotation of the skeleton's arm bones, then sets the "playing" value to active
    public void PlayMusic(AudioClip music)
    {
        musicSource.Stop();
        musicSource.clip = music;
        musicSource.Play();
        musicSource.loop = true;
    }
    void Begin()
    {
        PlayMusic(Music);
        Debug.Log("Begin!");
        introGraphic.SetActive(false);
        introText2.SetActive(false);
        introText.SetActive(false);
        timerTxt.gameObject.SetActive(true);
        timerUI.gameObject.SetActive(true);
        fadeOverlay.SetActive(false);
        SkeletonLeftBicep.transform.Rotate(0, 0, Random.Range(0, 360));
        SkeletonLeftBicep.constraints = RigidbodyConstraints2D.FreezeRotation;
        SkeletonLeftForearm.transform.Rotate(0, 0, Random.Range(0, 360));
        SkeletonLeftForearm.constraints = RigidbodyConstraints2D.FreezeRotation;
        SkeletonRightBicep.transform.Rotate(0, 0, Random.Range(0, 360));
        SkeletonRightBicep.constraints = RigidbodyConstraints2D.FreezeRotation;
        SkeletonRightForearm.transform.Rotate(0, 0, Random.Range(0, 360));
        SkeletonRightForearm.constraints = RigidbodyConstraints2D.FreezeRotation;
        Playing = true;

    }

    void End()
    {
        musicSource.Stop();
        Playing = false;
        timerOn = false;
        peopleOverlay.SetActive(true);
        fadeIn = true;

        if (m_TargetJoint)
        {
            Destroy (m_TargetJoint);
            m_TargetJoint = null;
        }
        leftBicep.constraints = RigidbodyConstraints2D.FreezeAll;
        leftForearm.constraints = RigidbodyConstraints2D.FreezeAll;
        rightBicep.constraints = RigidbodyConstraints2D.FreezeAll;
        rightForearm.constraints = RigidbodyConstraints2D.FreezeAll;

        float leftBicepStatus = (SkeletonLeftBicep.transform.localRotation.z - leftBicep.transform.localRotation.z);
        float leftForearmStatus = (SkeletonLeftForearm.transform.localRotation.z - leftForearm.transform.localRotation.z);
        float rightBicepStatus = (SkeletonRightBicep.transform.localRotation.z - rightBicep.transform.localRotation.z);
        float rightForearmStatus = (SkeletonRightForearm.transform.localRotation.z - rightForearm.transform.localRotation.z);
        Debug.Log("Left Bicep Difference: " + leftBicepStatus.ToString());
        Debug.Log("Left Forearm Difference: " + leftForearmStatus.ToString());
        Debug.Log("Right Bicep Difference: " + rightBicepStatus.ToString());
        Debug.Log("Right Forearm Difference: " + rightForearmStatus.ToString());
        if((0-DT <= leftBicepStatus && leftBicepStatus <= DT) && (0-DT <= leftForearmStatus && leftForearmStatus <= DT) && (0-DT <= rightBicepStatus && rightBicepStatus <= DT) && (0-DT <= rightForearmStatus && rightForearmStatus <= DT))
        {
        Win();
        }
        else Lose();
    }

    void Win()
    {
        audioSource.PlayOneShot(WinSFX);
        confettiParticle.SetActive(true);
        winText.SetActive(true);
    }

    void Lose()
    {
        audioSource.PlayOneShot(LoseSFX);
        loseText.SetActive(true);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        if(fadeIn==true){
            Color objectColor = peopleOverlay.GetComponent<Renderer>().material.color;
            float fadeAmount = objectColor.a + (fadeSpeed * Time.deltaTime);
            objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
            peopleOverlay.GetComponent<Renderer>().material.color = objectColor;
            if(objectColor.a >= 254)
            {
                fadeIn = false;
            }
        }
        //Sets up the timer, which should be 2 higher than the intended playtime.
        if (timerOn == true)
        {
            //When timer hits 10 seconds
            if (timeLeft <= 10 && timeLeft > 0 && Playing==false)
                Begin();
            if (timeLeft > 0)
            {
                timeLeft -= Time.deltaTime;
                if(Playing==true)
                {
                    updateTimer(timeLeft);
                    if(timeLeft%1==0)
                        audioSource.PlayOneShot(ClockTick);
                }
            }
            //When timer hits 0 seconds
            else
            {
                Debug.Log("Time is up!");
                timeLeft = 0;
                timerTxt.text = timeLeft.ToString() + " SECONDS";
                End();
            }
        }
        //Sets up Object Dragging
        if (Playing==true)
        {
                // Calculate the world position for the mouse.
            var worldPos = Camera.main.ScreenToWorldPoint (Input.mousePosition);

            if (Input.GetMouseButtonDown (0))
            {
                // Fetch the first collider.
                // NOTE: We could do this for multiple colliders.
                var collider = Physics2D.OverlapPoint (worldPos, m_DragLayers);
                if (!collider)
                    return;

                // Fetch the collider body.
                body = collider.attachedRigidbody;
                if (!body)
                    return;
                body.constraints = RigidbodyConstraints2D.None;

                // Add a target joint to the Rigidbody2D GameObject.
                m_TargetJoint = body.gameObject.AddComponent<TargetJoint2D> ();
                m_TargetJoint.dampingRatio = m_Damping;
                m_TargetJoint.frequency = m_Frequency;

                // Attach the anchor to the local-point where we clicked.
                m_TargetJoint.anchor = m_TargetJoint.transform.InverseTransformPoint (worldPos);		
            }
            else if (Input.GetMouseButtonUp (0))
            {
                body.constraints = RigidbodyConstraints2D.FreezeRotation;
                Destroy (m_TargetJoint);
                m_TargetJoint = null;
                return;
            }

            // Update the joint target.
            if (m_TargetJoint)
            {
                m_TargetJoint.target = worldPos;

                // Draw the line between the target and the joint anchor.
                if (m_DrawDragLine)
                    Debug.DrawLine (m_TargetJoint.transform.TransformPoint (m_TargetJoint.anchor), worldPos, m_Color);
            }
        }
    }
}



