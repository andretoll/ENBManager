using ENBManager.Infrastructure.Constants;
using ENBManager.Modules.Shared.Interfaces;
using ENBManager.Modules.Shared.Services;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;

namespace ENBManager.Modules.Tests.Shared.Services
{
    [TestFixture]
    public class PresetManagerTests
    {
        private IPresetManager _presetManager;
        private string _presetRoot;

        [SetUp]
        public void Setup()
        {
            _presetManager = new PresetManager();

            _presetRoot = Path.Combine(Paths.GetBaseDirectory(), "presetmanager_tests");
            Directory.CreateDirectory(_presetRoot);
        }

        [TearDown]
        public void Teardown()
        {
            Directory.Delete(Path.Combine(Paths.GetBaseDirectory(), "presetmanager_tests"), true);
        }

        [Test]
        public void ShouldReturnPresets()
        {
            // Arrange
            Directory.CreateDirectory(Path.Combine(_presetRoot, "Preset 1"));
            Directory.CreateDirectory(Path.Combine(_presetRoot, "Preset 2"));

            // Act
            var presets = _presetManager.GetPresets(_presetRoot);

            // Assert
            Assert.That(presets.Count(), Is.EqualTo(2));
        }

        [Test]
        public void ShouldHaveNameAndPathBasedOnDirectory()
        {
            // Arrange
            string presetName = "Preset 1";
            var dirInfo = Directory.CreateDirectory(Path.Combine(_presetRoot, presetName));

            // Act
            var presets = _presetManager.GetPresets(_presetRoot);

            // Assert
            Assert.That(presets.Single().Name, Is.EqualTo(presetName));
            Assert.That(presets.Single().FullPath, Is.EqualTo(dirInfo.FullName));
        }

        [Test]
        public void ShouldIncludeAllFiles()
        {
            // Arrange
            string presetName = "Preset 1";
            string fileName1 = "file1.ini";
            string fileName2 = "file1.txt";

            var dirInfo = Directory.CreateDirectory(Path.Combine(_presetRoot, presetName));
            CreateEmptyFile(Path.Combine(dirInfo.FullName, fileName1));
            CreateEmptyFile(Path.Combine(dirInfo.FullName, fileName2));

            // Act
            var preset = _presetManager.GetPresets(_presetRoot).Single();
            var files = preset.Files;

            // Assert
            Assert.That(files.Count(), Is.EqualTo(2));
            Assert.That(files.All(x => string.IsNullOrEmpty(x)), Is.False);
        }

        [Test]
        public void ShouldRenamePreset()
        {
            // Arrange
            string presetName = "Preset 1";
            string fileName1 = "file1.ini";
            string fileName2 = "file1.txt";
            var dirInfo = Directory.CreateDirectory(Path.Combine(_presetRoot, presetName));
            CreateEmptyFile(Path.Combine(dirInfo.FullName, fileName1));
            CreateEmptyFile(Path.Combine(dirInfo.FullName, fileName2));
            var preset = _presetManager.GetPresets(_presetRoot).Single();
            string newPresetName = "Preset 2";

            // Act
            _presetManager.RenamePreset(preset, newPresetName);

            // Assert
            Assert.That(Directory.Exists(Path.Combine(dirInfo.Parent.FullName, newPresetName)), Is.True);
            Assert.That(File.Exists(Path.Combine(dirInfo.Parent.FullName, newPresetName, fileName1)), Is.True);
            Assert.That(File.Exists(Path.Combine(dirInfo.Parent.FullName, newPresetName, fileName2)), Is.True);
        }

        [TestCase(null)]
        [TestCase("")]
        public void ShouldThrowExceptionWhenRenamingIfNewNameHasNullOrEmptyValue(string newPresetName)
        {
            // Arrange
            string presetName = "Preset 1";
            string fileName1 = "file1.ini";
            string fileName2 = "file1.txt";
            var dirInfo = Directory.CreateDirectory(Path.Combine(_presetRoot, presetName));
            CreateEmptyFile(Path.Combine(dirInfo.FullName, fileName1));
            CreateEmptyFile(Path.Combine(dirInfo.FullName, fileName2));
            var preset = _presetManager.GetPresets(_presetRoot).Single();

            // Assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                _presetManager.RenamePreset(preset, newPresetName);
            });
        }

        [Test]
        public void ShouldThrowExceptionWhenRenamingIfDirectoryDoesNotExist()
        {
            // Arrange
            string presetName = "Preset 1";
            string fileName1 = "file1.ini";
            string fileName2 = "file1.txt";
            var dirInfo = Directory.CreateDirectory(Path.Combine(_presetRoot, presetName));
            CreateEmptyFile(Path.Combine(dirInfo.FullName, fileName1));
            CreateEmptyFile(Path.Combine(dirInfo.FullName, fileName2));
            var preset = _presetManager.GetPresets(_presetRoot).Single();

            // Act
            Directory.Delete(preset.FullPath, true);

            // Assert
            Assert.Throws<DirectoryNotFoundException>(() =>
            {
                _presetManager.RenamePreset(preset, "Preset 2");
            });
        }

        [Test]
        public void ShouldThrowExceptionWhenRenamingIfNewNameEqualsOldName()
        {
            // Arrange
            string presetName = "Preset 1";
            string fileName1 = "file1.ini";
            string fileName2 = "file1.txt";
            var dirInfo = Directory.CreateDirectory(Path.Combine(_presetRoot, presetName));
            CreateEmptyFile(Path.Combine(dirInfo.FullName, fileName1));
            CreateEmptyFile(Path.Combine(dirInfo.FullName, fileName2));
            var preset = _presetManager.GetPresets(_presetRoot).Single();

            // Assert
            Assert.Throws<ArgumentException>(() =>
            {
                _presetManager.RenamePreset(preset, "Preset 1");
            });
        }

        [Test]
        public void ShouldThrowExceptionWhenDeletingIfDirectoryDoesNotExist()
        {
            // Arrange
            string presetName = "Preset 1";
            string fileName1 = "file1.ini";
            string fileName2 = "file1.txt";
            var dirInfo = Directory.CreateDirectory(Path.Combine(_presetRoot, presetName));
            CreateEmptyFile(Path.Combine(dirInfo.FullName, fileName1));
            CreateEmptyFile(Path.Combine(dirInfo.FullName, fileName2));
            var preset = _presetManager.GetPresets(_presetRoot).Single();

            // Act
            Directory.Delete(preset.FullPath, true);

            // Assert
            Assert.Throws<DirectoryNotFoundException>(() =>
            {
                _presetManager.DeletePreset(preset);
            });
        }

        private void CreateEmptyFile(string filename)
        {
            File.Create(filename).Dispose();
        }
    }
}
