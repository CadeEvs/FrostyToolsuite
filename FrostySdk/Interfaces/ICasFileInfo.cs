namespace Frosty.Sdk.Interfaces;

public interface ICasFileInfo
{
    bool GetIsPatch();
    
    int GetInstallChunkIndex();
    
    int GetCasIndex();
}