using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.PropertyChangeListenerEntityData))]
	public class PropertyChangeListenerEntity : LogicEntity, IEntityData<FrostySdk.Ebx.PropertyChangeListenerEntityData>
	{
		public new FrostySdk.Ebx.PropertyChangeListenerEntityData Data => data as FrostySdk.Ebx.PropertyChangeListenerEntityData;
		public override string DisplayName => "PropertyChangeListener";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;

		public PropertyChangeListenerEntity(FrostySdk.Ebx.PropertyChangeListenerEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

