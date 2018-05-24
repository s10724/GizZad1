using GizCore;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.WpfGraphControl;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace GizZad1Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GraphCore graph = new GraphCore();
        GraphViewer graphViewer;
        Graph graphVisualizer;
        public MainWindow()
        {
            InitializeComponent();
            GenerateStartGraph();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            graphViewer = new GraphViewer();
            graphViewer.BindToPanel(GraphDockPanel);


            graphViewer.ObjectUnderMouseCursorChanged += GraphViewer_ObjectUnderMouseCursorChanged;
            graphViewer.MouseUp += GraphViewer_MouseUp;


            DrawGraph();
        }


        private void CheckObject()
        {
            var newObjectId = graphViewer.ObjectUnderMouseCursor?.ToString();
            if (newObjectId != null && lastHoverId != newObjectId)
            {
                lastHoverId = newObjectId;
            }

            var isMarked = graphViewer.ObjectUnderMouseCursor?.MarkedForDragging;
            if (isMarked.HasValue)
            {
                if (isMarked.Value && lastSelectedId != lastHoverId)
                {
                    if (lastSelectedId != null)
                    {
                        Debug.WriteLine("Deselect: " + lastSelectedId);
                    }
                    lastSelectedId = lastHoverId;
                    Debug.WriteLine("Select: " + lastHoverId);
                }
                else if (!isMarked.Value && lastSelectedId == lastHoverId)
                {
                    lastSelectedId = string.Empty;
                    Debug.WriteLine("Deselect: " + lastHoverId);
                }

            }
        }

        private void GraphViewer_MouseUp(object sender, MsaglMouseEventArgs e)
        {
            string selectedNode = lastSelectedId;
            CheckObject();
            if (!string.IsNullOrWhiteSpace(lastSelectedId))
            {
                int lastSelectedGraphId = int.Parse(lastSelectedId);
                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    if (!string.IsNullOrWhiteSpace(selectedNode))
                    {
                        var first = graph.FindVertex(selectedNode);
                        var second = graph.FindVertex(lastSelectedId);

                        if (first?.Degree == 0 || second?.Degree == 0)
                        {
                            graph.ConnectVertex(first, second);
                        }
                    }
                    else
                    {
                        var vertex = graph.AddVertex();
                        graph.ConnectVertex(lastSelectedGraphId, vertex.Id);
                    }
                    DrawGraph();
                }
                else if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                {
                    graph.RemoveVertex(graph.Vertices.Where(x => x.Id == lastSelectedGraphId).FirstOrDefault());
                    DrawGraph();
                }
            }
        }

        private string lastSelectedId;
        private string lastHoverId;
        private void GraphViewer_ObjectUnderMouseCursorChanged(object sender, ObjectUnderMouseCursorChangedEventArgs e)
        {

            CheckObject();
        }

        private void DrawGraph()
        {
            graphVisualizer = new Graph();
            graphVisualizer.Attr.LayerDirection = LayerDirection.TB;

            foreach (var item in graph.Vertices)
            {
                graphVisualizer.AddNode(item.Id.ToString());
            }

            foreach (var item in graph.Edges)
            {
                var edge = graphVisualizer.AddEdge(item.First.Id.ToString(), item.Second.Id.ToString());
                edge.Attr.ArrowheadAtSource = edge.Attr.ArrowheadAtTarget = ArrowStyle.None;
            }


            graphViewer.Graph = graphVisualizer;


            PruferCodeTextBox.Text = string.Join(" ; ", graph.GeneratePruferCode());

        }

        private void GenerateStartGraph()
        {
            graph = new GraphCore();

            for (int i = 0; i < 6; i++)
                graph.AddVertex();

            graph.ConnectVertex(1, 4);
            graph.ConnectVertex(2, 4);
            graph.ConnectVertex(3, 4);
            graph.ConnectVertex(4, 5);
            graph.ConnectVertex(5, 6);
        }

        private void GenerateGraphFromPruferCode_Click(object sender, RoutedEventArgs e)
        {
            List<int> pruferCode = new List<int>();
            string rawPrufer = PruferCodeTextBox.Text;
            var arr = rawPrufer.Split(';');
            foreach (var item in arr)
            {
                int value;
                string str = item.Trim();
                if (!string.IsNullOrWhiteSpace(str) && int.TryParse(str, out value))
                {
                    pruferCode.Add(value);
                }
            }

            if (pruferCode.Count > 0)
            {
                graph.DecodePruferCode(pruferCode);
                DrawGraph();
            }
        }
    }
}
