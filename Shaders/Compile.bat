fxc TextureEditor.hlsl /Tvs_5_0 /EVS_Main /Fo.\Bin\Texture.vso
fxc TextureEditor.hlsl /Tps_5_0 /EPS_Main /Fo.\Bin\Texture.pso
fxc SoundWaveEditor.hlsl /Tvs_5_0 /EVS_Main /Fo.\Bin\SoundWave.vso
fxc SoundWaveEditor.hlsl /Tps_5_0 /EPS_Main /Fo.\Bin\SoundWave.pso
fxc IesResourceEditor.hlsl /Tps_5_0 /EPS_Main /Fo.\Bin\IesResource.pso
fxc HeightfieldDecalEditor.hlsl /Tvs_5_0 /EVS_Main /Fo.\Bin\Heightfield.vso
fxc HeightfieldDecalEditor.hlsl /Tps_5_0 /EPS_Main /Fo.\Bin\Heightfield.pso
fxc MovieTextureEditor.hlsl /Tvs_5_0 /EVS_Main /Fo.\Bin\MovieTexture.vso
fxc MovieTextureEditor.hlsl /Tps_5_0 /EPS_Main /Fo.\Bin\MovieTexture.pso

fxc DeferredAmbientPixelShader.hlsl /Tps_5_0 /Emain /Fo.\Bin\DeferredAmbientPixelShader.pso
fxc DeferredPointPixelShader.hlsl /Tps_5_0 /Emain /Fo.\Bin\DeferredPointPixelShader.pso

fxc SkyboxVertexShader.hlsl /Tvs_5_0 /Emain /Fo.\Bin\SkyboxVertexShader.vso
fxc SkyboxPixelShader.hlsl /Tps_5_0 /Emain /Fo.\Bin\SkyboxPixelShader.pso
fxc SkyboxGradientPixelShader.hlsl /Tps_5_0 /Emain /Fo.\Bin\SkyboxGradientPixelShader.pso
fxc LineVertexShader.hlsl /Tvs_5_0 /Emain /Fo.\Bin\LineVertexShader.vso
fxc LinePixelShader.hlsl /Tps_5_0 /Emain /Fo.\Bin\LinePixelShader.pso

fxc Grid.hlsl /Tvs_5_0 /EVS_Main /Fo.\Bin\Grid.vso
fxc Grid.hlsl /Tps_5_0 /EPS_Main /Fo.\Bin\Grid.pso
fxc TXAAResolveDepth.hlsl /Tps_5_0 /EPS_Main /Fo.\Bin\TXAAResolveDepth.pso
rem fxc EditorComposite.hlsl /Tps_5_0 /EPS_Main /Fo.\Bin\EditorComposite.pso
rem fxc TXAAResolveDepth.hlsl /Tps_5_0 /EPS_Resolve /Fo.\Bin\Resolve.pso

fxc HDR.hlsl /Tps_5_0 /EPS_DownScale4x4 /Fo.\Bin\DownScale4x4.pso
fxc HDR.hlsl /Tps_5_0 /EPS_SampleLumInitial /Fo.\Bin\SampleLumInitial.pso
fxc HDR.hlsl /Tps_5_0 /EPS_SampleLumIterative /Fo.\Bin\SampleLumIterative.pso
fxc HDR.hlsl /Tps_5_0 /EPS_SampleLumFinal /Fo.\Bin\SampleLumFinal.pso
fxc HDR.hlsl /Tps_5_0 /EPS_CalculateAdaptedLum /Fo.\Bin\CalculateAdaptedLum.pso

fxc HDR.hlsl /Tps_5_0 /EPS_BrightPass /Fo.\Bin\BrightPass.pso
fxc HDR.hlsl /Tps_5_0 /EPS_GaussianBlur5x5 /Fo.\Bin\GaussianBlur5x5.pso
fxc HDR.hlsl /Tps_5_0 /EPS_DownSample2x2 /Fo.\Bin\DownSample2x2.pso
fxc HDR.hlsl /Tps_5_0 /EPS_BloomBlur /Fo.\Bin\BloomBlur.pso
fxc HDR.hlsl /Tps_5_0 /EPS_RenderBloom /Fo.\Bin\RenderBloom.pso

