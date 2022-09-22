using System;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using FrostySdk;

namespace AnimationEditorPlugin
{
    public class AssetBankTypeLibrary
    {
        private const string ModuleName = "AssetBankClasses";
        private const string Namespace = "AnimationEditorPlugin.Data.";

        private static AssemblyBuilder m_assemblyBuilder;
        private static ModuleBuilder m_moduleBuilder;
        private static Assembly m_existingAssembly;

        public static void Initialize(bool loadSdk = true)
        {
            if (loadSdk)
            {
                // move across any newly created SDKs
                if (File.Exists("AssetBankTmpProfiles/" + ProfilesLibrary.SDKFilename + ".dll"))
                {
                    FileInfo srcFi = new FileInfo("AssetBankTmpProfiles/" + ProfilesLibrary.SDKFilename + ".dll");
                    FileInfo dstFi = new FileInfo("AssetBankProfiles/" + ProfilesLibrary.SDKFilename + ".dll");

                    File.Delete(dstFi.FullName);
                    File.Move(srcFi.FullName, dstFi.FullName);

                    File.Delete(srcFi.FullName);
                }

                // now try to load SDK
                if (File.Exists("AssetBankProfiles/" + ProfilesLibrary.SDKFilename + ".dll"))
                {
                    m_existingAssembly = Assembly.Load(new AssemblyName(ModuleName));
                }
            }

            AssemblyName name = new AssemblyName(ModuleName);
            m_assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
            m_moduleBuilder = m_assemblyBuilder.DefineDynamicModule(ModuleName);
        }
        
        public static Type GetType(string name)
        {
            if (m_existingAssembly != null)
            {
                Type type = m_existingAssembly.GetType(Namespace + name);
                if (type != null)
                {
                    return type;
                }
            }

            return m_moduleBuilder.Assembly.GetType(Namespace + name);
        }
    }
}