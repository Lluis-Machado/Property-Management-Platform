﻿// This file was auto-generated by ML.NET Model Builder. 
using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
namespace InvoiceItemAnalyzerAPI
{
    public partial class InvoiceItemModel
    {
        /// <summary>
        /// model input class for InvoiceItemModel.
        /// </summary>
        #region model input class
        public class ModelInput
        {
            [ColumnName(@"Id")]
            public float Id { get; set; }

            [ColumnName(@"VendorName")]
            public string VendorName { get; set; }

            [ColumnName(@"VendorTaxId")]
            public string VendorTaxId { get; set; }

            [ColumnName(@"ItemDescription")]
            public string ItemDescription { get; set; }

            [ColumnName(@"HasPeriod")]
            public bool HasPeriod { get; set; }

            [ColumnName(@"CategoryId")]
            public float CategoryId { get; set; }

        }

        #endregion

        /// <summary>
        /// model output class for InvoiceItemModel.
        /// </summary>
        #region model output class
        public class ModelOutput
        {
            [ColumnName(@"Id")]
            public float Id { get; set; }

            [ColumnName(@"VendorName")]
            public float[] VendorName { get; set; }

            [ColumnName(@"VendorTaxId")]
            public float[] VendorTaxId { get; set; }

            [ColumnName(@"ItemDescription")]
            public float[] ItemDescription { get; set; }

            [ColumnName(@"HasPeriod")]
            public float HasPeriod { get; set; }

            [ColumnName(@"CategoryId")]
            public uint CategoryId { get; set; }

            [ColumnName(@"Features")]
            public float[] Features { get; set; }

            [ColumnName(@"PredictedLabel")]
            public float PredictedLabel { get; set; }

            [ColumnName(@"Score")]
            public float[] Score { get; set; }

        }

        #endregion

        private static string MLNetModelPath = Path.GetFullPath("InvoiceItemModel.zip");

        public static readonly Lazy<PredictionEngine<ModelInput, ModelOutput>> PredictEngine = new Lazy<PredictionEngine<ModelInput, ModelOutput>>(() => CreatePredictEngine(), true);

        /// <summary>
        /// Use this method to predict on <see cref="ModelInput"/>.
        /// </summary>
        /// <param name="input">model input.</param>
        /// <returns><seealso cref=" ModelOutput"/></returns>
        public static ModelOutput Predict(ModelInput input)
        {
            var predEngine = PredictEngine.Value;
            return predEngine.Predict(input);
        }

        private static PredictionEngine<ModelInput, ModelOutput> CreatePredictEngine()
        {
            var mlContext = new MLContext();
            ITransformer mlModel = mlContext.Model.Load(MLNetModelPath, out var _);
            return mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(mlModel);
        }
    }
}