rem fxc HDR.hlsl /Tps_5_0 /EDownScale2x2 /Fo.\Bin\DownScale2x2.pso
rem fxc HDR.hlsl /Tps_5_0 /EFinalScenePass /Fo.\Bin\FinalScenePass.pso
rem fxc HDR.hlsl /Tps_5_0 /EGaussBlur5x5 /Fo.\Bin\GaussBlur5x5.pso
rem fxc HDR.hlsl /Tps_5_0 /EBloom /Fo.\Bin\Bloom.pso

fxc Lights.hlsl /Tps_5_0 /EPS_SunLight /Fo.\Bin\SunLight.pso
fxc Lights.hlsl /Tps_5_0 /EPS_Point /Fo.\Bin\PointLight.pso
fxc Lights.hlsl /Tps_5_0 /EPS_Sphere /Fo.\Bin\SphereLight.pso

fxc Utilities.hlsl /Tvs_5_0 /EVS_FullscreenQuad /Fo.\Bin\FullscreenQuad.vso
fxc Utilities.hlsl /Tps_5_0 /EPS_Resolve /Fo.\Bin\Resolve.pso
fxc Utilities.hlsl /Tps_5_0 /EPS_ResolveCubeMapFace /Fo.\Bin\ResolveCubeMapFace.pso
fxc Utilities.hlsl /Tps_5_0 /EPS_LookupTable /Fo.\Bin\LookupTable.pso
fxc Utilities.hlsl /Tps_5_0 /EPS_EditorComposite /Fo.\Bin\EditorComposite.pso
fxc Utilities.hlsl /Tps_5_0 /EPS_ResolveDepthToMsaa /Fo.\Bin\ResolveDepthToMsaa.pso
fxc Utilities.hlsl /Tps_5_0 /EPS_SelectionOutline /Fo.\Bin\SelectionOutline.pso
fxc Utilities.hlsl /Tps_5_0 /EPS_DebugRenderMode /Fo.\Bin\DebugRenderMode.pso
fxc Utilities.hlsl /Tps_5_0 /EPS_ResolveWorldNormals /Fo.\Bin\ResolveWorldNormals.pso

fxc IBL.hlsl /Tps_5_0 /EPS_DFG /Fo.\Bin\IBL_IntegrateDFG.pso
fxc IBL.hlsl /Tps_5_0 /EPS_DiffuseLD /Fo.\Bin\IBL_IntegrateDiffuseLD.pso
fxc IBL.hlsl /Tps_5_0 /EPS_SpecularLD /Fo.\Bin\IBL_IntegrateSpecularLD.pso
fxc IBL.hlsl /Tps_5_0 /EPS_IBL /Fo.\Bin\IBL_Main.pso

