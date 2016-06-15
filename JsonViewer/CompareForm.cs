using EPocalipse.Json.Viewer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

namespace compareWindow
{

    public partial class CompareWindow : Form
    {
        public static Color DIFF_COLOR = Color.Yellow;
        public static Color NOTFIND_COLOR = Color.Red;
        public static Color HIGH_LI_COLOR = Color.Blue;
        private Dictionary<string, bool> diffNodesPathLeft = new Dictionary<string, bool>();
        private Dictionary<string, bool> diffNodesPathRight = new Dictionary<string, bool>();
        private Dictionary<string, Dictionary<string, bool>> diffPathMap = new Dictionary<string, Dictionary<string, bool>>(2);
        private Dictionary<Button, TreeNode> linkDiffMap = new Dictionary<Button, TreeNode>();
        public CompareWindow()
        {
            InitializeComponent();
            diffPathMap.Add(compTreeViewLeft.Name, diffNodesPathLeft);
            diffPathMap.Add(compTreeViewRight.Name, diffNodesPathRight);
        }

        public CompareWindow(KeyValuePair<string, JsonObjectTree>[] data)
        {
            InitializeComponent();
            diffPathMap.Add(compTreeViewLeft.Name, diffNodesPathLeft);
            diffPathMap.Add(compTreeViewRight.Name, diffNodesPathRight);
            label1.Text = data[0].Key;
            label2.Text = data[1].Key;
            VisualizeJsonTree(compTreeViewLeft, data[0].Value);
            VisualizeJsonTree(compTreeViewRight, data[1].Value);
            compTreeViewLeft.BeginUpdate();
            compTreeViewRight.BeginUpdate();
            compareAndMarkNodes(compTreeViewLeft.Nodes[0].Nodes, compTreeViewRight.Nodes[0].Nodes);
            compTreeViewLeft.EndUpdate();
            compTreeViewRight.EndUpdate();
            this.defaultForeColor = this.ForeColor;
        }
        private int diffLinkCount = 0;
        public void setDiffLink(TreeNode node)
        {
            Button label = new Button();
            label.FlatStyle = FlatStyle.Flat;
            Dictionary<TreeNode, TreeNode> map = node.TreeView == compTreeViewLeft ? diffRefMapLeft : diffRefMapRight;
            if (map.ContainsKey(node))
            {
                Dictionary<TreeNode, TreeNode> map2 = node.TreeView != compTreeViewLeft ? diffRefMapLeft : diffRefMapRight;
                TreeNode node2 = map[node];
                label.BackColor =!map2.ContainsKey(node2)||map2[node2]!=node ? NOTFIND_COLOR : DIFF_COLOR;
            }
            label.Text = ++diffLinkCount + "";
            label.SetBounds(0, 40 + 25 * (panel1.Controls.Count + 1), panel1.Width-25, 22);
            label.Click += new EventHandler(diffLinkLabel_Click);
            label.MouseHover += new EventHandler(showToolTip);
            panel1.Controls.Add(label);
            linkDiffMap.Add(label, node);
        }

        private void diffLinkLabel_Click(object sender, EventArgs e)
        {
            Button lable = (Button)sender;
            TreeNode node = linkDiffMap[lable];
            showNodeDiff(node);

        }

        private void showNodeDiff(TreeNode node)
        {
            node.EnsureVisible();
            node.ExpandAll();
            List<TreeNode> flash = new List<TreeNode>(2);
            flash.Add(node);
            fillAllSubNodesList(flash, node);
            Dictionary<TreeNode, TreeNode> map = node.TreeView == compTreeViewLeft ? diffRefMapLeft : diffRefMapRight;
            if (map.ContainsKey(node))
            {
                TreeNode node2 = map[node];
                Dictionary<TreeNode, TreeNode> map2 = node.TreeView == compTreeViewLeft ? diffRefMapRight : diffRefMapLeft;
                if (map2.ContainsKey(node2) && map2[node2] == node)
                {
                    node2.EnsureVisible();
                    node2.ExpandAll();
                    flash.Add(node2);
                    fillAllSubNodesList(flash, node2);
                }
            }
            makeNodesHighLight(flash);
        }

