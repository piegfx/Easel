using System.Collections.Generic;
using Easel.Entities.Components;
using Easel.Graphics;

namespace Easel.Entities;

public class Prefab
{
    public Entity[] AddMeshes(Entity baseEntity, MaterialMesh[] meshes)
    {
        List<Entity> entities = new List<Entity>();

        foreach (MaterialMesh mesh in meshes)
        {
            Entity entity = new Entity()
            {
                Parent = baseEntity
            };
            
            entity.AddComponent(new MeshRenderer(mesh));
            
            entities.Add(entity);
        }

        return entities.ToArray();
    }
}