fxc MeshFallback.hlsl /Tvs_5_0 /EVS_MeshFallback /DPROFILE_20171117=1 /DOPTIMIZE=1 /Fo.\Bin\20171117.vso
fxc MeshFallback.hlsl /Tvs_5_0 /EVS_MeshFallback /DPROFILE_20170321=1 /DOPTIMIZE=1 /Fo.\Bin\20170321.vso
fxc MeshFallback.hlsl /Tvs_5_0 /EVS_MeshFallback /DPROFILE_20160607=1 /DOPTIMIZE=1 /Fo.\Bin\20160607.vso
fxc MeshFallback.hlsl /Tvs_5_0 /EVS_MeshFallback /DPROFILE_20151117=1 /DOPTIMIZE=1 /Fo.\Bin\20151117.vso
fxc MeshFallback.hlsl /Tvs_5_0 /EVS_MeshFallback /DPROFILE_20141118=1 /DOPTIMIZE=1 /Fo.\Bin\20141117.vso
fxc MeshFallback.hlsl /Tvs_5_0 /EVS_MeshFallback /DPROFILE_20141118=1 /DOPTIMIZE=1 /Fo.\Bin\20141118.vso
fxc MeshFallback.hlsl /Tvs_5_0 /EVS_MeshFallback /DPROFILE_20131115=1 /DOPTIMIZE=1 /Fo.\Bin\20131115.vso
fxc MeshFallback.hlsl /Tvs_5_0 /EVS_MeshFallback /DPROFILE_20160927=1 /DOPTIMIZE=1 /Fo.\Bin\20160927.vso
fxc MeshFallback.hlsl /Tvs_5_0 /EVS_MeshFallback /DPROFILE_20161021=1 /DOPTIMIZE=1 /Fo.\Bin\20161021.vso
fxc MeshFallback.hlsl /Tvs_5_0 /EVS_MeshFallback /DPROFILE_20170929=1 /DOPTIMIZE=1 /Fo.\Bin\20170929.vso
fxc MeshFallback.hlsl /Tvs_5_0 /EVS_MeshFallback /DPROFILE_20150223=1 /DOPTIMIZE=1 /Fo.\Bin\20150223.vso
fxc MeshFallback.hlsl /Tvs_5_0 /EVS_MeshFallback /DPROFILE_20170929=1 /DOPTIMIZE=1 /Fo.\Bin\20180914.vso
fxc MeshFallback.hlsl /Tvs_5_0 /EVS_MeshFallback /DPROFILE_20181207=1 /DOPTIMIZE=1 /Fo.\Bin\20181207.vso
fxc MeshFallback.hlsl /Tvs_5_0 /EVS_MeshFallback /DPROFILE_20180628=1 /DOPTIMIZE=1 /Fo.\Bin\20180628.vso
fxc MeshFallback.hlsl /Tvs_5_0 /EVS_MeshFallback /DPROFILE_20171210=1 /DOPTIMIZE=1 /Fo.\Bin\20171210.vso
fxc MeshFallback.hlsl /Tvs_5_0 /EVS_MeshFallback /DPROFILE_20140225=1 /DOPTIMIZE=1 /Fo.\Bin\20140225.vso
fxc MeshFallback.hlsl /Tvs_5_0 /EVS_MeshFallback /DPROFILE_20190905=1 /DOPTIMIZE=1 /Fo.\Bin\20190905.vso
fxc MeshFallback.hlsl /Tvs_5_0 /EVS_MeshFallback /DPROFILE_20170929=1 /DOPTIMIZE=1 /Fo.\Bin\20190911.vso
fxc MeshFallback.hlsl /Tvs_5_0 /EVS_MeshFallback /DPROFILE_20151103=1 /Fo.\Bin\20151103.vso
fxc MeshFallback.hlsl /Tvs_5_0 /EVS_MeshFallback /DPROFILE_20171110=1 /Fo.\Bin\20171110.vso
fxc MeshFallback.hlsl /Tvs_5_0 /EVS_MeshFallback /DPROFILE_20180807=1 /Fo.\Bin\20180807.vso
fxc MeshFallback.hlsl /Tvs_5_0 /EVS_MeshFallback /DPROFILE_20190729=1 /Fo.\Bin\20190729.vso
fxc MeshFallback.hlsl /Tvs_5_0 /EVS_MeshFallback /DPROFILE_20191101=1 /Fo.\Bin\20191101.vso

