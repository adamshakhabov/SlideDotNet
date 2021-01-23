﻿using System.Linq;
using ShapeCrawler.Models;
using Xunit;

// ReSharper disable TooManyChainedReferences

namespace ShapeCrawler.Tests.Unit
{
    public class TestFile_014
    {

        [Fact]
        public void Slide_Elements_Test()
        {
            // ARRANGE
            var pre = PresentationSc.Open(Properties.Resources._014, false);

            // ACT-ASSERT
            var elements = pre.Slides[2].Shapes;
        }

        [Fact]
        public void FontHeight_Test1()
        {
            // Arrange
            var pre = PresentationSc.Open(Properties.Resources._014, false);

            // Act
            var element = pre.Slides[3].Shapes.Single(x => x.Id == 5);
            var fh = element.TextBox.Paragraphs.First().Portions.First().Font.Size;

            // Assert
            Assert.Equal(1200, fh);
        }

        [Fact]
        public void FontHeight_Test2()
        {
            // Arrange
            var pre = PresentationSc.Open(Properties.Resources._014, false);

            // Act
            var element = pre.Slides[4].Shapes.Single(x => x.Id == 4);
            var fh = element.TextBox.Paragraphs.First().Portions.First().Font.Size;

            // Assert
            Assert.Equal(1200, fh);
        }

        [Fact]
        public void Title_FontHeight_Test()
        {
            // Arrange
            var pre = PresentationSc.Open(Properties.Resources._014, false);

            // Act
            var element = pre.Slides[5].Shapes.Single(x => x.Id == 52);
            var fh = element.TextBox.Paragraphs.First().Portions.First().Font.Size;

            // Assert
            Assert.Equal(2700, fh);
        }
    }
}
