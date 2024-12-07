using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class CubeGame : MonoBehaviour
{
    //someGameObject = GameObject.Find ("GameObjectName");
    public GameObject[] cubes; // Assign your 8 cubes here
    public GameObject[] correctPositions; // Assign the correct positions here
    public GameObject arrowPrefab; //= DirectionArrow; // Assign a prefab for the arrow indicator
    public GameObject currentArrow;
    public GameObject currentArrow1;
    public GameObject currentArrow2;

    public bool[] cubeCorrect = new bool[8]; // Array to check completion status of the game
    public bool[] refCorrect = new bool[8]; // reference for completion
    public bool completionStatusChanged = false;


    public GameObject targetObject; // The object above which the arrow will float

    private GameObject arrowInstance;
    public float floatHeight = 0.2f; // Height above the object
    public float rotationSpeed = 50.0f; // Speed of rotation

    public int cubeNumber; // Assign this in the inspector, representing the cube's number (1-8)

    public bool won = false;

    float distance = 0.00f;

    public GameObject cube; // Assign your cube object here
    public GameObject tile; // Assign your tile object here
    public float proximityThreshold = 0.01f; // Distance threshold



    public GameObject objectToHighlight; // Assign the cube object in the inspector
    public Color highlightColor = Color.yellow; // The color used for highlighting
    private Color originalColor;
    private Renderer objectRenderer;



    public Transform targetObjectTransform; // Assign the game object to be moved
    public Vector3 targetPosition = new Vector3(1.0f, 1.0f, 1.0f); // Assign the target position for the object
    public float speed = 1.0f; // Speed at which the object is moved

    private bool isMoving = false;
    private float startTime;
    public Vector3 startPosition = new Vector3(0.0f, 0.0f, 0.0f);



    public float TimeLeft;
    public bool TimerOn = false;

    //public Text TimerTxt;
    public TMP_Text TimerTxt;

    public int duration = 60;
    public int timeRemaining;
    public bool isCountingDown = false;

    public float idleTimer = 5.00f;
    public int gazeTimer = 20;

    public float aiTimer = 10.0f;

    //public GameObject movedObject;
    //public Vector3 movedCoordinatesOld = cubes[0];
    //public Vector3 movedCoordinatesNew;

    void Start()
    {
        Debug.Log("Start started");
        //StartCoroutine(waiter());



        // Initialize game setup, position cubes, etc.
        //if (targetObject != null && arrowPrefab != null)
        //{
        //    // Instantiate the arrow above the target object
        //    Vector3 arrowPosition = targetObject.transform.position + new Vector3(0, floatHeight, 0);
        //    arrowInstance = Instantiate(arrowPrefab, arrowPosition, new Quaternion(0, 0, 0, 0), targetObject.transform);
        //}


        // Ensure the object has a Renderer component
        if (objectToHighlight != null)
        {
            objectRenderer = objectToHighlight.GetComponent<Renderer>();
            if (objectRenderer == null)
            {
                Debug.LogError("The object to highlight does not have a Renderer component.");
                return;
            }

            // Save the original color of the object
            originalColor = objectRenderer.material.color;
        }

        TimerOn = true;
        
    }

    void Update()
    {
        // Check if player has moved any even-numbered cubes
        for (int i = 0; i < cubes.Length; i++)
        {
            UpdateCompletionStatus(cubes[i], correctPositions[i], i);

            if (i % 2 != 0) // Check for even numbered cubes (Player's turn) (the cubes are odd within the array indexing)
            {
                if (IsCubeInWrongPosition(cubes[i], correctPositions[i]))
                {
                    objectToHighlight = cubes[i];
                    Highlight(cubes[i]);
                    AskForHelp(cubes[i]);
                }
            }
            else
            {
                if (aiTimer < 1.0f)
                {
                    for (int j = 0; j < cubeCorrect.Length; j++)
                    {
                        if (cubeCorrect[j] == false && j % 2 == 0)
                        {
                            targetObject = cubes[j];
                            MoveAICube(cubes[j], correctPositions[j], j);
                            aiTimer = 10.0f;
                            break;
                        }
                    }
                    //targetObject = cubes[i];
                    //MoveAICube(cubes[i], correctPositions[i], i);
                    //aiTimer = 10.0f;
                }
                
            }
        }
        if (completionStatusChanged == true)
        {
            //Debug.Log("Completion status updated, updating reference correct");
            //refCorrect = cubeCorrect;
            completionStatusChanged = false;
            //idleTimer = 60;
        }

        if (arrowInstance != null)
        {
            // Rotate the arrow around its up axis
            arrowInstance.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }


        if (TimerOn)
        {
            if (TimeLeft > 0)
            {
                TimeLeft -= Time.deltaTime;
                updateTimer(TimeLeft);
                idleTimer -= Time.deltaTime;
                aiTimer -= Time.deltaTime;
                //Debug.Log(idleTimer);
            }
            else
            {
                Debug.Log("Time is UP!");
                TimeLeft = 0;
                TimerOn = false;
            }
        }

        if (idleTimer < 1)
        {
            Debug.Log("Idle timer is up! Showing HINT now.");
            showHint();
            idleTimer = 5;
        }

        won = checkForWin();
        if (won == true) {
            Debug.Log("YOU WON!!!!!!!");
            //yield return new WaitForSeconds(60);
            //StartCoroutine(waiter());
        }
    }

    IEnumerator waiter()
    {
        while (true)
        {
            Debug.Log("Reached waiter coroutine");
            //Wait for 60 seconds
            yield return new WaitForSeconds(6000000);
        }
    }

    bool showHint()
    {
        for (int i = 0; i < cubeCorrect.Length; i++)
        {
            if (!cubeCorrect[i]) {
                Highlight(cubes[i]);
                //Highlight(correctPositions[i]);
                ShowArrow(i);
                //ShowArrow(i);
                break;
                //return true;
            }
        }
        return false;
    }
    bool IsCubeInWrongPosition(GameObject cube, GameObject correctPosition)
    {

        return cube.transform.position != correctPosition.transform.position;
        // Compare cube's current position with correctPositions[index]
    }

        /*
    void HighlightCube(GameObject cube)
    {
        //renderer.material.color = Color.yellow;
        // Implement logic to highlight the cube
    }
        */

    void AskForHelp(GameObject cube)
    {

        // Use speech recognition or other input to check if the player says "yes"
        // If yes, call ShowArrow(cube)
    }

    bool ShowArrow(int i)
    {
        //if (targetObject != null && arrowPrefab != null)
            //{
            //    // Instantiate the arrow above the target object
            //    Vector3 arrowPosition = targetObject.transform.position + new Vector3(0, floatHeight, 0);
            //    arrowInstance = Instantiate(arrowPrefab, arrowPosition, new Quaternion(0, 0, 0, 0), targetObject.transform);
            //}

        if (currentArrow1 != null)
            {
                Destroy(currentArrow1);
            }

        if (currentArrow2 != null)
            {
                Destroy(currentArrow2);
            }

        if (arrowInstance != null)
        {
            Destroy(arrowInstance);
        }

        // Instantiate the arrow above the correct position
        //int correctIndex = GetCorrectPositionIndex(cube);
        //Vector3 arrowPosition1 = cube.transform.position + Vector3.up * 0.2f;
        //Vector3 arrowPosition2 = correctPositions[i].transform.position + Vector3.up * 0.2f;
        Vector3 arrowPosition2 = correctPositions[i].transform.position + new Vector3(0, floatHeight, 0);
        //Vector3 arrowPosition = correctPositions[correctIndex].transform.position + Vector3.up * 0.2f; // 1.0f is the height above the cube
        //currentArrow1 = Instantiate(arrowPrefab, arrowPosition, new Quaternion(0, 0, 0, 0), cube.transform);
        arrowInstance = Instantiate(arrowPrefab, arrowPosition2, new Quaternion(0, 0, 0, 0), correctPositions[i].transform);
        return true;
    }


    // Call this method with the space's number to check if it's the correct placement
    public bool CheckCorrectPlacement(int spaceNumber)
    {
        return cubeNumber == spaceNumber;
    }


    private bool DoNamesEndWithSameNumber(string name1, string name2)
    {
        //Debug.Log("Checking object numbers now");
        // Extract the last character of each name
        if (name1.Length > 0 && name2.Length > 0)
        {
            char lastChar1 = name1[name1.Length - 1];
            //Debug.Log(lastChar1);
            char lastChar2 = name2[name2.Length - 1];
            //Debug.Log(lastChar2);

            // Check if both characters are digits and are equal
            return char.IsDigit(lastChar1) && char.IsDigit(lastChar2) && lastChar1 == lastChar2;
        }
        return false;
    }

    private void HandleNameMatch()
    {
        //Debug.Log("Cube and tile names end with the same number!");
    }

    public void UpdateCompletionStatus(GameObject cube, GameObject correctPosition, int i)
    {
        for (int k = 0; k < cubes.Length; k++)
        {
            for (int j = 0; j < correctPositions.Length; j++)
            {
                distance = Vector3.Distance(cubes[k].transform.position, correctPositions[j].transform.position);
                if (distance < proximityThreshold)
                {
                    completionStatusChanged = true;
                    cubes[k].transform.position = correctPositions[j].transform.position;

                    // We want the cube to stop moving and face the participant when placed in the correct slot
                    Rigidbody cubeRigid;
                    cubeRigid = cubes[k].GetComponent<Rigidbody>();

                    cubeRigid.isKinematic = false;
                    cubes[k].transform.eulerAngles = new Vector3(0, 0, 0);
                    cubeRigid.constraints = RigidbodyConstraints.FreezeRotation;
                    //cubeRigid.constraints = RigidbodyConstraints.FreezePosition;
                    //Debug.Log("Objects are within proximity threshold");
                    // If within threshold, check if names end with the same number
                }
            }
        }

            // Check the distance between the cube and the tile
            distance = Vector3.Distance(cube.transform.position, correctPosition.transform.position);
        //Debug.Log("Distance between cube and tile is:  ");
        //Debug.Log(distance);
        if (distance < proximityThreshold)
        {
            cube.transform.position = correctPosition.transform.position;

            // We want the cube to stop moving and face the participant when placed in the correct slot
            Rigidbody cubeRigid;
            cubeRigid = cube.GetComponent<Rigidbody>();

            cubeRigid.isKinematic = false;
            cube.transform.eulerAngles = new Vector3(0, 0, 0);
            cubeRigid.constraints = RigidbodyConstraints.FreezeRotation;
            //cubeRigid.constraints = RigidbodyConstraints.FreezePosition;
            //Debug.Log("Objects are within proximity threshold");
            // If within threshold, check if names end with the same number
            if (DoNamesEndWithSameNumber(cube.name, correctPosition.name))
            {
                //Debug.Log("Cube and Slot are together");
                //Debug.Log(i);
                cubeCorrect[i] = true;
                HandleNameMatch();
            }
        }
    }

    public bool checkForWin()
    {
        for (int j = 0; j < cubeCorrect.Length; j++)
        {
            if (cubeCorrect[j] == false) {
                //Debug.Log("Reached checker for win if statement");
                //Debug.Log("You haven't won yet");
                return false;
            }
        }
        return true;
    }



    // Call this method to toggle highlight on/off
    public bool Highlight(GameObject highlightObject)
    {
        for (int j = 0; j < cubes.Length; j++)
        {
            objectRenderer = cubes[j].GetComponent<Renderer>();
            objectRenderer.material.color = originalColor;
        }

        objectRenderer = highlightObject.GetComponent<Renderer>();

        if (objectRenderer != null)
        {
            //if (objectRenderer.material.color == originalColor)
            //{
                // Apply highlight color
                objectRenderer.material.color = highlightColor;
                return true;
            //}
            //else
            //{
                // Revert to original color
            //    objectRenderer.material.color = originalColor;
            //   return false;
            //}
        }
        return false;
    }


    int GetCorrectPositionIndex(GameObject cube)
    {
        return 0;
    }

    void MoveAICube(GameObject cube, GameObject tile, int i)
    {
        //Debug.Log("Reached move AI cube function");
        Vector3 startPositionOriginal = startPosition;
        //Debug.Log(startPositionOriginal);
        Vector3 targetPositionOriginal = targetPosition;
        //Debug.Log(targetPositionOriginal);

        //Debug.Log("New values:");
        startPosition = cube.transform.position;
        //Debug.Log(startPosition);
        targetPosition = tile.transform.position;
        //Debug.Log(targetPosition);
        MoveObject(cube, tile, i);
        //cube.transform.position = tile.transform.position;

        startPosition = startPositionOriginal;
        targetPosition = targetPositionOriginal;
    }


    void StartMovingObject()
    {
        
        isMoving = true;
        startTime = Time.time;
        startPosition = targetObject.transform.position;
        
    }

    bool MoveObject(GameObject cube, GameObject correctPosition, int i)
    {
        //Debug.Log("Time for the MoveObject function to go");
        // Calculate fraction of the journey completed
        float distanceCovered = (Time.time - startTime) * speed;
        float fractionOfJourney = distanceCovered / Vector3.Distance(startPosition, targetPosition);

        //Debug.Log(i);
        //cubeCorrect[i] = true;
        //cube.transform.position = correctPosition.transform.position;

        // We want the cube to stop moving and face the participant when placed in the correct slot
        Rigidbody cubeRigid;
        cubeRigid = cube.GetComponent<Rigidbody>();

        cubeRigid.isKinematic = false;
        cube.transform.eulerAngles = new Vector3(0, 0, 0);
        cubeRigid.constraints = RigidbodyConstraints.FreezeRotation;


        // Move the object
        if (startPosition != targetPosition)
        {
            Debug.Log("LERP started");
            targetObject.transform.position = Vector3.Lerp(startPosition, targetPosition, fractionOfJourney);
            UpdateCompletionStatus(cube, correctPosition, i);
            return true;
        }
        // Check if the object has reached the target position
        if (targetObject.transform.position == targetPosition)
        {
            isMoving = false;
        }
        return false;
        
    }

    void updateTimer(float currentTime)
    {
        
        currentTime += 1;

        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);

        //TimerTxt.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        TimerTxt.SetText(string.Format("{0:00}:{1:00}", minutes, seconds));
        
    }
}
