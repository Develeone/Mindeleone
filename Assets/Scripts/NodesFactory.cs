using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class NodesFactory : MonoBehaviour
{
    public static NodesFactory instance;
    public static Node selectedNode;

    public GameObject nodePrefab;
    public List<Node> nodes;
    public Transform nodesContainer;

    public GameObject titleInput;
    
    private InputField titleInputField;

    private void Awake()
    {
        instance = this;
        titleInputField = titleInput.GetComponent<InputField>();
    }

    private void Start()
    {
        SelectNode(nodes[0]);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            SaveNodeTitle();

            AppendNode("");
            EditNodeTitle();
        }

        if (Input.GetKeyDown(KeyCode.Return)) {
            SaveNodeTitle();
        }
        
        if (Input.GetKeyDown(KeyCode.Delete) || (Input.GetKey(KeyCode.RightCommand) && Input.GetKeyDown(KeyCode.Backspace)))
            if (selectedNode != null)
            {
                //selectedNode.status = Node.NodeStatus.IRRELEVANT;
                Destroy(selectedNode.gameObject);
            }
    }

    private void SaveNodeTitle()
    {
        if (selectedNode == null)
            return;

        titleInputField.GetComponent<InputFieldValidator>().Validate();

        if (titleInputField.text.Length > 1)
            selectedNode.title = titleInputField.text;
    }

    private void EditNodeTitle()
    {
        if (selectedNode == null)
            return;

        titleInputField.GetType().GetField("m_AllowInput", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(titleInputField, true);
        titleInputField.GetType().InvokeMember("SetCaretVisible", BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.Instance, null, titleInputField, null);
        titleInputField.GetType().GetField("m_AllowInput", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(titleInputField, true);
        titleInputField.caretPosition = titleInputField.text.Length;
    }

    public void AppendNode(string title) {
        SpawnNode(title, selectedNode);
        SelectNode(nodes[nodes.Count - 1]);
    }

    public Node SpawnNode(string title, Node relatedNode = null, string uid = null)
    {
        Vector2 relationPosition = relatedNode == null ? Vector2.zero : relatedNode.position;
        Vector2 spawnPosition = relationPosition + Vector2.down + new Vector2(UnityEngine.Random.Range(-0.1f, 0.1f), UnityEngine.Random.Range(-0.1f, 0.1f));

        Node node = GameObject.Instantiate(nodePrefab, spawnPosition, Quaternion.identity, nodesContainer).GetComponent<Node>();

        node.title = title;

        if (uid != null)
            node.id = uid;

        if (relatedNode != null)
            node.AddRelation(relatedNode);

        nodes.Add(node);

        return node;
    }

    internal static void SelectNode(Node node)
    {
        if (selectedNode != null)
        {
            instance.SaveNodeTitle();
            selectedNode.Deselect();
        }

        selectedNode = node;

        NodeSummary.instance.ShowRelations(selectedNode);

        instance.titleInputField.text = selectedNode.title;

        selectedNode.Select();
    }

    // rgb
    private void ChangeNodeColor(float r, float g, float b)
    {
        if (selectedNode == null)
            return;
        
        if (r == selectedNode.color.r && g == selectedNode.color.g && b == selectedNode.color.b)
            return;

        selectedNode.color = new Color(r, g, b);
    }

    void OnGUI()
    {
        if (selectedNode == null)
            return;
        
        // Three input sliders for red, green and blue
        ChangeNodeColor(
            GUI.HorizontalSlider(new Rect(25, 25, 100, 30), selectedNode.color.r, 0.0f, 1.0f),
            GUI.HorizontalSlider(new Rect(25, 60, 100, 30), selectedNode.color.g, 0.0f, 1.0f),
            GUI.HorizontalSlider(new Rect(25, 95, 100, 30), selectedNode.color.b, 0.0f, 1.0f)
        );
    }
}
