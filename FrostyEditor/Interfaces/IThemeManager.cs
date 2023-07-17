using Avalonia;

namespace FrostyEditor.Interfaces;

public interface IThemeManager
{
    void Initialize(Application application);

    void Switch(int index);
}
