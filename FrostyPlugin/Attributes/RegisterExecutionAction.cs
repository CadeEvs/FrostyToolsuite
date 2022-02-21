using System;
using Frosty.Core.Mod;

namespace Frosty.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class RegisterExecutionAction : Attribute
    {
        /// <summary>
        /// Gets the type to use to define a execution action.
        /// </summary>
        /// <returns>The type to use to define a custom execution action</returns>
        public Type ExecutionActionType { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterExecutionAction"/> class.
        /// </summary>
        /// <param name="type">The type of the custom execution action. This type must derive from <see cref="ICustomEditorExecution"/></param>
        public RegisterExecutionAction(Type type)
        {
            ExecutionActionType = type;
        }
    }
}
