using Terraria;

namespace InfinityCore.API.Pots.DropTable
{
    public class PortalCoinPotsDrop : PotsDrop
    {
        public PortalCoinPotsDrop(int dropChance) : base("PortalCoin", (int x, int y) => { return Main.rand.Next(dropChance) == 0; })
        {
        }

        public override void ExecuteDrop(int x, int y)
        {
            Projectile.NewProjectile(x * 16 + 16, y * 16 + 16, 0f, -12f, 518, 0, 0f, Main.myPlayer);
        }
    }
}
