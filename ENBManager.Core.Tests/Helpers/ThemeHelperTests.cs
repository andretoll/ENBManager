using ENBManager.Core.Helpers;
using NUnit.Framework;
using System.Linq;

namespace ENBManager.Core.Tests.Helpers
{
    [TestFixture]
    public class ThemeHelperTests
    {
        [Test]
        public void ShouldGetColorSchemes()
        {
            // Act
            var colorSchemes = ThemeHelper.GetColorSchemes();

            // Assert
            Assert.That(colorSchemes.Count(), Is.GreaterThan(0));
        }
    }
}