        private void fillAllSubNodesList(List<TreeNode> nodeList,TreeNode node) {
            if (node.Nodes.Count < 1) {
                return;
            }
            foreach(TreeNode sub in node.Nodes){
                nodeList.Add(sub);
                fillAllSubNodesList(nodeList, sub);
            }
        }

        private TreeNode getSelecteNode() {
            
            TreeNode node = compTreeViewLeft.SelectedNode;
            if (compTreeViewLeft.ContainsFocus&&node != null)
            {
                return node;
            }
            node = compTreeViewRight.SelectedNode;
            if (compTreeViewRight.ContainsFocus && node != null)
            {
                return node;
            }
            return null;
        }

        private List<TreeNode> highLightNodes = new List<TreeNode>();
        private List<Color> lastNodeColors = new List<Color>();

        private Color defaultForeColor;


        private void cancelPrevHighLight() {
            compTreeViewLeft.BeginUpdate();
            compTreeViewRight.BeginUpdate();
            for (int i = 0; i < highLightNodes.Count; i++)
            {
                highLightNodes[i].BackColor = lastNodeColors[i];
                highLightNodes[i].ForeColor = defaultForeColor;
            }
            lastNodeColors.Clear();
            highLightNodes.Clear();
            compTreeViewLeft.EndUpdate();
            compTreeViewRight.EndUpdate();
        
        }

        private void makeNodesHighLight(List<TreeNode> nodes)
        {
            cancelPrevHighLight();
            compTreeViewLeft.BeginUpdate();
            compTreeViewRight.BeginUpdate();
            foreach (TreeNode node in nodes)
            {
                lastNodeColors.Add(node.BackColor);
                node.BackColor = HIGH_LI_COLOR;
                node.ForeColor = Color.White;
            }
            highLightNodes = nodes;
            compTreeViewLeft.EndUpdate();
            compTreeViewRight.EndUpdate();

        }

        private int flashTime;
        void highLightTimer_Tick(object sender, EventArgs e)
        {
            compTreeViewLeft.BeginUpdate();
            compTreeViewRight.BeginUpdate();
            if (flashTime > 0)
            {
                for (int i = 0; i < highLightNodes.Count; i++)
                {
                    TreeNode node = highLightNodes[i];
                    if (flashTime % 2 == 1)
                    {
                        node.BackColor = this.defaultNodeColor;
                    }
                    else
                    {
                        node.BackColor = lastNodeColors[i];
                    }
                }
                flashTime--;
            }
            else
            {
                for (int i = 0; i < highLightNodes.Count; i++)
                {
                    highLightNodes[i].BackColor = lastNodeColors[i];
                }
                flashTime = 0;
                highLightTimer.Stop();
                highLightNodes.Clear();
                lastNodeColors.Clear();
            }
            compTreeViewLeft.EndUpdate();
            compTreeViewRight.EndUpdate();

        }



        private ToolTip tip = new ToolTip();
        private void showToolTip(object sender, EventArgs e)
        {
            Button lable = (Button)sender;
            TreeNode node = linkDiffMap[lable];
            Dictionary<TreeNode, TreeNode> map = node.TreeView == compTreeViewLeft ? diffRefMapLeft : diffRefMapRight;
            string tipText = "";
            if (map.ContainsKey(node))
            {
                TreeNode node2 = map[node];
                Dictionary<TreeNode, TreeNode> map2 = node.TreeView == compTreeViewLeft ? diffRefMapRight : diffRefMapLeft;
                if (node.TreeView == compTreeViewLeft)
                {
                    tipText = node.Text + " | " + (!map2.ContainsKey(node2) || map2[node2] != node ? "" : node2.Text);
                }
                else
                {
                    tipText = (!map2.ContainsKey(node2) || map2[node2] != node ? "" : node2.Text) + " | " + node.Text;
                }
            }
            tip.SetToolTip(lable, tipText);
        }


