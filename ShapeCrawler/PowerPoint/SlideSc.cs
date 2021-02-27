﻿using System;
using System.IO;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using ShapeCrawler.Collections;
using ShapeCrawler.Drawing;
using ShapeCrawler.Factories;
using ShapeCrawler.Models;
using ShapeCrawler.SlideMaster;
using ShapeCrawler.Statics;
using SkiaSharp;

// ReSharper disable CheckNamespace
// ReSharper disable PossibleMultipleEnumeration

namespace ShapeCrawler
{
    /// <summary>
    ///     Represents a slide.
    /// </summary>
    public class SlideSc : ISlide
    {
        #region Fields

        private readonly Lazy<ImageSc> _backgroundImage;
        private readonly Lazy<ShapeCollection> _shapes;
        private readonly SlideNumber _sldNumEntity;
        private Lazy<CustomXmlPart> _customXmlPart;

        internal PresentationSc Presentation { get; }
        internal SlidePart SlidePart { get; }

        #endregion Fields

        #region Properties

        /// <summary>
        ///     Returns a slide shapes.
        /// </summary>
        public ShapeCollection Shapes => _shapes.Value;

        /// <summary>
        ///     Returns a slide number in presentation.
        /// </summary>
        public int Number => _sldNumEntity.Number;

        /// <summary>
        ///     Returns a background image of the slide. Returns <c>null</c>if slide does not have background image.
        /// </summary>
        public ImageSc Background => _backgroundImage.Value;

        public string CustomData
        {
            get => GetCustomData();
            set => SetCustomData(value);
        }

        public bool Hidden => SlidePart.Slide.Show != null && SlidePart.Slide.Show.Value == false;

#if DEBUG
        public static SlideLayoutSc Layout => GetSlideLayout();

        private static SlideLayoutSc GetSlideLayout()
        {
            return null;
        }
#endif

        #endregion Properties

        #region Constructors

        internal SlideSc(
            SlidePart slidePart,
            SlideNumber slideNumber,
            PresentationSc presentationEx) :
            this(slidePart, slideNumber, new SlideSchemeService(), presentationEx)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SlideSc" /> class.
        /// </summary>
        internal SlideSc(
            SlidePart sdkSldPart,
            SlideNumber sldNum,
            SlideSchemeService schemeService,
            PresentationSc presentationEx)
        {
            SlidePart = sdkSldPart ?? throw new ArgumentNullException(nameof(sdkSldPart));
            _sldNumEntity = sldNum ?? throw new ArgumentNullException(nameof(sldNum));
            _shapes = new Lazy<ShapeCollection>(GetShapesCollection);
            _backgroundImage = new Lazy<ImageSc>(TryGetBackground);
            _customXmlPart = new Lazy<CustomXmlPart>(GetSldCustomXmlPart);
            Presentation = presentationEx;
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        ///     Saves slide scheme in PNG file.
        /// </summary>
        /// <param name="filePath"></param>
        public void SaveScheme(string filePath)
        {
            SlideSchemeService.SaveScheme(_shapes.Value, Presentation.SlideWidth, Presentation.SlideHeight, filePath);
        }

        /// <summary>
        ///     Saves slide scheme in stream.
        /// </summary>
        /// <param name="stream"></param>
        public void SaveScheme(Stream stream)
        {
            SlideSchemeService.SaveScheme(_shapes.Value, Presentation.SlideWidth, Presentation.SlideHeight, stream);
        }
#if DEBUG
        public void SaveImage(string filePath)
        {
            ShapeCollection shapes = Shapes;

            SKImageInfo imageInfo = new SKImageInfo(500, 600);
            using SKSurface surface = SKSurface.Create(imageInfo);
            SKCanvas canvas = surface.Canvas;

            canvas.Clear(SKColors.Red);

            using SKPaint paint = new SKPaint
            {
                Color = SKColors.Blue,
                IsAntialias = true,
                StrokeWidth = 15,
                Style = SKPaintStyle.Stroke
            };
            canvas.DrawCircle(70, 70, 50, paint);

            using SKPaint textPaint = new SKPaint();
            textPaint.Color = SKColors.Green;
            textPaint.IsAntialias = true;
            textPaint.TextSize = 48;

            using SKImage image = surface.Snapshot();
            using SKData data = image.Encode(SKEncodedImageFormat.Png, 100);
            File.WriteAllBytes(filePath, data.ToArray());
        }
#endif

        public void Hide()
        {
            if (SlidePart.Slide.Show == null)
            {
                var showAttribute = new OpenXmlAttribute("show", "", "0");
                SlidePart.Slide.SetAttribute(showAttribute);
            }
            else
            {
                SlidePart.Slide.Show = false;
            }
        }

        #endregion Public Methods

        #region Private Methods

        private ShapeCollection GetShapesCollection()
        {
            return ShapeCollection.CreateForUserSlide(SlidePart, this);
        }

        private ImageSc TryGetBackground()
        {
            var backgroundImageFactory = new ImageExFactory();
            return backgroundImageFactory.TryFromSdkSlide(SlidePart);
        }

        private string GetCustomData()
        {
            if (_customXmlPart.Value == null)
            {
                return null;
            }

            var customXmlPartStream = _customXmlPart.Value.GetStream();
            using var customXmlStreamReader = new StreamReader(customXmlPartStream);
            var raw = customXmlStreamReader.ReadToEnd();
#if NET5_0
            return raw[ConstantStrings.CustomDataElementName.Length..];
#else
            return raw.Substring(ConstantStrings.CustomDataElementName.Length);
#endif
        }

        private void SetCustomData(string value)
        {
            Stream customXmlPartStream;
            if (_customXmlPart.Value == null)
            {
                var newSlideCustomXmlPart = SlidePart.AddCustomXmlPart(CustomXmlPartType.CustomXml);
                customXmlPartStream = newSlideCustomXmlPart.GetStream();
#if NETSTANDARD2_0
                _customXmlPart = new Lazy<CustomXmlPart>(() => newSlideCustomXmlPart);
#else
                _customXmlPart = new Lazy<CustomXmlPart>(newSlideCustomXmlPart);
#endif
            }
            else
            {
                customXmlPartStream = _customXmlPart.Value.GetStream();
            }

            using var customXmlStreamReader = new StreamWriter(customXmlPartStream);
            customXmlStreamReader.Write($"{ConstantStrings.CustomDataElementName}{value}");
        }

        private CustomXmlPart GetSldCustomXmlPart()
        {
            foreach (var customXmlPart in SlidePart.CustomXmlParts)
            {
                using var customXmlPartStream = new StreamReader(customXmlPart.GetStream());
                string customXmlPartText = customXmlPartStream.ReadToEnd();
                if (customXmlPartText.StartsWith(ConstantStrings.CustomDataElementName,
                    StringComparison.CurrentCulture))
                {
                    return customXmlPart;
                }
            }

            return null;
        }

        #endregion Private Methods
    }
}