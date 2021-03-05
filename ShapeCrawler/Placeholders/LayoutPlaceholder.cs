﻿using DocumentFormat.OpenXml;
using ShapeCrawler.Extensions;
using ShapeCrawler.Shared;
using P = DocumentFormat.OpenXml.Presentation;

namespace ShapeCrawler.Placeholders
{
    internal class LayoutPlaceholder : Placeholder
    {
        private readonly ResettableLazy<Shape> _masterShape;

        private LayoutPlaceholder(P.PlaceholderShape pPlaceholderShape, LayoutShape layoutShape)
            : base(pPlaceholderShape)
        {
            _masterShape = new ResettableLazy<Shape>(() =>
                layoutShape.SlideLayout.SlideMaster.Shapes.GetShapeByPPlaceholderShape(pPlaceholderShape));
        }

        private LayoutPlaceholder(P.PlaceholderShape pPlaceholderShape, LayoutAutoShape layoutAutoShape) 
            : base(pPlaceholderShape)
        {
            _masterShape = new ResettableLazy<Shape>(() =>
                layoutAutoShape.Slide.SlideLayout.Shapes.GetShapeByPPlaceholderShape(pPlaceholderShape));
        }

        /// <summary>
        ///     Creates placeholder. Returns <c>NULL</c> if the specified shape is not placeholder.
        /// </summary>
        internal static LayoutPlaceholder Create(OpenXmlCompositeElement pShapeTreeChild, LayoutShape slideShape)
        {
            P.PlaceholderShape pPlaceholderShape =
                pShapeTreeChild.ApplicationNonVisualDrawingProperties().GetFirstChild<P.PlaceholderShape>();
            if (pPlaceholderShape == null)
            {
                return null;
            }

            return new LayoutPlaceholder(pPlaceholderShape, slideShape);
        }

        public static LayoutPlaceholder Create(LayoutAutoShape layoutAutoShape)
        {
            P.PlaceholderShape pPlaceholderShape = layoutAutoShape.PShapeTreeChild.ApplicationNonVisualDrawingProperties().GetFirstChild<P.PlaceholderShape>();
            if (pPlaceholderShape == null)
            {
                return null;
            }

            return new LayoutPlaceholder(pPlaceholderShape, layoutAutoShape);
        }
    }
}