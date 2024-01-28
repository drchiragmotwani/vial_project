using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class sensorControl : MonoBehaviour
{
    public GameObject vial, sensor;
    private bool vialMoving;
    private List<float> distanceArr, finalDistanceArr;
    private List<Color> colorArr, finalColorArr, colors, currentColors;
    private List<string> numPad, colorStrings;
    private List<Text> countTexts;
    public Slider colorScaleSelector;
    public Text distanceDetected, colorDetected, red, green, blue, cyan, magenta, yellow, black, white, grey, errorText;
    private float keyPressedTime;
    System.Random _random;

    // Start is called before the first frame update
    void Start()
    {
        colors = new List<Color> { Color.red, Color.green, Color.blue, Color.black, Color.grey, Color.white, Color.cyan, Color.magenta, Color.yellow };
        colorStrings = new List<string> { "Red", "Green", "Blue", "Black", "Grey", "White", "Cyan", "Magenta", "Yellow" };
        _random = new System.Random();
        distanceArr = new List<float>();
        colorArr = new List<Color>();
        finalDistanceArr = new List<float>();
        finalColorArr = new List<Color>();
        currentColors = new List<Color> { Color.red, Color.green, Color.blue };
        numPad = new List<string> { "Alpha1", "Alpha2", "Alpha3", "Alpha7", "Alpha9", "Alpha8", "Alpha4", "Alpha5", "Alpha6", "Keypad1", "Keypad2", "Keypad3", "Keypad7", "Keypad9", "Keypad8", "Keypad4", "Keypad5", "Keypad6", };
        countTexts = new List<Text> { red, green, blue, black, grey, white, cyan, magenta, yellow };
        colorScaleSelector.value = 1;
    }

    // Update is called once per frame
    void Update()
    {
        updateColorScore();

        colorScaleSelector.onValueChanged.AddListener(delegate { sliderValueChange(); });

        Vector3 dir = transform.TransformDirection(Vector3.down);
        RaycastHit hit;
        Color color;

        if (Physics.Raycast(transform.position, dir, out hit, 13))
        {
            color = hit.transform.gameObject.GetComponent<Renderer>().material.color;
            Debug.DrawRay(transform.position, dir, Color.magenta);

            if (hit.distance >= 0)
            {
                // Debug.Log(hit.distance);
                distanceDetected.text = hit.distance.ToString();
                //Debug.Log(color == Color.grey);
                
                colorDetected.text = colorStrings[colors.FindIndex(x => x == color)];
                colorDetected.color = color;
            }
            
            distanceArr.Add(hit.distance);
            colorArr.Add(color);
        }

        if (vial.transform.position.z > -10)
        {
            vial.transform.Translate(-Vector3.forward * Time.deltaTime * 7);
            colorScaleSelector.interactable = false;
            vialMoving = true;
        }
        else
        {
            colorScaleSelector.interactable = true;
            vialMoving = false;
            
            if (distanceArr.Count != 0)
            {
                if (currentColors.Contains(colorArr[0]))
                {
                    finalDistanceArr.Add(distanceArr[0]);
                    finalColorArr.Add(colorArr[0]);
                }
                           
                distanceArr.Clear();
                colorArr.Clear();
            }
        }

        //if (Input.GetKey(KeyCode.Space) && !vialMoving)
        //{
        //    colorChange();
        //}

        //if (Input.GetKey(KeyCode.R) && !vialMoving)
        //{
        //    resetPosition();
        //    Debug.Log(finalDistanceArr.Count);
        //}
    }

    private void colorChange()
    {
        int index = _random.Next(colors.Count);
        Debug.Log("Color index: " + index);
        vial.GetComponent<Renderer>().material.SetColor("_Color", colors[index]);
        sensor.GetComponent<Renderer>().material.SetColor("_Color", colors[index]);
    }

    private void resetPosition()
    {
        vial.transform.position = vial.transform.position + new Vector3(0, 0, 20);

    }

    public void sliderValueChange()
    {
        if (!vialMoving)
        {
            decimal maxVal = decimal.Round((decimal)colorScaleSelector.value, 0) * 3; // To use in extracting elements from colors list
            currentColors.Clear();
            currentColors.Add(colors[(int)maxVal - 3]);
            currentColors.Add(colors[(int)maxVal - 2]);
            currentColors.Add(colors[(int)maxVal - 1]);
            errorText.text = "";
        }
        else
        {
            errorText.text = "Please wait for current scan to finish!";
        }

    }

    void OnGUI()
    {
        Event e = Event.current;
        if (e.type.ToString() == "KeyDown" && !vialMoving && e.keyCode.ToString() != "None")
        {
            Debug.Log(e.type);
            keyPressedTime = Time.time * 1000;

            errorText.text = "";

            string keyPressedString = e.keyCode.ToString();
            Debug.Log("Detected key code: " + e.keyCode);
            if (numPad.Contains(keyPressedString))
            {
                int keyPressedIndex = numPad.FindIndex(x => x == e.keyCode.ToString());
                if (keyPressedIndex > 8)
                {
                    keyPressedIndex = keyPressedIndex - 9;
                }

                vial.GetComponent<Renderer>().material.SetColor("_Color", colors[keyPressedIndex]);
                sensor.GetComponent<Renderer>().material.SetColor("_Color", colors[keyPressedIndex]);
            }
            else if (keyPressedString == "Space")
            {
                colorChange();
            }
            else if (keyPressedString == "R")
            {
                resetPosition();
            }
            else
            {
                errorText.text = "Invalid Key!";
            }
        }
        else if (e.isKey && vialMoving && Time.time * 1000 >= keyPressedTime + 1000)
        {
            errorText.text = "Please wait for current scan to finish!";
        }
        else if (!e.isKey && !vialMoving && Time.time*1000 >= keyPressedTime + 3000)
        {
            errorText.text = "";
        }
    }

    private void updateColorScore()
    {
        for (int n = 0; n < countTexts.Count; n++)
        {
            countTexts[n].text = finalColorArr.FindAll(x => x == colors[n]).Count.ToString();
        }
    }
}
