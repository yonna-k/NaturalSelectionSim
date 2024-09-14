using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Window_Graph : MonoBehaviour
{
    [SerializeField] private Sprite circleSprite;
    private RectTransform graphContainer;
    public GameObject GraphScreen;
    private RectTransform labelTemplateX;
    private RectTransform labelTemplateY;
    private static int initialpopulation;
    private static int gen_number;
    GameObject lastCircleGameObject = null;
    GameObject lastCircleGameObjectP = null;
    List<int> valuelist = new List<int>();
    List<int> valuelistP = new List<int>();

    void Start()
    {
        graphContainer = GraphScreen.transform.Find("graphContainer").GetComponent<RectTransform>();
        labelTemplateX = graphContainer.Find("labelTemplateX").GetComponent<RectTransform>();
        labelTemplateX.gameObject.SetActive(false);
        labelTemplateY = graphContainer.Find("labelTemplateY").GetComponent<RectTransform>();
        labelTemplateY.gameObject.SetActive(false);
        initialpopulation = (StateNameController.population);
        gen_number = PauseResume.gennumber;
        valuelist.Add(initialpopulation);
        valuelistP.Add(0);
        ShowGraph(valuelist, 0);
        ShowGraphPredator(valuelistP, 0);
    }

    void Update()
    {
        int gen = PauseResume.gennumber;
        if (gen_number != gen)
        {
            Debug.Log("in here");
            gen_number = PauseResume.gennumber;
            int temp = (GameObject.FindGameObjectsWithTag("Chicken").Length) + (GameObject.FindGameObjectsWithTag("Speed").Length) + (GameObject.FindGameObjectsWithTag("Energy").Length) + (GameObject.FindGameObjectsWithTag("SE").Length); //finds how many chickens on the plane at the start of each gen
            valuelist.Add(temp); //adds to a list
            ShowGraph(valuelist, gen_number - 1);
            int temp2 = (GameObject.FindGameObjectsWithTag("Predator").Length);
            valuelistP.Add(temp2); //adds to a list
            ShowGraphPredator(valuelistP, gen_number - 1);
        }

    }

    private GameObject CreateCircle(Vector2 anchoredPosition)
    {
        GameObject gameObject = new GameObject("circle", typeof(Image));
        gameObject.transform.localScale = new Vector3(0.1397466f, 0.4096875f, 1.0f); //scale of the dot
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().sprite = circleSprite;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(11, 11);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        return gameObject;
    }

    private void ShowGraph(List<int> valuelist, int j)
    {
        float graphHeight = graphContainer.sizeDelta.y;
        float graphWidth = graphContainer.sizeDelta.x;
        float yMaximum = 40f; //max y value
        float xSize = graphWidth / 20;//controls how spaced out the dots are
        {
            float xPosition = j * xSize;
            float yPosition = (valuelist[j] / yMaximum) * graphHeight;
            GameObject circleGameObject = CreateCircle(new Vector2(xPosition, yPosition));
            if (lastCircleGameObject != null)
            {
                CreateDotConnection(lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition, circleGameObject.GetComponent<RectTransform>().anchoredPosition);
            }
            lastCircleGameObject = circleGameObject;

            RectTransform labelX = Instantiate(labelTemplateX);
            labelX.SetParent(graphContainer, false);
            labelX.gameObject.SetActive(true);
            labelX.anchoredPosition = new Vector2(xPosition + 3, -5.2f); //formatting the x axis labels
            labelX.GetComponent<TextMeshProUGUI>().text = (j+1).ToString();
            labelX.localScale = new Vector3(0.05593378f, 0.1593675f, 1f);
        }
        int separatorcount = 10; //how many y axis labels there are
        for (int i = 0; i <= separatorcount; i++)
        {
            RectTransform labelY = Instantiate(labelTemplateY);
            labelY.SetParent(graphContainer, false);
            labelY.gameObject.SetActive(true);
            float normalisedvalue = i * 1f / separatorcount;
            labelY.anchoredPosition = new Vector2(0.3f, normalisedvalue * graphHeight);
            labelY.GetComponent<TextMeshProUGUI>().text = Mathf.RoundToInt(normalisedvalue * yMaximum).ToString();
            labelY.localScale = new Vector3(0.05593378f, 0.1593675f, 1f);
        }

    }

    private void CreateDotConnection (Vector2 dotPositionA, Vector2 dotPositionB) //creates connection between 2 dots on the graph
    {
        GameObject gameObject = new GameObject("dotConnection", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().color = new Color(1, 1, 1, .5f);
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        Vector2 dir = (dotPositionB - dotPositionA).normalized;
        float distance = Vector2.Distance(dotPositionA, dotPositionB);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.sizeDelta = new Vector2(distance, 3f);
        rectTransform.anchoredPosition = dotPositionA;
        rectTransform.anchoredPosition = dotPositionA + dir * distance * .5f;
        rectTransform.localEulerAngles = new Vector3(0, 0, GetAngleFromVectorFloat(dir));
        gameObject.transform.localScale = new Vector3(1.0f, 0.2f, 1.0f); //thickness of the line (y value change)
    }

    float GetAngleFromVectorFloat(Vector2 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;
        return n;
    }

    private void ShowGraphPredator(List<int> valuelist, int j)
    {
        float graphHeight = graphContainer.sizeDelta.y;
        float graphWidth = graphContainer.sizeDelta.x;
        float yMaximum = 40f; //max y value
        float xSize = graphWidth / 20;//controls how spaced out the dots are
        {
            float xPosition = j * xSize;
            float yPosition = (valuelist[j] / yMaximum) * graphHeight;
            GameObject circleGameObject = CreateCircle(new Vector2(xPosition, yPosition));
            circleGameObject.GetComponent<Image>().color = new Color (255, 0, 0);
            if (lastCircleGameObjectP != null)
            {
                CreateDotConnectionPredator(lastCircleGameObjectP.GetComponent<RectTransform>().anchoredPosition, circleGameObject.GetComponent<RectTransform>().anchoredPosition);
            }
            lastCircleGameObjectP = circleGameObject;
        }
        
    }
    private void CreateDotConnectionPredator(Vector2 dotPositionA, Vector2 dotPositionB) //creates connection between 2 dots on the graph
    {
        GameObject gameObject = new GameObject("dotConnection", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().color = new Color(255,0,0, 0.5f);
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        Vector2 dir = (dotPositionB - dotPositionA).normalized;
        float distance = Vector2.Distance(dotPositionA, dotPositionB);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.sizeDelta = new Vector2(distance, 3f);
        rectTransform.anchoredPosition = dotPositionA;
        rectTransform.anchoredPosition = dotPositionA + dir * distance * .5f;
        rectTransform.localEulerAngles = new Vector3(0, 0, GetAngleFromVectorFloat(dir));
        gameObject.transform.localScale = new Vector3(1.0f, 0.2f, 1.0f); //thickness of the line (y value change)
    }

}
