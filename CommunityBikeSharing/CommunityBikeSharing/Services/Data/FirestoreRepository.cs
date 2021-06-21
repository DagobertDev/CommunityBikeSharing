using System;
using System.Threading.Tasks;
using Plugin.CloudFirestore;

namespace CommunityBikeSharing.Services.Data
{
	public abstract class FirestoreRepository<T> : IRepository<T>
	{
		public async Task<T> Add(T model)
		{
			var document = GetNewDocument(model);
			await document.SetAsync(model);
			var snap = await document.GetAsync();
			return snap.ToObject<T>() ?? throw new ApplicationException("Could not get model");
		}

		public Task Update(T model) => GetDocument(model).UpdateAsync(model);

		public Task Update(T model, string field, object? value) =>
			GetDocument(model).UpdateAsync(FieldPath.GetMappingName<T>(field), value ?? FieldValue.Delete);

		public Task Delete(T model) => GetDocument(model).DeleteAsync();

		public void Add(T model, ITransaction transaction)
		{
			var document = GetNewDocument(model);
			transaction.Set(document, model);
		}

		public void Update(T model, ITransaction transaction)
		{
			transaction.Update(GetDocument(model), model);
		}

		public void Update(T model, string field, object? value, ITransaction transaction)
		{
			transaction.Update(GetDocument(model), FieldPath.GetMappingName<T>(field), value ?? FieldValue.Delete);
		}

		public void Delete(T model, ITransaction transaction)
		{
			transaction.Delete(GetDocument(model));
		}

		protected abstract IDocumentReference GetDocument(T model);
		protected virtual IDocumentReference GetNewDocument(T model) => GetCollection(model).Document();
		protected abstract ICollectionReference GetCollection(T model);
	}
}
