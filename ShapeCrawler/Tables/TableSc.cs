﻿using System;
using System.Collections.Generic;
using ShapeCrawler.Models.SlideComponents;
using ShapeCrawler.SlideMaster;
using A = DocumentFormat.OpenXml.Drawing;
using P = DocumentFormat.OpenXml.Presentation;

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
    }
}