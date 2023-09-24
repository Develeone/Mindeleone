using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SavesManager : MonoBehaviour
{
    public InputField saveField;

    public static SavesManager instance;

    public KeyCode masterKey = KeyCode.LeftControl;
    public KeyCode altMasterKey = KeyCode.LeftCommand;
    public KeyCode saveKey = KeyCode.S;
    public KeyCode loadKey = KeyCode.O;
    public KeyCode clearKey = KeyCode.X;
    public KeyCode exportKey = KeyCode.E;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        instance.saveField.text = PlayerPrefs.GetString("save");

        Load();
    }

    private void Update()
    {
        if (!Input.GetKey(masterKey) && !Input.GetKey(altMasterKey))
            return;
        
        if (Input.GetKeyDown(saveKey))   Save();
        if (Input.GetKeyDown(loadKey))   Load();
        if (Input.GetKeyDown(clearKey))  Clear();
        if (Input.GetKeyDown(exportKey)) Export();
    }

    public static void Export() {
        Export(GetSaveCode());
    }

    public static void Export(string saveCode) {
        instance.saveField.text = saveCode;
    }

    public static string GetSaveCode() {
        List<string> nodesCodes = new List<string>();

        foreach (Node node in NodesFactory.instance.nodes)
            nodesCodes.Add(node.GetCode());

        string save = String.Join("\n", nodesCodes.ToArray());

        return save;
    }

    public static void Load(string saveObject = null)
    {
        if (saveObject == null || saveObject == "")
            saveObject = instance.saveField.text;

        if (saveObject.Length < 5)
        {
            Debug.LogWarning("Save not found");
            return;
        }

        Debug.Log("Mindeleone loaded\n\n" + saveObject);

        foreach (Node node in NodesFactory.instance.nodes) Destroy(node.gameObject);

        NodesFactory.instance.nodes = new List<Node>();

        string[] nodesCodes = saveObject.Split("\n");

        foreach (string nodeCode in nodesCodes)
        {
            if (nodeCode.Length < 5)
                continue;

            string[] nodeData = nodeCode.Split(";");

            if (nodeData.Length < 3)
            {
                Debug.LogError("Error! Not enough node data!");
                continue;
            }

            string id = nodeData[0];
            string title = nodeData[1];
            string relations = nodeData[2];
            string positioning = nodeData[3];
            string size = nodeData[4];
            string status = nodeData[5];
            
            string color_r = nodeData.Length > 6 ? nodeData[6] : "1";
            string color_g = nodeData.Length > 7 ? nodeData[7] : "1";
            string color_b = nodeData.Length > 8 ? nodeData[8] : "1";

            if (status != "0") continue;

            Node node = NodesFactory.instance.SpawnNode(title, null, id);

            if (positioning != null)
            {
                Vector2 position = new Vector2(float.Parse(positioning.Split('|')[0]), float.Parse(positioning.Split('|')[1]));
                node.position = position;
            }

            if (size != null)
                node.size = float.Parse(size);

            if (status != null)
                node.status = (Node.NodeStatus) int.Parse(status);
            
            node.color = new Color(float.Parse(color_r), float.Parse(color_g), float.Parse(color_b));
        }

        foreach (Node node in NodesFactory.instance.nodes)
        {
            // �������� �������� ����� � ���� ��� ����� ���� ��������, �� �������� ��������� �� �����
            // ��������� ������, � ��� ��� �������
            List<string> relatedIds = new List<string>();

            foreach (Relation relation in node.GetComponents<Relation>())
            {
                string id = relation.relatedNode.id;

                if (!relatedIds.Contains(id))
                    relatedIds.Add(id);
            }

            // ���������� �� ���� ������� �� �����
            foreach (string nodeCode in nodesCodes)
            {
                string[] nodeData = nodeCode.Split(";");

                // ���� ������� ����� (???) ��� ��������� � ������ ����-��������� - ����������
                if (nodeData.Length < 3 || nodeCode.Split(";")[0] != node.id)
                    continue;

                foreach (string uid in nodeData[2].Split("|"))
                {
                    if (!relatedIds.Contains(uid))
                    {
                        Node targetNode = NodesFactory.instance.nodes.Where(i => i.id == uid).FirstOrDefault();

                        if (targetNode == null || node == null) { Debug.Log(targetNode + " " + node); continue; }

                        node.AddRelation(targetNode);
                        relatedIds.Add(uid);
                    }
                }
            }
        }

        NodesFiltersController.Refresh();
        
        // Invoke "UpdateColors" of all Relations
        foreach (Relation relation in FindObjectsOfType<Relation>())
            relation.UpdateLinesColors();
    }

    public static void Save()
    {
        string result = GetSaveCode();

        Export(result);

        Debug.Log("Mindeleone saved\n\n" + result);

        PlayerPrefs.SetString("save", result);

        instance.saveField.text = result;
    }

    public static void Clear()
    {
        return;
        PlayerPrefs.DeleteAll();
        Debug.Log("Save cleared!");
    }
}
