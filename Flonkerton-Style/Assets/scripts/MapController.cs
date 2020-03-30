using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum StripType {
    Outside,
    Office,
    Wall
}

public class MapController : MonoBehaviour
{
    //Marker for where the strips will start.
    public GameObject MarkerStrip;
    public GameObject OfficeBlank;
    public GameObject OutsideBlank;

    // Load all the strips to auto generate the map.
    private GameObject[] OfficeStrips = new GameObject[6]; //Replace '1' with total # of prefabs, don't forget to name you prefabs like "Prefab0 - Prefab9" etc.
    private GameObject[] OutsideStrips = new GameObject[12]; //Replace '1' with total # of prefabs, don't forget to name you prefabs like "Prefab0 - Prefab9" etc.
    private GameObject[] WallStrips = new GameObject[1]; //Replace '1' with total # of prefabs, don't forget to name you prefabs like "Prefab0 - Prefab9" etc.

    // MAP VARIABLES
    private List<GameObject> MapStrips;

    private int preloadStripCount = 10;

    public int MaxStripSectionLength = 10;
    public int MinStripSectionLength = 3;

    private bool forceBlank = false;

    private int remainingStripSectionLength; //TODO fuck these are long var names..
    private StripType currentSection = StripType.Outside;

    void Start()
    {
        // Load all the strips
        for (int p = 0; p < OfficeStrips.Length; p++) {      //load all the OFFICE strips
            OfficeStrips[p] = Resources.Load("strips/Office/strip-office-" + (p + 1)) as GameObject;
        }
        for (int p = 0; p < OutsideStrips.Length; p++) {     //load all the OUTSIDE strips
            OutsideStrips[p] = Resources.Load("strips/Outside/strip-" + (p + 1)) as GameObject;
        }
        for (int p = 0; p < WallStrips.Length; p++) {        //load all the WALL strips
            WallStrips[p] = Resources.Load("strips/Walls/strip-wall-" + (p + 1)) as GameObject;
        }

        MapStrips = new List<GameObject>();

        remainingStripSectionLength = Random.Range(MinStripSectionLength, MaxStripSectionLength);

        //Load the initial 10 strips.
        InitialGameLoad();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void InitialGameLoad() {
        //REMOVE ALL STRIPS
        var existingStrips = GameObject.FindGameObjectsWithTag("Strip");
        foreach(var s in existingStrips) {
            GameObject.Destroy(s);
        }

        currentSection = StripType.Outside;

        MapStrips.Add(MarkerStrip);

        // PRE-GENERATE
        for (var x = 0; x < preloadStripCount; x++) {
            SpawnNewStrip();
        }

    }

    GameObject GetRandomStrip(StripType stripType) {
        switch (stripType)
        {
            case StripType.Office:
                return OfficeStrips[Random.Range(0, OfficeStrips.Length)];
            case StripType.Outside:
                return OutsideStrips[Random.Range(0, OutsideStrips.Length)];
            case StripType.Wall:
                var x = WallStrips[Random.Range(0, WallStrips.Length)];
                Debug.Log(x);
                return x;
            default:
                Debug.Log("ERROR");
                return new GameObject();
        }
    }


    /// <summary>
    /// This generates a random strip AND updates strip variables used for changing sections. 
    /// This differs from GetRandomStrip as it chooses a strip for the appropriate section but 
    /// GetRandomStrip doesn't update the state variables. 
    /// </summary>
    /// <returns>Generates a random strip of the current section and updates.</returns>
    GameObject GetNextAppropriateStrip() {
        if (remainingStripSectionLength == 0)
        {
            remainingStripSectionLength = Random.Range(MinStripSectionLength, MaxStripSectionLength);
            //Change sections
            currentSection = (currentSection == StripType.Outside ? StripType.Office : StripType.Outside);

            // Used specifically after a wall to avoid props blocking walls.
            forceBlank = true; 

            return GetRandomStrip(StripType.Wall);
        }
        else if (remainingStripSectionLength > 0)
        {
            //Should generate a random strip from the current section it is on.
            remainingStripSectionLength--;

            // Used specifically after a wall to avoid props blocking walls.
            if (forceBlank) {
                forceBlank = false;
                return (currentSection == StripType.Office ? OfficeBlank : OutsideBlank);
            }
        }
        return GetRandomStrip(currentSection);
    }

    //TODO fix this logic to not grab childen, but lazy rn.
    public float GetStripWidth(GameObject strip) {
        //Get floor width. 
        //Note this grabs the first object in the prefab. So the first object must be the floor*.
        Transform grandchildTransform = strip.transform.GetChild(0).GetChild(0) as Transform;
        // Get the x coordinate (width) of the strip's mesh box
        return grandchildTransform.gameObject.GetComponent<Renderer>().bounds.size.x;
    }

    public void SpawnNewStrip()
    {
        var tempStrip = GetNextAppropriateStrip();

        var width = GetStripWidth(tempStrip);

        // Use last strip coordinates to instantiate new strip object
        GameObject lastStrip = MapStrips[MapStrips.Count -1] as GameObject;
        // Clone last strip to instantiate new strip object
        GameObject newStrip = Instantiate(tempStrip,
                                            new Vector3(lastStrip.transform.position.x - width,
                                                            0,
                                                            0),
                                          lastStrip.transform.rotation);
        //TODO MEMORY/STRIP CLEAN-UP

        MapStrips.Add(newStrip);
    }

    public GameObject GetStripAtIndex(int index) {
        //TODO account for offset when cleaning up.
        return MapStrips[index];
    }
}
