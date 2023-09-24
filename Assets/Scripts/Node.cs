using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Node : MonoBehaviour
{
    public enum NodeStatus { ACTUAL = 0, COMPLETED = 1, IRRELEVANT = 2 }

    public NodeStatus status {
        get {
            return _status;
        }
        set
        {
            _status = value;
            NodesFiltersController.Refresh();
        }
    }

    public string id = null;

    public string title
    {
        get
        {
            return _title;
        }
        set
        {
            _title = value;
            text.text = value;
            gameObject.name = value;
        }
    }

    public float size
    {
        get
        {
            return _size;
        }
        set
        {
            value = Mathf.Clamp(value, 1f, 100f);
            _size = value;
            Resize(value);
        }
    }

    public Vector2 position
    {
        get
        {
            return transform.localPosition;
        }
        set
        {
            transform.localPosition = value;
        }
    }

    public GameObject linePrefab;

    public GameObject selectionOverlay;

    public bool isActual {
        get { return status == NodeStatus.ACTUAL; }
    }

    public bool isCompleted
    {
        get { return status == NodeStatus.COMPLETED; }
    }

    #region Internal
    internal List<Relation> relations = new List<Relation>();
    internal Rigidbody2D rigidbody2D { get { return transform.GetComponent<Rigidbody2D>(); } }
    #endregion

    #region Private
    private NodeStatus _status = NodeStatus.ACTUAL;
    private string _title = "New node";
    private float _size = 1f;
    private TMP_Text text;
    #endregion

    public static List<Node> resizingNodesStack = new List<Node>();
    
    public Color color
    {
        get
        {
            return GetComponent<SpriteRenderer>().color;
        }
        set
        {
            GetComponent<SpriteRenderer>().color = value;
            
            UpdateLinesColors();
        }
    }

    public void UpdateLinesColors()
    {
        foreach (Relation relation in relations)
            relation.UpdateLinesColors();
    }

    private void Awake()
    {
        if (id == null || id == "")
            id = Random.Range(0, int.MaxValue).ToString();

        text = GetComponentInChildren<TMP_Text>();
    }

    private void FixedUpdate()
    {
        if (!GetComponent<Renderer>().isVisible && text.gameObject.activeSelf)
        {
            text.gameObject.SetActive(false);
        }
        else if (GetComponent<Renderer>().isVisible && !text.gameObject.activeSelf) {
            text.gameObject.SetActive(true);
        }
    }

    internal Relation AddRelation(Node relatedNode)
    {
        Debug.Log("Adding relation between " + title + " and " + relatedNode.title);

        if (relations.Count > 0)
        {
            Relation existingRelation = GetRelationWith(relatedNode);

            if (existingRelation != null)
                return existingRelation;
        }

        Relation createdRelation = gameObject.AddComponent<Relation>();
        createdRelation.Awake();
        createdRelation.Establish(relatedNode);
        relations.Add(createdRelation);

        createdRelation.oppositeRelation = relatedNode.AddRelation(this);

        // TODO: Allow to disable
        Resize(relations.Count);

        return createdRelation;
    }

    private void RemoveRelation(Node relatedNode)
    {
        Relation relation = GetRelationWith(relatedNode);

        if (relation != null)
        {
            relations.Remove(relation);
            Destroy(relation);
        }

        if (relatedNode.GetRelationWith(this) != null)
            relatedNode.RemoveRelation(this);
    }

    public Relation GetRelationWith(Node node)
    {
        return relations.Where(i => i.relatedNode == node).FirstOrDefault();
    }

    public void Resize(float newSize)
    {
        _size = newSize;
        transform.localScale = Vector2.one * _size;
    }

    // FOR WHAT??
    internal void SetPosition(Vector2 position)
    {
        transform.localPosition = position;
    }

    private void AddDeltaSize(float v, bool isRootScaler = true)
    {
        size += v;

        // Was commented out
        ResizeNeighbours(v, isRootScaler);
    }

    private void ResizeNeighbours(float v, bool isRootScaler = true)
    {
        if (isRootScaler)
            resizingNodesStack = new List<Node>();

        if (resizingNodesStack.FirstOrDefault(node => node.id == this.id) == null)
        {
            resizingNodesStack.Add(this);

            if (Mathf.Abs(v) < 0.1f)
                return;

            foreach (Relation relation in relations)
                relation.relatedNode.AddDeltaSize(v * 0.5f, false);
        }
    }

    internal string GetCode()
    {
        List<string> relationsList = new List<string>();

        foreach (Relation relation in relations) {
            relationsList.Add(relation.relatedNode.id);
        }

        string positioning = transform.position.x + "|" + transform.position.y;

        List<string> nodeCode = new List<string>();

        nodeCode.Add(id);
        nodeCode.Add(title);
        nodeCode.Add(String.Join("|", relationsList.ToArray()));
        nodeCode.Add(positioning);
        nodeCode.Add(size.ToString());
        nodeCode.Add(((int)status).ToString());

        nodeCode.Add((color.r).ToString());
        nodeCode.Add((color.g).ToString());
        nodeCode.Add((color.b).ToString());

        return String.Join(";", nodeCode.ToArray());
    }

    private void OnMouseDown()
    {
        if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetKey(SavesManager.instance.masterKey) || Input.GetKey(CameraMovement.instance.masterKey))
            return;

        // New relation
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (NodesFactory.selectedNode != null)
            {
                AddRelation(NodesFactory.selectedNode);
            }

            return;
        }

        // Remove relation
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            if (NodesFactory.selectedNode != null)
            {
                RemoveRelation(NodesFactory.selectedNode);
            }

            return;
        }

        NodesFactory.SelectNode(this);
    }

    private void OnMouseOver()
    {
        if (Input.GetKey(KeyCode.LeftShift) && NodesFactory.selectedNode == this)
            AddDeltaSize(Input.mouseScrollDelta.y, true);
    }

    internal void Deselect()
    {
        selectionOverlay.SetActive(false);
    }

    internal void Select()
    {
        selectionOverlay.SetActive(true);
    }


    public void SetVisibility(bool isVisible)
    {
        foreach (Relation relation in relations)
            relation.oppositeRelation.SetRelationVisibility(isVisible);

        gameObject.SetActive(isVisible);
    }


    private void OnDestroy()
    {
        for (int i = relations.Count - 1; i >= 0; i--)
            RemoveRelation(relations[i].relatedNode);

        NodesFactory.instance.nodes.Remove(this);
    }
}
