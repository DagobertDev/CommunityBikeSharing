using System.Collections.Generic;

namespace CommunityBikeSharing.Controls
{
	public class ItemGroup : List<object>
	{
		public string Name { get; }

		public ItemGroup(string name, IEnumerable<object> list) : base(list)
		{
			Name = name;
		}
	}
}
