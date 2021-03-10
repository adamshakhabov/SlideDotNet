﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using ShapeCrawler.Charts;
using C = DocumentFormat.OpenXml.Drawing.Charts;

// ReSharper disable PossibleMultipleEnumeration

namespace ShapeCrawler.Spreadsheet
{
    internal class ChartReferencesParser
    {
        #region Internal Methods

        internal static IReadOnlyList<double> GetNumbersFromCacheOrSpreadsheet(C.NumberReference numberReference, SlideChart slideChart)
        {
            if (numberReference.NumberingCache != null)
            {
                // From cache
                IEnumerable<C.NumericValue> cNumericValues =
                    numberReference.NumberingCache.Descendants<C.NumericValue>();
                var pointValues = new List<double>(cNumericValues.Count());
                foreach (var numericValue in cNumericValues)
                {
                    var number = double.Parse(numericValue.InnerText, CultureInfo.InvariantCulture.NumberFormat);
                    var roundNumber = Math.Round(number, 1);
                    pointValues.Add(roundNumber);
                }

                return pointValues;
            }

            // From Spreadsheet
            List<string> cellStrValues = GetCellStrValues(numberReference.Formula, slideChart);
            var cellNumberValues = new List<double>(cellStrValues.Count); // TODO: consider allocate on stack
            foreach (string cellValue in cellStrValues)
            {
                string cellValueStr = cellValue;
                cellValueStr = cellValueStr.Length == 0 ? "0" : cellValueStr;
                cellNumberValues.Add(double.Parse(cellValueStr, CultureInfo.InvariantCulture.NumberFormat));
            }

            return cellNumberValues;
        }

        internal static string GetSingleString(C.StringReference stringReference, SlideChart slideChart)
        {
            string fromCache = stringReference.StringCache?.GetFirstChild<C.StringPoint>().Single().InnerText;
            if (fromCache != null)
            {
                return fromCache;
            }

            C.Formula cFormula = stringReference.Formula;
            List<string> cellStrValues = GetCellStrValues(cFormula, slideChart);

            return cellStrValues.Single();
        }

        #endregion Internal Methods

        #region Private Methods

        /// <summary>
        ///     Gets cell values.
        /// </summary>
        /// <param name="cFormula">
        ///     Cell range formula (c:f).
        ///     <c:cat>
        ///         <c:strRef>
        ///             <c:f>
        ///                 Sheet1!$A$2:$A$3
        ///             </c:f>
        ///         </c:strRef>
        ///     </c:cat>
        /// </param>
        /// <remarks>EmbeddedPackagePart : OpenXmlPart</remarks>
        private static List<string> GetCellStrValues(C.Formula cFormula, SlideChart slideChart)
        {
            Dictionary<EmbeddedPackagePart, SpreadsheetDocument> packPartToSpreadsheetDoc = slideChart.Slide.Presentation.PresentationData.SpreadsheetCache;
            EmbeddedPackagePart xlsxPackagePart = slideChart.ChartPart.EmbeddedPackagePart;
            bool cached = packPartToSpreadsheetDoc.TryGetValue(xlsxPackagePart, out var spreadSheetDoc);
            if (!cached)
            {
                spreadSheetDoc = SpreadsheetDocument.Open(xlsxPackagePart.GetStream(), false);
                packPartToSpreadsheetDoc.Add(xlsxPackagePart, spreadSheetDoc);
            }

            // Get all <x:c> elements of formula sheet
            string filteredFormula = GetFilteredFormula(cFormula);
            string[] sheetNameAndCellsFormula = filteredFormula.Split('!'); //eg: Sheet1!A2:A5 -> ['Sheet1', 'A2:A5']
            WorkbookPart workbookPart = spreadSheetDoc.WorkbookPart;
            string sheetId = workbookPart.Workbook.Sheets.Elements<Sheet>()
                .First(xSheet => sheetNameAndCellsFormula[0] == xSheet.Name).Id;
            var worksheetPart = (WorksheetPart) workbookPart.GetPartById(sheetId);
            IEnumerable<Cell> xCells = worksheetPart.Worksheet.GetFirstChild<SheetData>().ChildElements
                .SelectMany(e => e.Elements<Cell>()); //TODO: use HashSet

            List<string> formulaCellAddressList =
                new CellFormulaParser(sheetNameAndCellsFormula[1]).GetCellAddresses(); //eg: [1] = 'A2:A5'

            var xCellValues = new List<string>(formulaCellAddressList.Count);
            foreach (string address in formulaCellAddressList)
            {
                string xCellValue = xCells.First(xCell => xCell.CellReference == address).InnerText;
                xCellValues.Add(xCellValue);
            }

            return xCellValues;
        }

        private static string GetFilteredFormula(C.Formula formula)
        {
#if NETSTANDARD2_1 || NETCOREAPP2_0 || NET5_0
            var filteredFormula = formula.Text
                .Replace("'", string.Empty, StringComparison.OrdinalIgnoreCase)
                .Replace("$", string.Empty,
                    StringComparison.OrdinalIgnoreCase); //eg: Sheet1!$A$2:$A$5 -> Sheet1!A2:A5            
#else
            var filteredFormula = formula.Text.Replace("'", string.Empty).Replace("$", string.Empty);
#endif
            return filteredFormula;
        }

        #endregion Private Methods
    }
}