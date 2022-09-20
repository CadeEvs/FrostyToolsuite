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
        private const string Namespace = "FrostySdk.Ebx.";

        private static AssemblyBuilder m_assemblyBuilder;
        private static ModuleBuilder m_moduleBuilder;
        private static Assembly m_existingAssembly;

        public static void Initialize(bool loadSdk = true)
        {
            if (loadSdk)
            {
                // move across any newly created SDKs
                if (File.Exists("AssetBank/TmpProfiles/" + ProfilesLibrary.SDKFilename + ".dll"))
                {
                    FileInfo srcFi = new FileInfo("AssetBank/TmpProfiles/" + ProfilesLibrary.SDKFilename + ".dll");
                    FileInfo dstFi = new FileInfo("AssetBank/Profiles/" + ProfilesLibrary.SDKFilename + ".dll");

                    File.Delete(dstFi.FullName);
                    File.Move(srcFi.FullName, dstFi.FullName);

                    File.Delete(srcFi.FullName);
                }

                // now try to load SDK
                if (File.Exists("AssetBank/Profiles/" + ProfilesLibrary.SDKFilename + ".dll"))
                {
                    m_existingAssembly = Assembly.Load(new AssemblyName(ModuleName));
                }
            }

            AssemblyName name = new AssemblyName(ModuleName);
            m_assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
            m_moduleBuilder = m_assemblyBuilder.DefineDynamicModule(ModuleName);
        }
    }
}