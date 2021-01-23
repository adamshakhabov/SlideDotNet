﻿using System;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing;
using ShapeCrawler.Enums;
using ShapeCrawler.Models;
using P = DocumentFormat.OpenXml.Presentation;

namespace ShapeCrawler.SlideMaster
{
    public class MasterShape : BaseShape
    {
        public MasterShape(OpenXmlCompositeElement compositeElement) : base(compositeElement)
        {
        }

        public override long X => _compositeElement.GetFirstChild<P.ShapeProperties>().Transform2D.Offset.X;

        public override long Y => _compositeElement.GetFirstChild<P.ShapeProperties>().Transform2D.Offset.Y;

        public override long Width => _compositeElement.GetFirstChild<P.ShapeProperties>().Transform2D.Extents.Cx;

        public override long Height => _compositeElement.GetFirstChild<P.ShapeProperties>().Transform2D.Extents.Cy;

        public override GeometryType GeometryType => GetGeometryType();

        /// <summary>
        /// Gets placeholder type. Returns null if the master shape is not a placeholder.
        /// </summary>
        public PlaceholderType? PlaceholderType => GetPlaceholderType();

        #region Private Methods

        private GeometryType GetGeometryType()
        {
            // Get the shape geometry type in SDK format
            PresetGeometry presetGeometry = _compositeElement.GetFirstChild<P.ShapeProperties>().
                                                                GetFirstChild<PresetGeometry>();
            
            if (presetGeometry == null)
            {
                return GeometryType.Custom;
            }

#if NETSTANDARD2_0
            Enum.TryParse(presetGeometry.Preset.Value.ToString(), true, out GeometryType geometryType);
#else
            // Get SDK format into internal type
            GeometryType geometryType = Enum.Parse<GeometryType>(presetGeometry.Preset.Value.ToString());
#endif

            return geometryType;
        }

        private PlaceholderType? GetPlaceholderType()
        {
            P.PlaceholderShape placeholderShape = _compositeElement.GetFirstChild<P.NonVisualShapeProperties>().ApplicationNonVisualDrawingProperties.PlaceholderShape;
            if (placeholderShape == null)
            {
                return null;
            }

            // Convert outer sdk placeholder type into library placeholder type
            if (placeholderShape.Type == P.PlaceholderValues.Title ||
                placeholderShape.Type == P.PlaceholderValues.CenteredTitle)
            {
                return Enums.PlaceholderType.Title;
            }

            return (PlaceholderType) Enum.Parse(typeof(PlaceholderType), placeholderShape.Type.Value.ToString());
        }

        #endregion Private Methods
    }
}