        private void VisualizeJsonTree(TreeView treeView, JsonObjectTree tree)
        {
            //levelNodesCount = new int[20];
            treeView.BeginUpdate();
            try
            {
                treeView.Nodes.Clear();
                AddNode(treeView.Nodes, tree.Root);
                JsonViewerTreeNode node = (JsonViewerTreeNode)treeView.Nodes[0];
                defaultNodeColor = node.BackColor;
                node.ExpandAll();
                compTreeViewLeft.SelectedNode = node;
                treeView.Sort();//很关键，否则无法正常比较

            }
            catch (Exception) { }
            finally
            {
                treeView.EndUpdate();
            }
        }

        //private int[] levelNodesCount;

        private void AddNode(TreeNodeCollection nodes, JsonObject jsonObject)
        {
            JsonViewerTreeNode newNode = new JsonViewerTreeNode(jsonObject);
            nodes.Add(newNode);
            newNode.Text = jsonObject.Text.Trim();
            newNode.Tag = jsonObject;
            newNode.ImageIndex = (int)jsonObject.JsonType;
            newNode.SelectedImageIndex = newNode.ImageIndex;
            foreach (JsonObject field in jsonObject.Fields)
            {
                AddNode(newNode.Nodes, field);
            }

            //levelNodesCount[newNode.Level]++;
        }


        private Dictionary<TreeNode, TreeNode> diffRefMapLeft = new Dictionary<TreeNode, TreeNode>();
        private Dictionary<TreeNode, TreeNode> diffRefMapRight = new Dictionary<TreeNode, TreeNode>();
        private List<TreeNode> diffList = new List<TreeNode>();

        //比较两个节点树
        private void compareAndMarkNodes(TreeNodeCollection nodes1, TreeNodeCollection nodes2)
        {
            if (nodes1.Count < 1 || nodes2.Count < 1)
            {
                return;
            }
            int i1 = 0;
            int i2 = 0;
            while (i1 < nodes1.Count || i2 < nodes2.Count)
            {
                TreeNode node1 = nodes1[i1 >= nodes1.Count ? nodes1.Count - 1 : i1];
                TreeNode node2 = nodes2[i2 >= nodes2.Count ? nodes2.Count - 1 : i2];
                //一方边界
                if (i2 >= nodes2.Count && i1 < nodes1.Count)
                {
                    i1++;
                    markAllDiff(node1,true,true);
                    diffRefMapLeft.Remove(node1);
                    diffRefMapLeft.Add(node1, node2);
                    setDiffLink(node1);
                }
                else if (i1 >= nodes1.Count && i2 < nodes2.Count)
                {
                    i2++;
                    markAllDiff(node2, true, true);
                    diffRefMapRight.Remove(node2);
                    diffRefMapRight.Add(node2, node1);
                    setDiffLink(node2);
                }
                else
                {//两方均未边界

                    if ((node1.Text == node2.Text)||(node1.Level<2&&node2.Level<2))
                    {
                        compareAndMarkNodes(node1.Nodes, node2.Nodes);
                        i1++;
                        i2++;
                    }
                    else
                    {
                        if (node1.Nodes.Count < 1 && node2.Nodes.Count < 1)
                        {//叶子节点属性名相同
                            string[] arr1 = node1.Text.Split(new char[] { ':' }, 2);
                            string[] arr2 = node2.Text.Split(new char[] { ':' }, 2);
                            if (arr1[0] == arr2[0])
                            {
                                i1++;
                                i2++;
                                markAllDiff(node1, true, false);
                                markAllDiff(node2, true, false);
                                diffRefMapLeft.Remove(node1);
                                diffRefMapRight.Remove(node2);
                                diffRefMapLeft.Add(node1, node2);
                                diffRefMapRight.Add(node2, node1);
                                setDiffLink(node2);
                                continue;
                            }
                        }
                        if (node1.Text.CompareTo(node2.Text) > 0)
                        {
                            markAllDiff(node2, true, true);
                            i2++;
                            diffRefMapRight.Remove(node2);
                            diffRefMapRight.Add(node2, node1);
                            setDiffLink(node2);

                        }
                        else if (node1.Text.CompareTo(node2.Text) < 0)
                        {
                            markAllDiff(node1, true, true);
                            diffRefMapLeft.Remove(node1);
                            diffRefMapLeft.Add(node1, node2);
                            setDiffLink(node1);
                            i1++;
                        }
                    }
                }
            }
        }


