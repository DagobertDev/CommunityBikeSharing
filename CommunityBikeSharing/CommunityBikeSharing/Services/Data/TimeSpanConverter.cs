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
			if (value.Type == DocumentObjectType.Int64)
			{
				var number = value.Int64;
				result = TimeSpan.FromMinutes(number);
				return true;
			}

			result = null;
			return false;
		}

		public override bool ConvertTo(object? value, out object? result)
		{
			if (value is TimeSpan timeSpan)
			{
				result = timeSpan.Minutes;
				return true;
			}

			result = null;
			return false;
		}
	}
}
