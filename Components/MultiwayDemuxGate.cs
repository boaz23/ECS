using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
    class MultiwayDemuxGate : Gate
    {
        public int ControlBits { get; }
        public Wire[] Outputs { get; }
        public WireSet Control { get; }
        public Wire Input { get; }

        private readonly CompleteBinaryTree<Demux> m_demuxGates;

        public MultiwayDemuxGate(int cControlBits)
        {
            ControlBits = cControlBits;
            var demuxGates = new CompleteBinaryTree<Demux>(ControlBits - 1);
            Control = new WireSet(ControlBits);
            Input = new Wire();
            Outputs = new Wire[demuxGates.ItemsCount + 1];
            InitializeOutputs();

            InitializeDemuxGates(demuxGates);
            ConnectInput(demuxGates);
            ConnectOutputs(demuxGates);
            BuildDeuxGatesTree(demuxGates);

            m_demuxGates = demuxGates;
        }

        private void BuildDeuxGatesTree(CompleteBinaryTree<Demux> demuxGates)
        {
            int iControl = 0;
            for (int depth = demuxGates.Height; depth > 0; depth--)
            {
                Range range = demuxGates.GetDepthIndexRange(depth);
                for (int i = range.Start; i < range.End; i += 2)
                {
                    Demux parent = demuxGates.Parent(i);
                    demuxGates[i].Input.ConnectInput(parent.Output1);
                    demuxGates[i].ConnectControl(Control[iControl]);
                    demuxGates[i + 1].Input.ConnectInput(parent.Output2);
                    demuxGates[i + 1].ConnectControl(Control[iControl]);
                }

                iControl++;
            }
        }

        public void ConnectInput(Wire wInput)
        {
            Input.ConnectInput(wInput);
        }
        public void ConnectControl(WireSet wsControl)
        {
            Control.ConnectInput(wsControl);
        }

        public override bool TestGate()
        {
            return TestGate(0) && TestGate(1);
        }

        private void InitializeOutputs()
        {
            for (int i = 0; i < Outputs.Length; i++)
            {
                Outputs[i] = new Wire();
            }
        }

        private static void InitializeDemuxGates(CompleteBinaryTree<Demux> demuxGates)
        {
            for (int i = 0; i < demuxGates.ItemsCount; i++)
            {
                demuxGates[i] = new Demux();
            }
        }

        private void ConnectOutputs(CompleteBinaryTree<Demux> demuxGates)
        {
            int iOutput = 0;
            foreach (var itemIndexPair in demuxGates.GetDepthEnumerator(demuxGates.Height))
            {
                Demux demuxGate = itemIndexPair.Item;
                Outputs[iOutput].ConnectInput(demuxGate.Output1);
                Outputs[iOutput + 1].ConnectInput(demuxGate.Output2);

                iOutput += 2;
            }
        }

        private void ConnectInput(CompleteBinaryTree<Demux> demuxGates)
        {
            Demux demuxGate = demuxGates.Root;
            demuxGate.Input.ConnectInput(Input);
            demuxGate.ConnectControl(Control[ControlBits - 1]);
        }

        private bool TestGate(int input)
        {
            for (int control = 0; control < m_demuxGates.ItemsCount + 1; control++)
            {
                Control.SetValue(control);
                Input.Value = input;
                int[] expected = DoMultiwayDemux(input);
                if (!TestOutput(expected))
                {
                    return false;
                }
            }

            return true;
        }

        private int[] DoMultiwayDemux(int input)
        {
            int iOutput = Control.GetValue();
            int[] expected = new int[Outputs.Length];
            expected[iOutput] = input;
            return expected;
        }

        private bool TestOutput(int[] expected)
        {
            for (int i = 0; i < Outputs.Length; i++)
            {
                if (Outputs[i].Value != expected[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
