namespace ENBManager.Infrastructure.BusinessEntities
{
    public class Keywords
    {
        #region Public Properties

        public string[] Files { get; } = {
            "enb",
            "ReShade",
            "dxgi",

            "AmbientLight.fx",
            "Bloom.fx",
            "Curves.fx",
            "d3d9_smaa.dll",
            "d3d9.fx",
            "d3d9_sweetfx.dll",
            "d3d9injFX.dll",
            "DefaultPreset.ini",
            "DPX.fx",
            "effect.txt",
            "FXAA.fxh",
            "HDR.fx",
            "injector.ini",
            "LumaSharpen.fx",
            "LUT.fx",
            "SMAA.fx",
            "SMAA.fh",
            "SMAA.h",
            "technique.fxh",
            "Vibrance.fx"
        };

        public string[] Directories { get; } = {
            "Data\\Shaders",
            "enb",
            "ReShade",
            "SweetFX"
        };

        #endregion

        #region Static Members

        public static Keywords Instance => new Keywords();

        public static bool MatchesKeyword(string[] keywords, string name)
        {
            foreach (var keyword in keywords)
            {
                if (name.ToLower().Contains(keyword.ToLower()))
                    return true;
            }

            return false;
        } 

        #endregion
    }
}
