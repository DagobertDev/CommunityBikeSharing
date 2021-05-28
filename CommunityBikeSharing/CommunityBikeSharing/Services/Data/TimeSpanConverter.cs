#nullable enable
using System;
using Plugin.CloudFirestore;
using Plugin.CloudFirestore.Converters;

namespace CommunityBikeSharing.Services.Data
{
	public class TimeSpanConverter : DocumentConverter
	{
		public TimeSpanConverter(Type targetType) : base(targetType)
		{
			if (targetType != typeof(TimeSpan))
			{
				throw new ArgumentException($"{nameof(TimeSpanConverter)} only works for {nameof(TimeSpan)}");
			}
		}

		public override bool ConvertFrom(DocumentObject value, out object? result)
		{
			switch (value.Type)
			{
				case DocumentObjectType.Int64:
				{
					var number = value.Int64;
					result = TimeSpan.FromMinutes(number);
					return true;
				}
				case DocumentObjectType.Double:
				{
					var number = value.Double;
					result = TimeSpan.FromMinutes(number);
					return true;
				}
				default:
					throw new NotImplementedException($"{value.Type} can't be converted to {nameof(TimeSpan)}");
			}
		}

		public override bool ConvertTo(object? value, out object? result)
		{
			if (value is TimeSpan timeSpan)
			{
				result = timeSpan.TotalMinutes;
				return true;
			}

			result = 0;
			return true;
		}
	}
}
