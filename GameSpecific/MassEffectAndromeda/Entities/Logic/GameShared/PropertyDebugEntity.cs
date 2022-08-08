using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PropertyDebugEntityData))]
	public class PropertyDebugEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PropertyDebugEntityData>
	{
		public new FrostySdk.Ebx.PropertyDebugEntityData Data => data as FrostySdk.Ebx.PropertyDebugEntityData;
		public override string DisplayName => "PropertyDebug";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
        public override IEnumerable<ConnectionDesc> Events
        {
            get => new List<ConnectionDesc>()
            {
                new ConnectionDesc("Show", Direction.In),
                new ConnectionDesc("Hide", Direction.In)
            };
        }
        public override IEnumerable<ConnectionDesc> Properties
        {
            get => new List<ConnectionDesc>()
            {
                new ConnectionDesc("TextColor", Direction.In),
                new ConnectionDesc("BoolValue", Direction.In),
                new ConnectionDesc("FloatValue", Direction.In),
                new ConnectionDesc("IntValue", Direction.In),
                new ConnectionDesc("UintValue", Direction.In),
                new ConnectionDesc("TransformValue", Direction.In),
                new ConnectionDesc("Vec2Value", Direction.In),
                new ConnectionDesc("Vec3Value", Direction.In),
                new ConnectionDesc("Vec4Value", Direction.In),
                new ConnectionDesc("StringValue", Direction.In)
            };
        }

        public PropertyDebugEntity(FrostySdk.Ebx.PropertyDebugEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

