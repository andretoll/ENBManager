﻿using ENBManager.Core.BusinessEntities.Base;
using ENBManager.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace ENBManager.Core.Services
{
    public class GameRegistry : IGameRegistry
    {
        private readonly List<GameBase> _supportedGames;

        public GameRegistry()
        {
            _supportedGames = new List<GameBase>();

            _supportedGames.Add(new SkyrimSE());
            _supportedGames.Add(new Skyrim());
            _supportedGames.Add(new Fallout4());
        }

        public IEnumerable<GameBase> GetSupportedGames()
        {
            return _supportedGames;
        }
    }

    internal class SkyrimSE : GameBase
    {
        public override string Title => "The Elder Scrolls V: Skyrim Special Edition";
        public override string Executable => "SkyrimSE.exe";
        public override string InstalledLocation { get; set; }
        public override BitmapImage Icon => new BitmapImage(new Uri("pack://application:,,,/ENBManager.Core;component/Resources/Icons/Games/skyrimse.png"));
    }

    internal class Skyrim : GameBase
    {
        public override string Title => "The Elder Scrolls V: Skyrim";
        public override string Executable => "Skyrim.exe";
        public override string InstalledLocation { get; set; }
        public override BitmapImage Icon => new BitmapImage(new Uri("pack://application:,,,/ENBManager.Core;component/Resources/Icons/Games/skyrim.png"));
    }

    internal class Fallout4 : GameBase
    {
        public override string Title => "Fallout 4";
        public override string Executable => "Fallout4.exe";
        public override string InstalledLocation { get; set; }
        public override BitmapImage Icon => new BitmapImage(new Uri("pack://application:,,,/ENBManager.Core;component/Resources/Icons/Games/fallout4.png"));
    }
}