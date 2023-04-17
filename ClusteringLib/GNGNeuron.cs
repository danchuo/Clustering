﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItemLib;
using EuclideanGeometryLib;
using RandomAlgoLib;
using AlgorithmLib;
using System.Drawing;
using System.Threading;

namespace ClusteringLib
{
    public class GNGNeuron : ClusteringNeuron
    {
        double Error;
        List<GNGNeuron> Neighbours = new List<GNGNeuron>();
        Dictionary<GNGNeuron, int> Ages = new Dictionary<GNGNeuron, int>();
        public GNGNeuron(double[] coordinates) : base(coordinates)
        {
            Error = 0;
        }
        public GNGNeuron(double[] coordinates, double _Error) : base(coordinates)
        {
            Error = _Error;
        }
        public double GetError
        {
            get
            {
                return Error;
            }
        }
        public void IncreaseError(Item item)
        {
            Error += EuclideanGeometry.Distance(item.GetCoordinates, GetCoordinates());
        }
        public void MultiplyError(double x)
        {
            Error *= x;
        }
        public void Connect(GNGNeuron neuron)
        {
            PartialConnect(neuron);
            neuron.PartialConnect(this);
        }
        public void PartialConnect(GNGNeuron neuron)
        {
            if (Neighbours.FindIndex(x => x == neuron) == -1) Neighbours.Add(neuron);
            Ages[neuron] = 0;
        }
        public void Disconnect(GNGNeuron neuron)
        {
            PartialDisconnect(neuron);
            neuron.PartialDisconnect(this);
        }
        public void PartialDisconnect(GNGNeuron neuron)
        {
            Neighbours.RemoveAt(Neighbours.FindIndex(x => x == neuron));
            Ages.Remove(neuron);
        }
        public void LearnNeighbours(Item item, double learningSpeed)
        {
            foreach (var neighbour in Neighbours)
            {
                neighbour.Learn(item.GetCoordinates, learningSpeed);
            }
        }
        public void IncreaseAges()
        {
            foreach (var neighbour in Neighbours)
            {
                ++Ages[neighbour];
            }
        }
        public void DeleteOldNeighbours(int maxAge)
        {
            for (int i = 0; i < Neighbours.Count; ++i)
            {
                if (Ages[Neighbours[i]] > maxAge)
                {
                    Disconnect(Neighbours[i]);
                    --i;
                }
            }
        }
        public bool NoNeighbours
        {
            get
            {
                return Neighbours.Count == 0;
            }
        }
        public GNGNeuron FindMostIncorrectNeighbour()
        {
            int result = 0;
            double maxError = Neighbours[0].GetError;
            for (int i = 1; i < Neighbours.Count; ++i)
            {
                if (Neighbours[i].GetError > maxError)
                {
                    result = i;
                    maxError = Neighbours[result].GetError;
                }
            }
            return Neighbours[result];
        }
        public delegate void del1();
        public void GetComponent(List<GNGNeuron> Component, Dictionary<GNGNeuron, bool> UsedNeurons)
        {
            if (UsedNeurons[this]) return;
            Component.Add(this);
            UsedNeurons[this] = true;
            foreach (var neighbour in Neighbours)
            {
                neighbour.GetComponent(Component, UsedNeurons);
            }
        }
    }//class GNGNeuron
}