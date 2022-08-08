using FrostySdk;
using System.Collections.Generic;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.OutpostDataTableData))]
	public class OutpostDataTable : LogicEntity, IEntityData<FrostySdk.Ebx.OutpostDataTableData>
	{
		public new FrostySdk.Ebx.OutpostDataTableData Data => data as FrostySdk.Ebx.OutpostDataTableData;
		public override string DisplayName => "OutpostDataTable";
		public override IEnumerable<ConnectionDesc> Links
		{
			get => new List<ConnectionDesc>()
			{
				new ConnectionDesc("Table", Direction.In)
			};
		}
		public override IEnumerable<ConnectionDesc> Events
		{
			get
            {
				List<ConnectionDesc> outEvents = new List<ConnectionDesc>();
				for (int i = 0; i < Data.InEventHashes.Count; i++)
				{
					outEvents.Add(new ConnectionDesc() { Name = Utils.GetString((int)Data.InEventHashes[i]), Direction = Direction.In });
				}
				for (int i = 0; i < Data.OutEventHashes.Count; i++)
                {
					outEvents.Add(new ConnectionDesc() { Name = Utils.GetString((int)Data.OutEventHashes[i]), Direction = Direction.Out });
				}
				outEvents.Add(new ConnectionDesc("ReadFromTable", Direction.In));
				outEvents.Add(new ConnectionDesc("WriteToTable", Direction.In));
				outEvents.Add(new ConnectionDesc("OnRead", Direction.Out));
				outEvents.Add(new ConnectionDesc("OnWrite", Direction.Out));
				return outEvents;
            }
		}
		public override IEnumerable<ConnectionDesc> Properties
		{
			get
			{
				List<ConnectionDesc> outProperties = new List<ConnectionDesc>();
				for (int i = 0; i < Data.FloatHashes.Count; i++)
				{
					outProperties.Add(new ConnectionDesc() { Name = Utils.GetString((int)Data.FloatHashes[i]), Direction = Direction.In });
					outProperties.Add(new ConnectionDesc() { Name = Utils.GetString((int)Data.FloatHashes[i]), Direction = Direction.Out });
				}
				for (int i = 0; i < Data.IntHashes.Count; i++)
				{
					outProperties.Add(new ConnectionDesc() { Name = Utils.GetString((int)Data.IntHashes[i]), Direction = Direction.In });
					outProperties.Add(new ConnectionDesc() { Name = Utils.GetString((int)Data.IntHashes[i]), Direction = Direction.Out });
				}
				for (int i = 0; i < Data.BoolHashes.Count; i++)
				{
					outProperties.Add(new ConnectionDesc() { Name = Utils.GetString((int)Data.BoolHashes[i]), Direction = Direction.In });
					outProperties.Add(new ConnectionDesc() { Name = Utils.GetString((int)Data.BoolHashes[i]), Direction = Direction.Out });
				}
				for (int i = 0; i < Data.TransformHashes.Count; i++)
				{
					outProperties.Add(new ConnectionDesc() { Name = Utils.GetString((int)Data.TransformHashes[i]), Direction = Direction.In });
					outProperties.Add(new ConnectionDesc() { Name = Utils.GetString((int)Data.TransformHashes[i]), Direction = Direction.Out });
				}
				return outProperties;
			}
		}

		public OutpostDataTable(FrostySdk.Ebx.OutpostDataTableData inData, Entity inParent)
			: base(inData, inParent)
		{
		}
	}
}

