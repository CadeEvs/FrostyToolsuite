using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frosty.Core.Viewport;
using LevelEditorPlugin.Editors;
using LevelEditorPlugin.Managers;
using LevelEditorPlugin.Render.Proxies;
using SharpDX;

namespace LevelEditorPlugin.Entities
{
#if MASS_EFFECT
    [EntityBinding(DataType = typeof(FrostySdk.Ebx.CharacterSpawnReferenceObjectData))]
    public class CharacterSpawnReferenceObject : SpawnReferenceObject, IEntityData<FrostySdk.Ebx.CharacterSpawnReferenceObjectData>
    {
        public new FrostySdk.Ebx.CharacterSpawnReferenceObjectData Data => data as FrostySdk.Ebx.CharacterSpawnReferenceObjectData;
        public override IEnumerable<ConnectionDesc> Links
        {
            get
            {
                List<ConnectionDesc> outLinks = new List<ConnectionDesc>();
                outLinks.AddRange(base.Links);
                outLinks.Add(new ConnectionDesc("ShaderParameters", Direction.In));
                outLinks.Add(new ConnectionDesc("Vehicle", Direction.In));
                return outLinks;
            }
        }
        public override IEnumerable<ConnectionDesc> Events
        {
            get
            {
                List<ConnectionDesc> outEvents = new List<ConnectionDesc>();
                outEvents.AddRange(base.Events);
                outEvents.Add(new ConnectionDesc("Kill", Direction.In));
                outEvents.Add(new ConnectionDesc("OnKilled", Direction.Out));
                return outEvents;
            }
        }
        public override IEnumerable<ConnectionDesc> Properties
        {
            get
            {
                List<ConnectionDesc> outProperties = new List<ConnectionDesc>();
                outProperties.AddRange(base.Properties);
                outProperties.Add(new ConnectionDesc("Immortal", Direction.In));
                outProperties.Add(new ConnectionDesc("FakeImmortal", Direction.In));
                outProperties.Add(new ConnectionDesc("ControllableTransform", Direction.Out));
                outProperties.Add(new ConnectionDesc("Transform", Direction.Out));
                return outProperties;
            }
        }

        public CharacterSpawnReferenceObject(FrostySdk.Ebx.CharacterSpawnReferenceObjectData inData, Entity inParent)
            : base(inData, inParent)
        {
        }
    }
#endif
}
