using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace InfinityCore.API.Interface
{
    public interface ITreeHook
    {
        /// <summary>
        /// Executed right after the tree is drawn, perfect to add thing like particle or glowmask!
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="position"></param>
        /// <param name="sourceRectangle"></param>
        /// <param name="origin"></param>
        void PostDrawTreeTop(SpriteBatch sb, Vector2 position, Rectangle? sourceRectangle, Vector2 origin);

        /// <summary>
        /// 
        /// </summary>
        /// <param name=""></param>
        /// <param name="position"></param>
        /// <param name="sourceRectangle"></param>
        /// <param name="origin"></param>
        void PostDrawTreeBranch(SpriteBatch sb, Vector2 position, Rectangle? sourceRectangle, Vector2 origin);
    }
}
