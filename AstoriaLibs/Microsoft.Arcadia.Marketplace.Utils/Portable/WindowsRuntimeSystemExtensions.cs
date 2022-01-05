using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Microsoft.Arcadia.Marketplace.Utils.Portable
{
	public static class WindowsRuntimeSystemExtensions
	{
		public static Task AsTask(this IAsyncAction action)
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Expected O, but got Unknown
			TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
			action.Completed = ((AsyncActionCompletedHandler)delegate
			{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected I4, but got Unknown
			AsyncStatus status = action.Status;
				switch (status - 1)
				{
					case 0:
						tcs.SetResult(null);
						break;
					case (AsyncStatus)2:
						tcs.SetException(action.ErrorCode);
						break;
					case (AsyncStatus)1:
						tcs.SetCanceled();
						break;
				}
			});
			return tcs.Task;
		}

		public static Task AsTask<TProgress>(this IAsyncActionWithProgress<TProgress> action)
		{
			TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
			action.Completed = ((AsyncActionWithProgressCompletedHandler<TProgress>)delegate
			{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected I4, but got Unknown
			AsyncStatus status = action.Status;
				switch (status - 1)
				{
					case 0:
						tcs.SetResult(null);
						break;
					case (AsyncStatus)2:
						tcs.SetException(action.ErrorCode);
						break;
					case (AsyncStatus)1:
						tcs.SetCanceled();
						break;
				}
			});
			return tcs.Task;
		}

		public static Task<TResult> AsTask<TResult>(this IAsyncOperation<TResult> operation)
		{
			TaskCompletionSource<TResult> tcs = new TaskCompletionSource<TResult>();
			operation.Completed = ((AsyncOperationCompletedHandler<TResult>)delegate
			{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected I4, but got Unknown
			AsyncStatus status = operation.Status;
				switch (status - 1)
				{
					case 0:
						tcs.SetResult(operation.GetResults());
						break;
					case (AsyncStatus)2:
						tcs.SetException(operation.ErrorCode);
						break;
					case (AsyncStatus)1:
						tcs.SetCanceled();
						break;
				}
			});
			return tcs.Task;
		}

		public static Task<TResult> AsTask<TResult, TProgress>(this IAsyncOperationWithProgress<TResult, TProgress> operation)
		{
			TaskCompletionSource<TResult> tcs = new TaskCompletionSource<TResult>();
			operation.Completed = ((AsyncOperationWithProgressCompletedHandler<TResult, TProgress>)delegate
			{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected I4, but got Unknown
			AsyncStatus status = operation.Status;
				switch (status - 1)
				{
					case 0:
						tcs.SetResult(operation.GetResults());
						break;
					case (AsyncStatus)2:
						tcs.SetException(operation.ErrorCode);
						break;
					case (AsyncStatus)1:
						tcs.SetCanceled();
						break;
				}
			});
			return tcs.Task;
		}

		public static TaskAwaiter GetAwaiter(this IAsyncAction source)
		{
			return source.AsTask().GetAwaiter();
		}

		public static TaskAwaiter GetAwaiter<TProgress>(this IAsyncActionWithProgress<TProgress> source)
		{
			return source.AsTask().GetAwaiter();
		}

		public static TaskAwaiter<TResult> GetAwaiter<TResult>(this IAsyncOperation<TResult> source)
		{
			return source.AsTask().GetAwaiter();
		}

		public static TaskAwaiter<TResult> GetAwaiter<TResult, TProgress>(this IAsyncOperationWithProgress<TResult, TProgress> source)
		{
			return source.AsTask().GetAwaiter();
		}
	}
}