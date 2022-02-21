using System;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public class RegisterUserShaderAttribute : System.Attribute
{
    public string ShaderName { get; set; }
    public string XmlDescriptor { get; set; }

    public RegisterUserShaderAttribute(string shaderName, string xmlDescriptor)
    {
        this.ShaderName = shaderName;
        this.XmlDescriptor = xmlDescriptor;
    }
}