        private void drawDiffLine(Graphics g, TreeNode nodeFrom, TreeNode nodeTo)
        {
            TreeNode node1, node2;
            if (nodeTo.Bounds.X > nodeFrom.Bounds.X)
            {
                node1 = nodeFrom;
                node2 = nodeTo;
            }
            else
            {
                node2 = nodeFrom;
                node1 = nodeTo;
            }
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Pen pen = new Pen(Color.GreenYellow, node1.Bounds.Height);
            int centerX = (compTreeViewLeft.Bounds.X + compTreeViewLeft.Bounds.Width + compTreeViewRight.Bounds.X) / 2;
            int height1 = node2.BackColor == DefaultBackColor ? node2.Bounds.Height : node2.Bounds.Height / 2;
            Point p1 = new Point(node1.Bounds.X + node1.Bounds.Width, node1.Bounds.Y + node1.Bounds.Height / 2);
            Point p2 = new Point(centerX, node1.Bounds.Y + node1.Bounds.Height / 2);
            Point p3 = new Point(centerX, node2.Bounds.Y + height1);
            Point p4 = new Point(node2.Bounds.X, node2.Bounds.Y + height1);
            g.DrawBezier(pen, p1, p2, p3, p4);
        }

        private Color defaultNodeColor;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="addPPath">是否添加父节点路径</param>
        private void markAllDiff(TreeNode node, bool addPPath = true,bool notfind = false)
        {
            node.BackColor = notfind ? NOTFIND_COLOR :DIFF_COLOR;
            Dictionary<string, bool> map = diffPathMap[node.TreeView.Name];
            //if (!map.ContainsKey(node.FullPath))
            //{
            //    map.Add(node.FullPath, true);
            //}
            foreach (TreeNode node1 in node.Nodes)
            {
                markAllDiff(node1, false, notfind);
            }
            while (addPPath && node.Parent != null)
            {
                if (!map.ContainsKey(node.Parent.FullPath))
                {
                    map.Add(node.Parent.FullPath, true);
                }
                node = node.Parent;
            }
            //node.TreeView.Refresh();
        }

        //差异的节点的父节点收起时点亮
        private void compTreeView_AfterExpandChange(object sender, TreeViewEventArgs e)
        {
            Dictionary<string, bool> map = diffPathMap[e.Node.TreeView.Name];
            if (map.ContainsKey(e.Node.FullPath))
            {
                compTreeViewLeft.BeginUpdate();
                compTreeViewRight.BeginUpdate();
                if (e.Node.IsExpanded)
                {
                    e.Node.BackColor = defaultNodeColor;
                }
                else
                {
                    e.Node.BackColor = DIFF_COLOR;
                }
                compTreeViewLeft.EndUpdate();
                compTreeViewRight.EndUpdate();
            }
        }

        private void panel1_Click(object sender, EventArgs e)
        {


        }

        /// <summary>
        /// 选中差异节点时候动作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void compTreeView_AfterCheck(object sender, TreeViewEventArgs e)
        {
            Dictionary<TreeNode, TreeNode> map = e.Node.TreeView == compTreeViewLeft ? diffRefMapLeft : diffRefMapRight;
            cancelPrevHighLight();
            if (map.ContainsKey(e.Node))
            {
                showNodeDiff(e.Node);
            }
        }


        private void CompareWindow_Paint(object sender, PaintEventArgs e)
        {

        }

        private void CompareWindow_Activated(object sender, EventArgs e)
        {
            panel1.Focus();
        }

        private void CompareWindow_Click(object sender, EventArgs e)
        {
            cancelPrevHighLight();
        }

        private void splitContainer2_Panel_Scroll(object sender, ScrollEventArgs e)
        {
            //this.Text = e.OldValue+","+e.NewValue;
        }


    }
}
