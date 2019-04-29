using Facepunch.Models;
using Facepunch.Models.Database;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;

namespace Facepunch
{
	public static class Database
	{
		public static void Count(string parent, Action<int> onResult)
		{
			if (onResult == null)
			{
				throw new ArgumentNullException("onResult");
			}
			string str = Application.Manifest.DatabaseUrl.Replace("{action}", "count");
			str = string.Concat(str, "&parent=", parent);
			WebClient webClient = new WebClient()
			{
				Encoding = Encoding.UTF8
			};
			webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler((object s, DownloadStringCompletedEventArgs e) => {
				int num = 0;
				try
				{
					JToken jTokens = JToken.Parse(e.Result);
					if (jTokens.Type == JTokenType.Integer)
					{
						num = (int)jTokens;
					}
				}
				catch (Exception exception)
				{
					num = -1;
				}
				Threading.QueueOnMainThread(() => onResult(num));
			});
			webClient.DownloadStringAsync(new Uri(str));
		}

		public static void Insert(string parent, object contents, Action<bool> onFinished)
		{
			Action<bool, string> action;
			string str = parent;
			object obj = contents;
			if (onFinished == null)
			{
				action = null;
			}
			else
			{
				action = (bool success, string id) => onFinished(success);
			}
			Database.Insert(str, obj, action);
		}

		public static void Insert(string parent, object contents, Action<bool, string> onFinished = null)
		{
			if (Application.Manifest == null || string.IsNullOrEmpty(Application.Manifest.DatabaseUrl))
			{
				return;
			}
			Add add = new Add()
			{
				Auth = Application.Integration.Auth,
				Content = JsonConvert.SerializeObject(contents),
				Parent = parent
			};
			NameValueCollection nameValueCollection = new NameValueCollection()
			{
				{ "data", JsonConvert.SerializeObject(add) }
			};
			Uri uri = new Uri(Application.Manifest.DatabaseUrl.Replace("{action}", "add"));
			WebClient webClient = new WebClient()
			{
				Encoding = Encoding.UTF8
			};
			if (onFinished != null)
			{
				webClient.UploadValuesCompleted += new UploadValuesCompletedEventHandler((object s, UploadValuesCompletedEventArgs e) => {
					AddResponse addResponse = JsonConvert.DeserializeObject<AddResponse>(Encoding.UTF8.GetString(e.Result));
					onFinished(addResponse.Status == "ok", addResponse.Id);
				});
			}
			webClient.UploadValuesAsync(uri, "POST", nameValueCollection);
		}

		public static Result<T> Query<T>(string parent, int limit, Action<Result<T>> onFinished = null)
		{
			Action action2 = null;
			Result<T> result = new Result<T>()
			{
				Running = true
			};
			string str = Application.Manifest.DatabaseUrl.Replace("{action}", "query");
			str = string.Concat(str, "&parent=", parent);
			str = string.Concat(str, "&limit=", limit);
			WebClient webClient = new WebClient()
			{
				Encoding = Encoding.UTF8
			};
			webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler((object s, DownloadStringCompletedEventArgs e) => {
				result.Running = false;
				try
				{
					QueryResponse[] queryResponseArray = JsonConvert.DeserializeObject<QueryResponse[]>(e.Result);
					Result<T> array = result;
					QueryResponse[] queryResponseArray1 = queryResponseArray;
					Func<QueryResponse, Result<T>.Entry> u003cu003e9_32 = Database.<>c__3<T>.<>9__3_2;
					if (u003cu003e9_32 == null)
					{
						u003cu003e9_32 = (QueryResponse x) => new Result<T>.Entry()
						{
							Id = x.Id,
							Created = x.Created,
							Updated = x.Updated,
							AuthorId = x.AuthorId,
							AuthType = x.AuthType,
							Content = JsonConvert.DeserializeObject<T>(x.Content)
						};
						Database.<>c__3<T>.<>9__3_2 = u003cu003e9_32;
					}
					array.Entries = ((IEnumerable<QueryResponse>)queryResponseArray1).Select<QueryResponse, Result<T>.Entry>(u003cu003e9_32).ToArray<Result<T>.Entry>();
					result.Success = true;
				}
				catch (Exception exception)
				{
					result.Success = false;
				}
				if (onFinished != null)
				{
					Action u003cu003e9_1 = action2;
					if (u003cu003e9_1 == null)
					{
						Action action = () => onFinished(result);
						Action action1 = action;
						action2 = action;
						u003cu003e9_1 = action1;
					}
					Threading.QueueOnMainThread(u003cu003e9_1);
				}
			});
			webClient.DownloadStringAsync(new Uri(str));
			return result;
		}

		public static void Remove(string parent, string id, Action<bool> onFinished = null)
		{
			if (Application.Manifest == null || string.IsNullOrEmpty(Application.Manifest.DatabaseUrl))
			{
				return;
			}
			Remove remove = new Remove()
			{
				Auth = Application.Integration.Auth,
				Id = id,
				Parent = parent
			};
			NameValueCollection nameValueCollection = new NameValueCollection()
			{
				{ "data", JsonConvert.SerializeObject(remove) }
			};
			Uri uri = new Uri(Application.Manifest.DatabaseUrl.Replace("{action}", "remove"));
			WebClient webClient = new WebClient()
			{
				Encoding = Encoding.UTF8
			};
			if (onFinished != null)
			{
				webClient.UploadValuesCompleted += new UploadValuesCompletedEventHandler((object s, UploadValuesCompletedEventArgs e) => {
					BaseResponse baseResponse = JsonConvert.DeserializeObject<BaseResponse>(Encoding.UTF8.GetString(e.Result));
					onFinished(baseResponse.Status == "ok");
				});
			}
			webClient.UploadValuesAsync(uri, "POST", nameValueCollection);
		}
	}
}