fxc MeshFallback.hlsl /Tps_5_0 /EPS_MeshFallback /DPROFILE_20171117=1 /Fo.\Bin\20171117.pso
fxc MeshFallback.hlsl /Tps_5_0 /EPS_MeshFallback /DPROFILE_20170321=1 /Fo.\Bin\20170321.pso
fxc MeshFallback.hlsl /Tps_5_0 /EPS_MeshFallback /DPROFILE_20160607=1 /Fo.\Bin\20160607.pso
fxc MeshFallback.hlsl /Tps_5_0 /EPS_MeshFallback /DPROFILE_20151117=1 /Fo.\Bin\20151117.pso
fxc MeshFallback.hlsl /Tps_5_0 /EPS_MeshFallback /DPROFILE_20141118=1 /Fo.\Bin\20141117.pso
fxc MeshFallback.hlsl /Tps_5_0 /EPS_MeshFallback /DPROFILE_20141118=1 /Fo.\Bin\20141118.pso
fxc MeshFallback.hlsl /Tps_5_0 /EPS_MeshFallback /DPROFILE_20160927=1 /Fo.\Bin\20160927.pso
fxc MeshFallback.hlsl /Tps_5_0 /EPS_MeshFallback /DPROFILE_20161021=1 /Fo.\Bin\20161021.pso
fxc MeshFallback.hlsl /Tps_5_0 /EPS_MeshFallback /DPROFILE_20170929=1 /Fo.\Bin\20170929.pso
fxc MeshFallback.hlsl /Tps_5_0 /EPS_MeshFallback /DPROFILE_20150223=1 /Fo.\Bin\20150223.pso
fxc MeshFallback.hlsl /Tps_5_0 /EPS_MeshFallback /DPROFILE_20170929=1 /Fo.\Bin\20180914.pso
fxc MeshFallback.hlsl /Tps_5_0 /EPS_MeshFallback /DPROFILE_20181207=1 /Fo.\Bin\20181207.pso
fxc MeshFallback.hlsl /Tps_5_0 /EPS_MeshFallback /DPROFILE_20180628=1 /Fo.\Bin\20180628.pso
fxc MeshFallback.hlsl /Tps_5_0 /EPS_MeshFallback /DPROFILE_20131115=1 /Fo.\Bin\20131115.pso
fxc MeshFallback.hlsl /Tps_5_0 /EPS_MeshFallback /DPROFILE_20131115=1 /Fo.\Bin\20171210.pso
fxc MeshFallback.hlsl /Tps_5_0 /EPS_MeshFallback /DPROFILE_20140225=1 /Fo.\Bin\20140225.pso
fxc MeshFallback.hlsl /Tps_5_0 /EPS_MeshFallback /DPROFILE_20190905=1 /Fo.\Bin\20190905.pso
fxc MeshFallback.hlsl /Tps_5_0 /EPS_MeshFallback /DPROFILE_20170929=1 /Fo.\Bin\20190911.pso

fxc MeshFallback.hlsl /Tps_5_0 /EPS_MeshFallback /DPROFILE_20151103=1 /Fo.\Bin\20151103.pso
fxc MeshFallback.hlsl /Tps_5_0 /EPS_MeshFallback /DPROFILE_20171110=1 /Fo.\Bin\20171110.pso
fxc MeshFallback.hlsl /Tps_5_0 /EPS_MeshFallback /DPROFILE_20180807=1 /Fo.\Bin\20180807.pso
fxc MeshFallback.hlsl /Tps_5_0 /EPS_MeshFallback /DPROFILE_20190729=1 /Fo.\Bin\20190729.pso
fxc MeshFallback.hlsl /Tps_5_0 /EPS_MeshFallback /DPROFILE_20191101=1 /Fo.\Bin\20191101.pso

fxc UnpackNormals.hlsl /Tcs_5_0 /ECS_UnpackAxisAngle /Fo.\Bin\UnpackAxisAngle.vso
fxc UnpackNormals.hlsl /Tcs_5_0 /ECS_UnpackQuaternion /Fo.\Bin\UnpackQuaternion.vso

cd ..\FrostyEditor\bin\Developer\Debug
FrostyCmd.exe "D:\OriginLibrary\originapps\Mass Effect Andromeda\MassEffectAndromeda.exe" shader
copy ..\..\..\Shaders.bin .\Shaders.bin