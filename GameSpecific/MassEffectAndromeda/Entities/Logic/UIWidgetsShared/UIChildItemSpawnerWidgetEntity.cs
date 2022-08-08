using Frosty.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace LevelEditorPlugin.Entities
{
	[EntityBinding(DataType = typeof(FrostySdk.Ebx.UIChildItemSpawnerWidgetEntityData))]
	public class UIChildItemSpawnerWidgetEntity : UIWidgetEntity, IEntityData<FrostySdk.Ebx.UIChildItemSpawnerWidgetEntityData>
	{
		public new FrostySdk.Ebx.UIChildItemSpawnerWidgetEntityData Data => data as FrostySdk.Ebx.UIChildItemSpawnerWidgetEntityData;
		public override string DisplayName => "UIChildItemSpawnerWidget";
		public override IEnumerable<string> HeaderRows
		{
			get
			{
				List<string> outHeaderRows = new List<string>();
				outHeaderRows.Add(App.AssetManager.GetEbxEntry(Data.ChildItemBlueprint.External.FileGuid).Filename);
				return outHeaderRows;
			}
		}

		protected List<Entity> childEntities = new List<Entity>();

		public UIChildItemSpawnerWidgetEntity(FrostySdk.Ebx.UIChildItemSpawnerWidgetEntityData inData, Entity inParent)
			: base(inData, inParent)
		{
		}

		public override void PerformLayout()
		{
			base.PerformLayout();
			foreach (var entity in childEntities)
			{
				if (entity is IUIWidget)
				{
					(entity as IUIWidget).PerformLayout();
				}
			}
		}

		public override void OnMouseMove(Point mousePos)
		{
			base.OnMouseMove(mousePos);
			foreach (var element in childEntities)
			{
				if (element is IUIWidget)
				{
					(element as IUIWidget).OnMouseMove(mousePos);
				}
			}
		}

		public override void OnMouseDown(Point mousePos, MouseButton button)
		{
			base.OnMouseDown(mousePos, button);
			foreach (var element in childEntities)
			{
				if (element is IUIWidget)
				{
					(element as IUIWidget).OnMouseDown(mousePos, button);
				}
			}
		}

		public override void OnMouseUp(Point mousePos, MouseButton button)
		{
			base.OnMouseUp(mousePos, button);
			foreach (var element in childEntities)
			{
				if (element is IUIWidget)
				{
					(element as IUIWidget).OnMouseUp(mousePos, button);
				}
			}
		}
	}
}

