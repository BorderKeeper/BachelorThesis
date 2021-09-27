﻿using System;
using System.Collections.Generic;
using BachelorThesis.DataAnalysis.Entities;
using BachelorThesis.Network;

namespace BachelorThesis.DataAnalysis
{
    class Program
    {
        private static SqlDataProvider _dal;

        static void Main(string[] args)
        {
            _dal = new SqlDataProvider();

            var hiddenLayer = GetHiddenLayer();

            var hiddenLayerValues = GetHiddenLayerValues();

            var weights = GetHiddenLayerWeights();

            var networks = GetNetworks();

            var features = GetFeatures();

            var results = GetResults();
        }

        private static List<HiddenLayer> GetHiddenLayer()
        {
            var hiddenLayer = new List<HiddenLayer>();

            using var reader = _dal.Read("SELECT * FROM last_hidden_layer");

            while (reader.Read())
            {
                hiddenLayer.Add(new HiddenLayer(reader));
            }

            return hiddenLayer;
        }

        private static List<HiddenLayerValues> GetHiddenLayerValues()
        {
            var hiddenLayerValues = new List<HiddenLayerValues>();

            using var reader = _dal.Read("SELECT * FROM last_hidden_layer_values");

            while (reader.Read())
            {
                hiddenLayerValues.Add(new HiddenLayerValues(reader));
            }

            return hiddenLayerValues;
        }

        private static List<HiddenWeights> GetHiddenLayerWeights()
        {
            var hiddenLayerValues = new List<HiddenWeights>();

            using var reader = _dal.Read("SELECT * FROM last_hidden_weights");

            while (reader.Read())
            {
                hiddenLayerValues.Add(new HiddenWeights(reader));
            }

            return hiddenLayerValues;
        }

        private static List<Entities.Network> GetNetworks()
        {
            var hiddenLayerValues = new List<Entities.Network>();

            using var reader = _dal.Read("SELECT * FROM network");

            while (reader.Read())
            {
                hiddenLayerValues.Add(new Entities.Network(reader));
            }

            return hiddenLayerValues;
        }

        private static List<OcrFeature> GetFeatures()
        {
            var hiddenLayerValues = new List<OcrFeature>();

            using var reader = _dal.Read("SELECT * FROM ocr_features");

            while (reader.Read())
            {
                hiddenLayerValues.Add(new OcrFeature(reader));
            }

            return hiddenLayerValues;
        }

        private static List<OcrResult> GetResults()
        {
            var hiddenLayerValues = new List<OcrResult>();

            using var reader = _dal.Read("SELECT * FROM ocr_results");

            while (reader.Read())
            {
                hiddenLayerValues.Add(new OcrResult(reader));
            }

            return hiddenLayerValues;
        }
    }
}