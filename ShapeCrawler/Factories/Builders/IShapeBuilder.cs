﻿using System.Collections.Generic;
using ShapeCrawler.Charts;
using ShapeCrawler.Enums;
using ShapeCrawler.Models.SlideComponents;
using ShapeCrawler.Settings;
using ShapeCrawler.Tables;
using OleObject = ShapeCrawler.Models.SlideComponents.OleObject;

namespace ShapeCrawler.Factories.Builders
{
    /// <summary>
    /// Represents a shape builder.
    /// </summary>
    public interface IShapeBuilder
    {
        /// <summary>
        /// Builds a shape with OLE object content.
        /// </summary>
        ShapeSc WithOle(ILocation innerTransform, ShapeContext spContext, OleObject ole);

        /// <summary>
        /// Builds a shape with picture content.
        /// </summary>
        ShapeSc WithPicture(ILocation innerTransform, ShapeContext spContext, PictureSc picture, GeometryType geometry);

        /// <summary>
        /// Builds a AutoShape.
        /// </summary>
        ShapeSc WithAutoShape(ILocation innerTransform, ShapeContext spContext, GeometryType geometry);

        /// <summary>
        /// Builds a shape with table content.
        /// </summary>
        ShapeSc WithTable(ILocation innerTransform, ShapeContext spContext, TableSc table);

        /// <summary>
        /// Builds a shape with OLE object content.
        /// </summary>
        ShapeSc WithChart(ILocation innerTransform, ShapeContext spContext, ChartSc chart);

        /// <summary>
        /// Builds a group shape which has grouped shape items.
        /// </summary>
        ShapeSc WithGroup(ILocation innerTransform, ShapeContext spContext, IList<ShapeSc> groupedShapes);
    }
}
