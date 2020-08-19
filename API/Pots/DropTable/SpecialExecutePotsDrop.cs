using System;

namespace InfinityCore.API.Pots.DropTable
{
    public class SpecialExecutePotsDrop : PotsDrop
    {
        private Action<int, int> execute;

        public SpecialExecutePotsDrop(string name, Func<int, int, bool> successFunc, Action<int, int> execute) : base(name, successFunc)
        {
            this.execute = execute;
        }

        public override void ExecuteDrop(int x, int y)
        {
            execute(x, y);
        }
    }
}
