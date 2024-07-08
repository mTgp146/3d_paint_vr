using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ResetToolsButton : MonoBehaviour
{
    public GameObject[] tools;

    private Vector3[] toolStartPositions;
    
    void Awake()
    {
        toolStartPositions = new Vector3[tools.Length];
        
        int index = 0;
        foreach (GameObject tool in tools)
        {
            if (tool != null)
            {
                toolStartPositions[index] = tool.transform.position;
            }

            index++;
        }
    }

    public void ResetAllTools()
    {
        int index = 0;
        foreach (GameObject tool in tools)
        {
            if (tool != null)
            {
                tool.transform.position = toolStartPositions[index];
            }

            index++;
        }
    }
}
