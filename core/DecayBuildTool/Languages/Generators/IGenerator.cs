namespace DecayBuildTool.Languages.Generators
{
    public interface IGenerator
    {
        string GenerateHeader(HeaderFileData fileData);
    }
}