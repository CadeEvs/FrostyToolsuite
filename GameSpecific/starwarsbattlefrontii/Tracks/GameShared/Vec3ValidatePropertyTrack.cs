
namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.Vec3ValidatePropertyTrackData))]
	public class Vec3ValidatePropertyTrack : ValidatePropertyTrackBase, IEntityData<FrostySdk.Ebx.Vec3ValidatePropertyTrackData>
	{
		public new FrostySdk.Ebx.Vec3ValidatePropertyTrackData Data => data as FrostySdk.Ebx.Vec3ValidatePropertyTrackData;
		public override string DisplayName => "Vec3ValidatePropertyTrack";

		public Vec3ValidatePropertyTrack(FrostySdk.Ebx.Vec3ValidatePropertyTrackData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

