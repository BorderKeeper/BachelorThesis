using System;
using System.Drawing;
using System.Windows.Forms;

namespace BachelorThesis.Network.UI
{
    public partial class BachelorThesisWindow : Form
    {
        private const string SavePath = "D:\\BachelorThesis\\networks";

        private const string NetworkName = "UINet";

        private Graphics _graphics;

        private int LayerOffset = 20;
        private int NeuronOffset = 5;

        public BachelorThesisWindow()
        {
            InitializeComponent();

            _graphics = CreateGraphics();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Graphics graphics = CreateGraphics();

            NeuralNetwork neuralNetwork = NeuralNetwork.Load(SavePath, NetworkName);

            int secondToLastLayer = neuralNetwork.Neurons.GetLength(0) - 2;
            for (int layer = secondToLastLayer; layer < neuralNetwork.Neurons.GetLength(0); layer++)
            {
                for (int neuron = 0; neuron < neuralNetwork.Neurons[layer].Length; neuron++)
                {
                    var neuronObject = neuralNetwork.Neurons[layer][neuron];

                    DrawNeuron(layer, neuron, neuronObject);

                    if (layer != neuralNetwork.Neurons.GetLength(0) - 1)
                    {
                        for (int nextNeuron = 0; nextNeuron < neuralNetwork.Neurons[layer + 1].Length; nextNeuron++)
                        {
                            var weight = neuralNetwork.Weights[layer][neuron, nextNeuron];

                            DrawWeight(weight);
                        }
                    }
                }
            }
        }

        private void DrawNeuron(int layer, int neuron, Neuron neuronObject)
        {
            int size = 20;

            var absBias = (int) Math.Abs(neuronObject.Output * 30);

            if (absBias > 255) absBias = 255;

            var redBias = 0;
            var blueBias = 0;
            if (neuronObject.Bias < 0) redBias = absBias;
            else blueBias = absBias;

            var neuronColor = Color.FromArgb(absBias, redBias, blueBias, 0);

            var x = layer * (size + LayerOffset);
            var y = neuron * (size + NeuronOffset);

            _graphics.FillEllipse(new SolidBrush(neuronColor), new Rectangle(y, x, size, size));
            _graphics.DrawEllipse(new Pen(Color.Black), new Rectangle(y, x, size, size));
        }

        private void DrawWeight(double weight)
        {
        }

        private void DrawPixel(int x, int y, Color c)
        {
            var brush = new SolidBrush(c);

            _graphics.FillRectangle(brush, x, y, 1, 1);
        }

        private void BachelorThesisWindow_Load(object sender, EventArgs e)
        {

        }
    }
}
