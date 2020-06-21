namespace ENBManager.Infrastructure.BusinessEntities
{
    public class Keywords
    {
        #region Public Properties

        public string[] Files { get; } = {
            "enb",
            "ReShade",
            "dxgi",

            "_weatherlist.ini",
            "AmbientLight.fx",
            "Bloom.fx",
            "common.fxh",
            "Curves.fx",
            "d3d9.fx",
            "d3d9_fx.dll",
            "d3d9_fxaa.dll",
            "d3d9_SFX.dll",
            "d3d9_SFX_FXAA.dll",
            "d3d9_SFX_SMAA.dll",
            "d3d9_Sharpen.dll",
            "d3d9_smaa.dll",
            "d3d9_sweetfx.dll",
            "d3d9injFX.dll",
            "DefaultPreset.ini",
            "DPX.fx",
            "effect.txt",
            "FXAA.fxh",
            "HDR.fx",
            "injector.ini",
            "injFX_Settings.h",
            "LumaSharpen.fx",
            "LUT.fx",
            "shader.fx",
            "SMAA.fx",
            "SMAA.fh",
            "SMAA.h",
            "SweetFX_preset.txt",
            "SweetFX_settings.txt",
            "technique.fxh",
            "Vibrance.fx"
        };

        public string[] Directories { get; } = {
            "Data\\Shaders",
            "enb",
            "injFX_Shaders",
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
