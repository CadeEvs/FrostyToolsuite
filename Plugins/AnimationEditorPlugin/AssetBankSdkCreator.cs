using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Controls.Primitives;
using AnimationEditorPlugin.Formats;
using FrostySdk;
using FrostySdk.IO;
using Microsoft.CSharp;

namespace AnimationEditorPlugin
{
    public class AssetBankModuleWriter : IDisposable
    {
        private List<Bank> m_classes;
        private string m_filename;

        public AssetBankModuleWriter(string inFilename, List<Bank> inClasses)
        {
            m_filename = inFilename;
            m_classes = inClasses;
        }

        public void Write(uint version)
        {
            // create sdk
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using FrostySdk.Attributes;");
            sb.AppendLine("using FrostySdk.Managers;");
            sb.AppendLine("using System.Reflection;");
            sb.AppendLine("using FrostySdk;");
            sb.AppendLine("using FrostySdk.Ebx;");
            sb.AppendLine();
            sb.AppendLine("[assembly: SdkVersion(" + (int)version + ")]");
            sb.AppendLine();
            sb.AppendLine("namespace AnimationEditorPlugin.Data");
            sb.AppendLine("{");
            {
                foreach (Bank classObj in m_classes)
                {
                    sb.AppendLine(WriteClass(classObj));
                }
            }
            sb.AppendLine("}");
            
            // temp testing
            //File.WriteAllText(@"C:\Frostbite\FrostyToolsSuite\Testing\Test.txt", sb.ToString());
            
            // write and compile sdk
            using (NativeWriter writer = new NativeWriter(new FileStream("temp.cs", FileMode.Create)))
            {
                writer.WriteLine(sb.ToString());
            }

            CSharpCodeProvider provider = new CSharpCodeProvider();

            CompilerParameters compilerParams = new CompilerParameters
            {
                GenerateExecutable = false,
                OutputAssembly = m_filename,
                CompilerOptions = "-define:DV_" + (int)ProfilesLibrary.DataVersion
            };

            FileInfo fi = new FileInfo(Assembly.GetExecutingAssembly().FullName);
            
            compilerParams.ReferencedAssemblies.Add("mscorlib.dll");
            compilerParams.ReferencedAssemblies.Add("FrostySdk.dll");
            compilerParams.ReferencedAssemblies.Add(fi.DirectoryName + "/Profiles/" + ProfilesLibrary.SDKFilename + ".dll");

            CompilerResults results = provider.CompileAssemblyFromFile(compilerParams, "temp.cs");
            File.Delete("temp.cs");
            
            if (results.Errors.Count > 0)
            {
                using (NativeWriter writer = new NativeWriter(new FileStream("ErrorsAssetBanks.txt", FileMode.Create)))
                {
                    foreach (CompilerError error in results.Errors)
                    {
                        writer.WriteLine("[Line: " + error.Line + "]: " + error.ErrorText);
                    }
                }
            }
        }

        private string WriteClass(Bank classObj)
        {
            StringBuilder sb = new StringBuilder();
            {
                if (string.IsNullOrEmpty(classObj.Name))
                {
                    return "";
                }
                
                string className = ReplaceBadCharacters(classObj.Name);
                
                sb.AppendLine("public class " + className);
                sb.AppendLine("{");

                foreach (Bank.Entry entryObj in classObj.Entries)
                {
                    sb.AppendLine(WriteField(entryObj));
                }
                
                sb.AppendLine("}");
            }
            return sb.ToString();
        }

        private string WriteField(Bank.Entry fieldObj)
        {
            StringBuilder sb = new StringBuilder();
            {
                if (string.IsNullOrEmpty(fieldObj.Name))
                {
                    return "";
                }
                
                string fieldName = ReplaceBadCharacters(fieldObj.Name);
                string fieldType = "";
                
                BankType objType = (BankType)fieldObj.Type;
                fieldType = fieldObj.IsArray
                    ? $"List<{GetFieldType(objType)}>"
                    : GetFieldType(objType);
                
                sb.AppendLine("public " + fieldType + " " + fieldName + " { get { return _" + fieldName + "; } set { _" + fieldName + " = value; } }");

                bool requiresDeclaration = fieldObj.IsArray
                                           || objType == BankType.Vector2
                                           || objType == BankType.Vector3
                                           || objType == BankType.Vector4
                                           || objType == BankType.String;

                sb.AppendLine("protected " + fieldType + " _" + fieldName + ((requiresDeclaration) ? " = new " + fieldType + "()" : "") + ";");
            }
            return sb.ToString();
        }

        private string GetFieldType(BankType type)
        {
            switch (type)
            {
                case BankType.Invalid: return "float";
                case BankType.Boolean: return "bool";
                case BankType.Int8: return "sbyte";
                case BankType.UInt8: return "byte";
                case BankType.Int16: return "short";
                case BankType.Uint16: return "ushort";
                case BankType.Int32: return "int";
                case BankType.UInt32: return "uint";
                case BankType.Int64: return "long";
                case BankType.UInt64: return "ulong";
                case BankType.Float: return "float";
                case BankType.Vector2: return "Vec2";
                case BankType.Vector3: return "Vec3";
                case BankType.Vector4: return "Vec4";
                case BankType.Quaternion: break;
                case BankType.Matrix: break;
                case BankType.Guid: return "Guid";
                case BankType.String: return "string";
                case BankType.Reference: break;
                case BankType.Double: return "double";
                default: return "string";
            }

            return "string";
        }

        private string ReplaceBadCharacters(string name)
        {
            return name.Replace(":", "_").Replace(" ", "_");
        }
        
        public void Dispose()
        {
        }
    }
    
    public class AssetBankSdkCreator
    {
        
    }
}