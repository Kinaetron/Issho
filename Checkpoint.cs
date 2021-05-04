using Microsoft.Xna.Framework;

using PolyOne;
using PolyOne.Collision;
using PolyOne.Scenes;

namespace Issho
{
    public class Checkpoint : Entity
    {
        public bool Reached { get; set; }


        public Checkpoint(Vector2 position) :
            base(position)
        {
            this.Tag((int)GameTags.Checkpoint);
            this.Collider = new Hitbox((float)16.0f, (float)80.0f, 0.0f, 0.0f);
            this.Visible = true;
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
        }

        public override void Update()
        {
            Reached = base.CollideCheck((int)GameTags.Player, Position);
            base.Update();
        }
    }
}
