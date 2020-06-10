using ENBManager.Infrastructure.Constants;
using ENBManager.Modules.Shared.Interfaces;
using ENBManager.Modules.Shared.Services;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ENBManager.Modules.Tests.Shared.Services
{
    [TestFixture]
    public class PresetManagerTests
    {
        private IPresetManager _presetManager;
        private string _presetRoot;
        private string _gameDir;

        [SetUp]
        public void Setup()
        {
            _presetManager = new PresetManager();

            _presetRoot = Path.Combine(Paths.GetBaseDirectory(), "presetmanager_tests", "preset_dir");
            Directory.CreateDirectory(_presetRoot);
            _gameDir = Path.Combine(Paths.GetBaseDirectory(), "presetmanager_tests", "game_dir");
            Directory.CreateDirectory(_gameDir);
        }

        [TearDown]
        public void Teardown()
        {
            Directory.Delete(Path.Combine(Paths.GetBaseDirectory(), "presetmanager_tests"), true);
        }

        [Test]
        public void ShouldReturnAllPresets()
        {
            // Arrange
            CreatePreset("Preset 1");
            CreatePreset("Preset 2");

            // Act
            var presets = _presetManager.GetPresets(_presetRoot);

            // Assert
            Assert.That(presets.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task ShouldReturnSinglePreset()
        {
            // Arrange
            string presetName = "Preset 1";
            string file1 = "file1.ini";
            string file2 = "file1.txt";
            var dirInfo = CreatePreset(presetName, 
                file1,
                file2);
            CreatePreset("Preset 2",
                "file2.ini",
                "file2.txt");

            // Act
            var preset = await _presetManager.GetPresetAsync(_presetRoot, presetName);

            // Assert
            Assert.That(preset.Name, Is.EqualTo(presetName));
            Assert.That(preset.Files.Count(), Is.EqualTo(2));
            CollectionAssert.Contains(preset.Files, Path.Combine(dirInfo.FullName, file1));
            CollectionAssert.Contains(preset.Files, Path.Combine(dirInfo.FullName, file2));
        }

        [Test]
        public async Task ShouldHaveNameAndPathBasedOnDirectory()
        {
            // Arrange
            string presetName = "Preset 1";
            var dirInfo = CreatePreset(presetName);

            // Act
            var preset = await _presetManager.GetPresetAsync(_presetRoot, presetName);

            // Assert
            Assert.That(preset.Name, Is.EqualTo(presetName));
            Assert.That(preset.FullPath, Is.EqualTo(dirInfo.FullName));
        }

        [Test]
        public async Task ShouldIncludeAllFiles()
        {
            // Arrange
            // Arrange
            string presetName = "Preset 1";
            string file1 = "file1.ini";
            string file2 = "file1.txt";
            var dirInfo = CreatePreset(presetName,
                file1,
                file2);

            // Act
            var preset = await _presetManager.GetPresetAsync(_presetRoot, presetName);
            var files = preset.Files;

            // Assert
            Assert.That(files.Count(), Is.EqualTo(2));
            Assert.That(files.All(x => string.IsNullOrEmpty(x)), Is.False);
        }

        [Test]
        public async Task ShouldRenamePreset()
        {
            // Arrange
            string presetName = "Preset 1";
            string newPresetName = "Preset 2";
            var dirInfo = CreatePreset(presetName);
            var preset = await _presetManager.GetPresetAsync(_presetRoot, presetName);

            // Act
            _presetManager.RenamePreset(preset, newPresetName);

            // Assert
            Assert.That(Directory.Exists(Path.Combine(preset.FullPath, presetName)), Is.False);
            Assert.That(Directory.Exists(Path.Combine(dirInfo.Parent.FullName, newPresetName)), Is.True);
        }

        [TestCase(null)]
        [TestCase("")]
        public async Task ShouldThrowExceptionWhenRenamingIfNewNameHasNullOrEmptyValue(string newPresetName)
        {
            // Arrange
            string presetName = "Preset 1";
            var dirInfo = CreatePreset(presetName);
            var preset = await _presetManager.GetPresetAsync(_presetRoot, presetName);

            // Assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                _presetManager.RenamePreset(preset, newPresetName);
            });
        }

        [Test]
        public async Task ShouldThrowExceptionWhenRenamingIfDirectoryDoesNotExist()
        {
            // Arrange
            string presetName = "Preset 1";
            var dirInfo = CreatePreset(presetName);
            var preset = await _presetManager.GetPresetAsync(_presetRoot, presetName);

            // Act
            Directory.Delete(preset.FullPath, true);

            // Assert
            Assert.Throws<DirectoryNotFoundException>(() =>
            {
                _presetManager.RenamePreset(preset, "Preset 2");
            });
        }

        [Test]
        public async Task ShouldThrowExceptionWhenRenamingIfNewNameEqualsOldName()
        {
            // Arrange
            string presetName = "Preset 1";
            var dirInfo = CreatePreset(presetName);
            var preset = await _presetManager.GetPresetAsync(_presetRoot, presetName);

            // Assert
            Assert.Throws<ArgumentException>(() =>
            {
                _presetManager.RenamePreset(preset, presetName);
            });
        }

        [Test]
        public async Task ShouldThrowExceptionWhenDeletingIfDirectoryDoesNotExist()
        {
            // Arrange
            string presetName = "Preset 1";
            var dirInfo = CreatePreset(presetName);
            var preset = await _presetManager.GetPresetAsync(_presetRoot, presetName);

            // Act
            Directory.Delete(preset.FullPath, true);

            // Assert
            Assert.Throws<DirectoryNotFoundException>(() =>
            {
                _presetManager.DeletePreset(preset);
            });
        }

        [Test]
        public async Task ShouldActivatePreset()
        {
            // Arrange
            string presetName = "Preset 1";
            string file1 = "file.txt";
            string file2 = "file.ini";
            CreatePreset(presetName,
                file1,
                file2);
            var preset = await _presetManager.GetPresetAsync(_presetRoot, presetName);

            // Act
            await _presetManager.ActivatePresetAsync(_gameDir, preset);

            // Assert
            Assert.That(File.Exists(Path.Combine(_gameDir, file1)), Is.True);
            Assert.That(File.Exists(Path.Combine(_gameDir, file2)), Is.True);
        }

        [Test]
        public async Task ShouldDeactivatePreset()
        {
            // Arrange
            string presetName = "Preset 1";
            string file1 = "file.txt";
            string file2 = "file.ini";
            CreatePreset(presetName,
                file1,
                file2);
            var preset = await _presetManager.GetPresetAsync(_presetRoot, presetName);
            await _presetManager.ActivatePresetAsync(_gameDir, preset);

            // Act
            await _presetManager.DeactivatePresetAsync(_gameDir, preset);

            // Assert
            Assert.That(File.Exists(Path.Combine(_gameDir, file1)), Is.False);
            Assert.That(File.Exists(Path.Combine(_gameDir, file2)), Is.False);
        }

        [Test]
        public async Task ShouldCreateExistingPreset()
        {
            // Arrange
            string presetName = "Preset 1";
            string validFile1 = "enbseries.txt";
            string validFile2 = "enblocal.ini";
            string validFile3 = @"enbcache\file.ini";
            string invalidFile1 = @"folder\file.ini";
            string invalidFile2 = "file.txt";
            CreatePreset(presetName,
                validFile1,
                validFile2,
                validFile3,
                invalidFile1,
                invalidFile2);
            var preset = await _presetManager.GetPresetAsync(_presetRoot, presetName);
            await _presetManager.ActivatePresetAsync(_gameDir, preset);

            // Act
            var newPreset = _presetManager.CreateExistingPreset(_gameDir);

            // Assert
            Assert.That(newPreset.Files.Count(), Is.EqualTo(3));
            CollectionAssert.DoesNotContain(newPreset.Files, invalidFile1);
            CollectionAssert.DoesNotContain(newPreset.Files, invalidFile2);
        }

        [Test]
        public async Task ShouldSaveCurrentPreset()
        {
            // Arrange
            string presetName = "Preset 1";
            string file1 = "enbseries.txt";
            string file2 = "enblocal.ini";
            string file3 = @"enbcache\file.ini";
            CreatePreset(presetName,
                file1,
                file2,
                file3);
            var preset = await _presetManager.GetPresetAsync(_presetRoot, presetName);
            await _presetManager.ActivatePresetAsync(_gameDir, preset);
            var newPreset = _presetManager.CreateExistingPreset(_gameDir);
            newPreset.Name = "Preset 2";

            // Act
            await _presetManager.SaveCurrentPresetAsync(_presetRoot, _gameDir, newPreset);
            var addedPreset = await _presetManager.GetPresetAsync(_presetRoot, newPreset.Name);

            // Assert
            Assert.That(addedPreset.Name, Is.EqualTo("Preset 2"));
            Assert.That(addedPreset.Files.Count(), Is.EqualTo(3));
            CollectionAssert.Contains(addedPreset.Files, Path.Combine(addedPreset.FullPath, file1));
            CollectionAssert.Contains(addedPreset.Files, Path.Combine(addedPreset.FullPath, file2));
            CollectionAssert.Contains(addedPreset.Files, Path.Combine(addedPreset.FullPath, file3));
        }

        [Test]
        public async Task ShouldSaveNewPreset()
        {
            // Arrange
            string presetName = "Preset 1";
            string anotherPresetName = "Preset 2";
            string file1 = "enbseries.txt";
            string file2 = "enblocal.ini";
            string file3 = @"enbcache\file.ini";
            CreatePreset(presetName,
                file1,
                file2,
                file3);
            var preset = await _presetManager.GetPresetAsync(_presetRoot, presetName);
            preset.Name = anotherPresetName;

            // Act
            await _presetManager.SaveNewPresetAsync(_presetRoot, preset);
            var newPreset = await _presetManager.GetPresetAsync(_presetRoot, anotherPresetName);

            // Assert
            Assert.That(newPreset.Name, Is.EqualTo(anotherPresetName));
            Assert.That(newPreset.Files.Count(), Is.GreaterThan(0));
        }

        [Test]
        public async Task ShouldValidatePresetWhenIdentical()
        {
            // Arrange
            string presetName = "Preset 1";
            string file1 = "enbseries.txt";
            string file2 = "enblocal.ini";
            string file3 = @"enbcache\file.ini";
            CreatePreset(presetName,
                file1,
                file2,
                file3);
            var preset = await _presetManager.GetPresetAsync(_presetRoot, presetName);
            await _presetManager.ActivatePresetAsync(_gameDir, preset);

            // Act
            AddFileContent(Path.Combine(_gameDir, file1), "changed");
            AddFileContent(Path.Combine(preset.FullPath, file1), "changed");
            bool expectingTrue = await _presetManager.ValidatePresetAsync(Path.Combine(_gameDir), preset);

            // Assert
            Assert.That(expectingTrue, Is.True);
        }

        [Test]
        public async Task ShouldInvalidatePresetWhenDifferent()
        {
            // Arrange
            string presetName = "Preset 1";
            string file1 = "enbseries.txt";
            string file2 = "enblocal.ini";
            string file3 = @"enbcache\file.ini";
            CreatePreset(presetName,
                file1,
                file2,
                file3);
            var preset = await _presetManager.GetPresetAsync(_presetRoot, presetName);
            await _presetManager.ActivatePresetAsync(_gameDir, preset);

            // Act
            AddFileContent(Path.Combine(_gameDir, file1), "changed1");
            AddFileContent(Path.Combine(preset.FullPath, file1), "changed2");
            bool expectingFalse = await _presetManager.ValidatePresetAsync(Path.Combine(_gameDir), preset);

            // Assert
            Assert.That(expectingFalse, Is.False);
        }

        [Test]
        public async Task ShouldUpdatePresetFiles()
        {
            // Arrange
            string presetName = "Preset 1";
            string file1 = "enbseries.txt";
            string file2 = "enblocal.ini";
            string file3 = @"enbcache\file.ini";
            CreatePreset(presetName,
                file1,
                file2,
                file3);
            var preset = await _presetManager.GetPresetAsync(_presetRoot, presetName);
            await _presetManager.ActivatePresetAsync(_gameDir, preset);

            // Act
            AddFileContent(Path.Combine(_gameDir, file1), "changed1");
            AddFileContent(Path.Combine(preset.FullPath, file1), "changed2");
            bool expectingFalse = await _presetManager.ValidatePresetAsync(Path.Combine(_gameDir), preset);
            await _presetManager.UpdatePresetFilesAsync(Path.Combine(_gameDir), preset);
            bool expectingTrue = await _presetManager.ValidatePresetAsync(Path.Combine(_gameDir), preset);

            // Assert
            Assert.That(expectingFalse, Is.False);
            Assert.That(expectingTrue, Is.True);
        }

        private DirectoryInfo CreatePreset(string presetName, params string[] files)
        {
            var dirInfo = Directory.CreateDirectory(Path.Combine(_presetRoot, presetName));

            foreach (var file in files)
            {
                if (!Directory.Exists(Path.Combine(dirInfo.FullName, Path.GetDirectoryName(file))))
                    Directory.CreateDirectory(Path.Combine(dirInfo.FullName, Path.GetDirectoryName(file)));

                File.Create(Path.Combine(dirInfo.FullName, file)).Dispose();
            }

            return dirInfo;
        }

        private void AddFileContent(string path, string content)
        {
            using (StreamWriter stream = new StreamWriter(path))
            {
                stream.Write(content);
            }
        }
    }
}
