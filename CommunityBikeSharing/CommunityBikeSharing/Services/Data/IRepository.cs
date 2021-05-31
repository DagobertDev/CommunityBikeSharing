#nullable enable
using System.Threading.Tasks;
using Plugin.CloudFirestore;

namespace CommunityBikeSharing.Services.Data
{
	public interface IRepository<T>
	{
		Task<T> Add(T model);
		Task Update(T model);
		Task Update(T model, string field, object? value);
		Task Delete(T model);
		void Add(T model, ITransaction transaction);
		void Update(T model, ITransaction transaction);
		void Update(T model, string field, object? value, ITransaction transaction);
		void Delete(T model, ITransaction transaction);
	}
}
