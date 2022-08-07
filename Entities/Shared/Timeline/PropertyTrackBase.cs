
using System;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PropertyTrackBaseData))]
	public class PropertyTrackBase : SchematicPinTrack, IEntityData<FrostySdk.Ebx.PropertyTrackBaseData>
	{
		public new FrostySdk.Ebx.PropertyTrackBaseData Data => data as FrostySdk.Ebx.PropertyTrackBaseData;
		public override string DisplayName => "PropertyTrackBase";
		public virtual object CurrentValue => null;
		public string PropertyName => propertyName;

		protected string propertyName;

		public PropertyTrackBase(FrostySdk.Ebx.PropertyTrackBaseData inData, Entity inParent)
			: base(inData, inParent)
		{
			propertyName = FrostySdk.Utils.GetString(Data.TargetPinNameHash);
		}
	}
}

