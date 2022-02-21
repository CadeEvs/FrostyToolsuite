using System.Windows.Controls;
using System.Windows.Media;

namespace Frosty.Core.Controls
{
    public class FrostyBaseEditor : Control
    {
        public virtual ImageSource Icon => null;

        public virtual void Closed()
        {
        }
    }
}
