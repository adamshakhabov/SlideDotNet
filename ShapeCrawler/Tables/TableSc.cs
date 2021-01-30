﻿using System;
using System.Collections.Generic;
using ShapeCrawler.Models.SlideComponents;
using ShapeCrawler.SlideMaster;
using A = DocumentFormat.OpenXml.Drawing;
using P = DocumentFormat.OpenXml.Presentation;
// ReSharper disable SuggestVarOrType_BuiltInTypes

namespace ShapeCrawler.Tables
{
    /// <summary>
    /// Represents a table element on a slide.
    /// </summary>
    public class TableSc
    {
        #region Fields

        private readonly P.GraphicFrame _pGraphicFrame;

        #endregion Fields

        #region Public Properties

        public RowCollection Rows => GetRowsCollection();

        #endregion Public Properties

        internal ShapeSc Shape { get; set; }

        #region Constructors

        internal TableSc(P.GraphicFrame pGraphicFrame)
        {
            _pGraphicFrame = pGraphicFrame ?? throw new ArgumentNullException(nameof(pGraphicFrame));
        }

        #endregion Constructors

        #region Private Methods

        private RowCollection GetRowsCollection()
        {
            A.Table aTable = _pGraphicFrame.GetFirstChild<A.Graphic>().GraphicData.GetFirstChild<A.Table>();
            IEnumerable<A.TableRow> tableRows = aTable.Elements<A.TableRow>();

            return new RowCollection(tableRows);
        }

        #endregion Private Methods

        public CellSc this[int rowIndex, int columnIndex] => Rows[0].Cells[0];

        public void MergeCells(CellSc cell1, CellSc cell2)
        {
            if (cell1 == cell2)
            {
                return;
            }

            int minRowIndex = cell1.FirstRowIndex;
            if (cell2.FirstRowIndex < cell1.FirstRowIndex)
            {
                minRowIndex = cell2.FirstRowIndex;
            }
            int maxRowIndex = cell1.FirstRowIndex;
            if (cell2.FirstRowIndex > cell1.FirstRowIndex)
            {
                maxRowIndex = cell2.FirstRowIndex;
            }

            int minColIndex = cell1.FirstColIndex;
            if (cell2.FirstColIndex < cell1.FirstColIndex)
            {
                minColIndex = cell2.FirstColIndex;
            }
            int maxColIndex = cell1.FirstColIndex;
            if (cell2.FirstColIndex > cell1.FirstColIndex)
            {
                maxColIndex = cell2.FirstColIndex;
            }

            for (int rowIdx = minRowIndex; rowIdx <= maxRowIndex; rowIdx++)
            {
                for (int colIdx = minColIndex; colIdx < maxColIndex; colIdx++)
                {
                    this[rowIdx, colIdx].SetMerged();
                }
            }
        }
    }
}