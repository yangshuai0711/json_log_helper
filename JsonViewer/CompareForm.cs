using EPocalipse.Json.Viewer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace compareWindow
{
    public partial class CompareWindow : Form
    {
        public CompareWindow()
        {
            InitializeComponent();
        }

        public CompareWindow(KeyValuePair<string,JsonObjectTree>[] data)
        {
            InitializeComponent();
            label1.Text = data[0].Key;
            label2.Text = data[1].Key;
            VisualizeJsonTree(compTreeViewLeft, data[0].Value);
            VisualizeJsonTree(compTreeViewRight, data[1].Value);
        }

        private void CompareWindow_Load(object sender, EventArgs e)
        {
            
        }



        private void VisualizeJsonTree(TreeView treeView,JsonObjectTree tree)
        {
            //levelNodesCount = new int[20];
            
            treeView.Nodes.Clear();
            AddNode(treeView.Nodes, tree.Root);
            JsonViewerTreeNode node = (JsonViewerTreeNode)treeView.Nodes[0];
            //JsonViewer.expandSubNodes(node, index - 1 < 0 ? 0 : index - 1);
            node.Expand();
            compTreeViewLeft.SelectedNode = node;
        }

        //private int[] levelNodesCount;

        private void AddNode(TreeNodeCollection nodes, JsonObject jsonObject)
        {
            JsonViewerTreeNode newNode = new JsonViewerTreeNode(jsonObject);
            nodes.Add(newNode);
            newNode.Text = jsonObject.Text;
            newNode.Tag = jsonObject;
            newNode.ImageIndex = (int)jsonObject.JsonType;
            newNode.SelectedImageIndex = newNode.ImageIndex;
            foreach (JsonObject field in jsonObject.Fields)
            {
                AddNode(newNode.Nodes, field);
            }

            //levelNodesCount[newNode.Level]++;
        }
    }
}
