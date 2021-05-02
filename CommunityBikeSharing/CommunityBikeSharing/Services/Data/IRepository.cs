using System.Threading.Tasks;

namespace CommunityBikeSharing.Services.Data
{
	public interface IRepository<T>
	{
		Task<T> Add(T model);
		Task Update(T model);
		Task Delete(T model);
	}
}
