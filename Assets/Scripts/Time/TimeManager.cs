using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    [Header("Internal Clock")]
    [SerializeField]
    GameTimestamp timestamp;
    public float timeScale = 1.0f;

    [Header ("Day and Night cycle")]
    //The transform of the directional light (sun)
    public Transform sunTransform;
    private float indoorAngle = 40; 

    //List of Objects to inform of changes to the time
    List<ITimeTracker> listeners = new List<ITimeTracker>();

    private void Awake()
    {
        //If there is more than one instance, destroy the extra
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            //Set the static instance to this instance
            Instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        //Initialise the time stamp
        timestamp = new GameTimestamp(0, GameTimestamp.Season.Spring, 1, 6, 0);
        StartCoroutine(TimeUpdate());

    }

    //Load the time from a save 
    public void LoadTime(GameTimestamp timestamp)
    {
        this.timestamp = new GameTimestamp(timestamp);
    }

    IEnumerator TimeUpdate()
    {
        while (true)
        {
            Tick();
            yield return new WaitForSeconds(1 / timeScale);
        }
       
    }
    

    //A tick of the in-game time
    public void Tick()
    {
        timestamp.UpdateClock();

        //Inform each of the listeners of the new time state 
        foreach(ITimeTracker listener in listeners)
        {
            listener.ClockUpdate(timestamp);
        }

        UpdateSunMovement(); 
    }

    public void SkipTime(GameTimestamp timeToSkipTo)
    {
        //Convert to minutes
        int timeToSkipInMinutes = GameTimestamp.TimestampInMinutes(timeToSkipTo);
        Debug.Log("Time to skip to:" + timeToSkipInMinutes);
        int timeNowInMinutes = GameTimestamp.TimestampInMinutes(timestamp);
        Debug.Log("Time now: " + timeNowInMinutes);

        int differenceInMinutes = timeToSkipInMinutes - timeNowInMinutes;
        Debug.Log(differenceInMinutes + " minutes will be advanced");

        //Check if the timestamp to skip to has already been reached
        if (differenceInMinutes <= 0) return; 

        for(int i = 0; i<differenceInMinutes; i++)
        {
            Tick(); 
        }
    }

    //Day and night cycle
    void UpdateSunMovement()
    {

        //Disable Day and Night cycle if indoors
        if (SceneTransitionManager.Instance.CurrentlyIndoor())
        {
            sunTransform.eulerAngles = new Vector3(indoorAngle, 0, 0);
            return; 
        }

        //Convert the current time to minutes
        int timeInMinutes = GameTimestamp.HoursToMinutes(timestamp.hour) + timestamp.minute;

        //Sun moves 15 degrees in an hour
        //.25 degrees in a minute
        //At midnight (0:00), the angle of the sun should be -90
        float sunAngle = .25f * timeInMinutes - 90;

        //Apply the angle to the directional light
        sunTransform.eulerAngles = new Vector3(sunAngle, 0, 0);
    }

    //Get the timestamp
    public GameTimestamp GetGameTimestamp()
    {
        //Return a cloned instance
        return new GameTimestamp(timestamp);
    }


    //Handling Listeners

    //Add the object to the list of listeners
    public void RegisterTracker(ITimeTracker listener)
    {
        listeners.Add(listener);
    }

    //Remove the object from the list of listeners
    public void UnregisterTracker(ITimeTracker listener)
    {
        listeners.Remove(listener);
    }


}
