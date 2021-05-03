﻿using System.Collections.Generic;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using ShapeCrawler.SlideMasters;

namespace ShapeCrawler.Collections
{
    /// <summary>
    ///     Represents a collections of Slide Masters.
    /// </summary>
    public interface ISlideMasterCollection
    {
        /// <summary>
        ///     Gets the number of series items in the collection.
        /// </summary>
        int Count { get; }

        /// <summary>
        ///     Gets the element at the specified index.
        /// </summary>
        ISlideMaster this[int index] { get; }

        /// <summary>
        ///     Gets the generic enumerator that iterates through the collection.
        /// </summary>
        IEnumerator<ISlideMaster> GetEnumerator();
    }

    internal class SlideMasterCollection : ISlideMasterCollection // TODO: add interface
    {
        readonly List<ISlideMaster> slideMasters;
        
        private SlideMasterCollection(SCPresentation presentation, List<ISlideMaster> slideMasters)
        {
            Presentation = presentation;
            this.slideMasters = slideMasters;
        }

        internal SCPresentation Presentation { get; }

        internal static ISlideMasterCollection Create(SCPresentation presentation)
        {
            IEnumerable<SlideMasterPart> slideMasterParts = presentation.PresentationPart.SlideMasterParts;
            var slideMasters = new List<ISlideMaster>(slideMasterParts.Count());
            foreach (SlideMasterPart slideMasterPart in slideMasterParts)
            {
                slideMasters.Add(new SCSlideMaster(presentation, slideMasterPart.SlideMaster));
            }

            return new SlideMasterCollection(presentation, slideMasters);
        }

        internal SCSlideLayout GetSlideLayoutBySlide(SCSlide slide)
        {
            SlideLayoutPart inputSlideLayoutPart = slide.SlidePart.SlideLayoutPart;

            return this.slideMasters.SelectMany(sm => sm.SlideLayouts)
                .First(sl => sl.SlideLayoutPart == inputSlideLayoutPart);
        }

        internal ISlideMaster GetSlideMasterByLayout(SCSlideLayout slideLayout)
        {
            return this.slideMasters.First(sldMaster =>
                sldMaster.SlideLayouts.Any(sl => sl.SlideLayoutPart == slideLayout.SlideLayoutPart));
        }

        public ISlideMaster this[int index] => this.slideMasters[index];

        public int Count => this.slideMasters.Count;
        public IEnumerator<ISlideMaster> GetEnumerator()
        {
            return this.slideMasters.GetEnumerator();
        }
    }
}