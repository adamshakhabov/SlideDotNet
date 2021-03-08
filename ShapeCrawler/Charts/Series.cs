﻿using System;
using System.Collections.Generic;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using ShapeCrawler.Exceptions;
using ShapeCrawler.Spreadsheet;
using C = DocumentFormat.OpenXml.Drawing.Charts;

// ReSharper disable PossibleMultipleEnumeration

namespace ShapeCrawler.Charts
{
    /// <summary>
    ///     Represents a chart series.
    /// </summary>
    public class Series
    {
        #region Constructors

        internal Series(SlideChart slideChart, ChartType type, OpenXmlElement seriesXmlElement,
            ChartReferencesParser chartRefParser)
        {
            SlideChart = slideChart;
            _seriesXmlElement = seriesXmlElement;
            _chartRefParser = chartRefParser;
            _pointValues = new Lazy<IReadOnlyList<double>>(GetPointValues);
            _name = new Lazy<string>(GetNameOrDefault);
            Type = type;
        }

        #endregion Constructors

        internal SlideChart SlideChart { get; }

        /// <summary>
        ///     Gets chart type.
        /// </summary>
        public ChartType Type { get; }

        /// <summary>
        ///     Gets collection of point values.
        /// </summary>
        public IReadOnlyList<double> PointValues => _pointValues.Value;

        public bool HasName => _name.Value != null;

        public string Name
        {
            get
            {
                if (_name.Value == null)
                {
                    throw new NotSupportedException(ExceptionMessages.SeriesHasNotName);
                }

                return _name.Value;
            }
        }

        #region Fields

        private readonly Lazy<IReadOnlyList<double>> _pointValues;
        private readonly Lazy<string> _name;
        private readonly ChartPart _chartPart;
        private readonly OpenXmlElement _seriesXmlElement;
        private readonly ChartReferencesParser _chartRefParser;

        #endregion Fields

        #region Private Methods

        private IReadOnlyList<double> GetPointValues()
        {
            C.NumberReference numReference;
            C.Values cVal = _seriesXmlElement.GetFirstChild<C.Values>();
            if (cVal != null) // scatter type chart does not have <c:val> element
            {
                numReference = cVal.NumberReference;
            }
            else
            {
                numReference = _seriesXmlElement.GetFirstChild<C.YValues>().NumberReference;
            }

            return _chartRefParser.GetNumbersFromCacheOrSpreadsheet(numReference);
        }

        private string GetNameOrDefault()
        {
            var strReference = _seriesXmlElement.GetFirstChild<C.SeriesText>()?.StringReference;
            if (strReference == null)
            {
                return null;
            }

            return _chartRefParser.GetSingleString(strReference, _chartPart);
        }

        #endregion Private Methods
    }
}