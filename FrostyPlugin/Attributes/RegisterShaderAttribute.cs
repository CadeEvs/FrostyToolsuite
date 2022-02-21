using System;

namespace Frosty.Core.Attributes
{
    /// <summary>
    /// Specifies a shaders type.
    /// </summary>
    public enum ShaderType
    {
        /// <summary>
        /// Vertex shader.
        /// </summary>
        VertexShader,
        /// <summary>
        /// Pixel shader.
        /// </summary>
        PixelShader,
        /// <summary>
        /// Compute shader.
        /// </summary>
        ComputeShader
    }

    /// <summary>
    /// This attribute registers a custom vertex, pixel or compute shader to the plugin system that is retrievable via the <see cref="Frosty.Core.Viewport.FrostyShaderDb"/> class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class RegisterShaderAttribute : Attribute
    {
        /// <summary>
        /// Gets the type of shader to register.
        /// </summary>
        /// <returns>The type of shader to register.</returns>
        public ShaderType ShaderType { get; private set; }

        /// <summary>
        /// Gets the name of the embedded resource that represents the shader data in Namespace.ResourceName format.
        /// </summary>
        /// <returns>The name of the embedded resource.</returns>
        public string ResourceName { get; set; }

        /// <summary>
        /// Gets the key that can be used to obtain the shader from the <see cref="Frosty.Core.Viewport.FrostyShaderDb"/> class.
        /// </summary>
        /// <returns>The key name.</returns>
        public string KeyName { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterShaderAttribute"/> class using the shader type, key and resource name.
        /// </summary>
        /// <param name="type">The type of shader to register.</param>
        /// <param name="key">The key used to obtain the shader from the <see cref="Frosty.Core.Viewport.FrostyShaderDb"/> class.</param>
        /// <param name="name">The name of the embedded resource in Namespace.ResourceName format</param>
        public RegisterShaderAttribute(ShaderType type, string key, string name)
        {
            ShaderType = type;
            KeyName = key;
            ResourceName = name;
        }
    }
}
