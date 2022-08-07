using FrostySdk;
using LevelEditorPlugin.Managers;
using System.Collections.Generic;
using System.IO;
using FrostySdk.Ebx;
using SchematicChannelAsset = LevelEditorPlugin.Assets.SchematicChannelAsset;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.SchematicChannelEntityData))]
	public class SchematicChannelEntity : LogicEntity, IEntityData<FrostySdk.Ebx.SchematicChannelEntityData>
	{
		public new FrostySdk.Ebx.SchematicChannelEntityData Data => data as FrostySdk.Ebx.SchematicChannelEntityData;
		public override string DisplayName => "SchematicChannel";
		public override FrostySdk.Ebx.Realm Realm => Data.Realm;
		public override IEnumerable<ConnectionDesc> Links
		{
			get
			{
				List<ConnectionDesc> outLinks = new List<ConnectionDesc>();
				foreach (LinkChannel link in schematicChannelAsset.Data.Links)
				{
					string name = Utils.GetString(link.Id);
					outLinks.Add(new ConnectionDesc() { Name = name, Direction = Direction.In });
					outLinks.Add(new ConnectionDesc() { Name = name, Direction = Direction.Out });
				}
				return outLinks;
			}
		}
		public override IEnumerable<ConnectionDesc> Properties
        {
			get
            {
				List<ConnectionDesc> outProperties = new List<ConnectionDesc>();
				foreach (PropertyChannel prop in schematicChannelAsset.Data.Properties)
				{
					string name = Utils.GetString(prop.Id);
					outProperties.Add(new ConnectionDesc() { Name = name, Direction = Direction.In });
					outProperties.Add(new ConnectionDesc() { Name = name, Direction = Direction.Out });
				}
				return outProperties;
            }
        }
        public override IEnumerable<string> HeaderRows
        {
			get
            {
				List<string> outHeaderRows = new List<string>();
				if (schematicChannelAsset != null)
				{
					outHeaderRows.Add(Path.GetFileName(schematicChannelAsset.Name));
				}
				return outHeaderRows;
            }
        }

        private SchematicChannelAsset schematicChannelAsset;

		public SchematicChannelEntity(FrostySdk.Ebx.SchematicChannelEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
			SetFlags(EntityFlags.HasLogic);
			schematicChannelAsset = LoadedAssetManager.Instance.LoadAsset<SchematicChannelAsset>(this, Data.Channel);
			schematicChannelAsset.AddEntity(this);
		}

        public override void OnEvent(int eventHash)
        {
			schematicChannelAsset.OnEvent(this, eventHash);
            base.OnEvent(eventHash);
        }

        public override IEvent GetEvent(int nameHash)
        {
            IEvent evt = base.GetEvent(nameHash);
			if (evt == null)
			{
				evt = new Event<OutputEvent>(this, nameHash);
			}
			return evt;
        }

        public override IProperty GetProperty(int nameHash)
        {
			IProperty property = base.GetProperty(nameHash);
			if (property == null)
            {
				property = new Property<object>(this, nameHash);
            }
			return property;
        }

        public override void OnPropertyChanged(int propertyHash)
        {
			schematicChannelAsset.OnPropertyUpdated(this, GetProperty(propertyHash));
            base.OnPropertyChanged(propertyHash);
        }

        public override void Destroy()
        {
			LoadedAssetManager.Instance.UnloadAsset(schematicChannelAsset);
        }
    }
}

