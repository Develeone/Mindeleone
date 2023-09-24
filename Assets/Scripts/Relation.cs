using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Relation : MonoBehaviour
{
    public Node currentNode;
    public Node relatedNode;
    public Relation oppositeRelation;

    private GameObject lineHandler;
    public LineRenderer line;
    private SpringJoint2D joint;

    private float spacing = 1f;

    public void Awake()
    {
        currentNode = GetComponent<Node>();
    }

    public void Establish(Node _relatedNode)
    {
        relatedNode = _relatedNode;

        lineHandler = Instantiate(currentNode.linePrefab, transform);
        line = lineHandler.GetComponent<LineRenderer>();
    }

    void Update()
    {
        line.SetPosition(0, transform.position);
        line.SetPosition(1, relatedNode.position);
    }

    public void SetRelationVisibility(bool isVisible) {
        lineHandler.SetActive(isVisible);
    }

    private void OnDestroy()
    {
        Destroy(lineHandler);
    }

    public void UpdateLinesColors()
    {
        line.startColor = currentNode.color;
        line.endColor = relatedNode.color;
        
        //currentNode.UpdateLinesColors();
        //relatedNode.UpdateLinesColors();
    }
}
