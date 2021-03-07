﻿using System;
using ShapeCrawler.Placeholders;
using ShapeCrawler.Shapes;
using ShapeCrawler.SlideMaster;
using P = DocumentFormat.OpenXml.Presentation;

namespace ShapeCrawler.Drawing
{
    internal class LayoutPicture : LayoutShape, IShape
    {
        public LayoutPicture(SlideLayoutSc slideLayout, P.Picture pPicture) : base(slideLayout, pPicture)
        {
        }

        public string Name => throw new NotImplementedException();

        public bool Hidden => throw new NotImplementedException();

        public IPlaceholder Placeholder => throw new NotImplementedException();

        public string CustomData
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
    }
}