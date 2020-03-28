using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfinityCore.API.Interface
{
	public interface ILoadable
	{
		/// <summary>
		/// Loader will set this variable the false if Load() return false, preventing to unload if it's not necessary
		/// </summary>
		bool IsLoaded { get;  }

		/// <summary>
		/// Load your stuff in here, nothing more simple than this concept
		/// </summary>
		/// <returns>
		/// - false : Prevent unloading, use it generally if you loaded nothing or an exception happened, I guess
		/// - true : Mean that there will be thing to unload
		/// </returns>
		bool Load();

		/// <summary>
		/// Unload the stuff you loaded here, super simple concept, will not be executed if AsLoaded is false
		/// </summary>
		/// <returns>
		/// - false : An error was thrown
		/// - true : ÉUser unloaded everything they had to unload
		/// </returns>
		bool Unload();
	}
}
