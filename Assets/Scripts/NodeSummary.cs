using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NodeSummary : MonoBehaviour
{
    public static NodeSummary instance;

    public GameObject nodeRelationButtonPrefab;
    public Transform nodeRelationButtonsContainer;

    private void Awake()
    {
        instance = this;
    }

    public void ShowRelations(Node node) {
        // Destroy all previous buttons
        for (int i = 0; i < nodeRelationButtonsContainer.childCount; i++)
            Destroy(nodeRelationButtonsContainer.GetChild(i).gameObject);

        //Spawn new buttons
        List<Relation> relations = new List<Relation>(node.GetComponents<Relation>());

        relations.Sort(delegate (Relation a, Relation b) {
             return a.relatedNode.size > b.relatedNode.size ? -1 : 1;
        });

        foreach (Relation relation in relations)
        {
            GameObject buttonObject = Instantiate(nodeRelationButtonPrefab, nodeRelationButtonsContainer);
            buttonObject.GetComponentInChildren<Text>().text = "(" + relation.relatedNode.size + ") " + relation.relatedNode.title;
            buttonObject.GetComponentInChildren<Image>().color = relation.relatedNode.color - new Color(0, 0, 0, 0.5f);
            buttonObject.GetComponentInChildren<Button>().onClick.AddListener(() => {
                NodesFactory.SelectNode(relation.relatedNode);
                CameraMovement.instance.FocusNode(relation.relatedNode);
            });
        }
    }
}
